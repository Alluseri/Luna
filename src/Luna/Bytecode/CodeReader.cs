using Alluseri.Luna.Internals;
using Alluseri.Luna.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using static Alluseri.Luna.BadInstructionReadException;

namespace Alluseri.Luna.Bytecode;

// TODO: Make Instruction.Read() internal because labels are UB.
// DESIGN: Why don't I store MemoryStream for Code per-instance? Just try-finally it properly.
// TODO: This is generally not very well-protected against memory leak attacks because I usually don't check integers. This is a mistake.

public class CodeReader {
	private InternalClass Class;

	public CodeReader(InternalClass Class) {
		this.Class = Class;
	}

	public List<Instruction>? Read(CodeAttribute Code) {
		// TODO: Verify the validity of all bytecode locations for **all pseudos**. Branches fail immediately(because of SMT). Others need inspection in JVM verifier.

		// TODO: Benchmark 2 dicts vs 1 dict with valuetuple as value

		Dictionary<int, Label> LabelIndexes = new();
		Dictionary<int, List<PseudoInstruction>> PseudoInsnMap = new();
		List<(Instruction Instruction, int Location)> Instructions = new();

		var LineTable = ((LineNumberTableAttribute?) Code.Attributes.FirstOrDefault(T => T is LineNumberTableAttribute))?.Lines;
		if (LineTable != null) {
			foreach (LineEntry Le in LineTable) {
				PseudoInsnMap.GetOrNew(Le.InstructionIndex).Add(new LineNumber(Le.LineNumber));
			}
		}

		for (int i = 0; i < Code.ExceptionTable.Count; i++) {
			ExceptionHandler EH = Code.ExceptionTable[i];
			string Identifier = $"EH_{i}";
			PseudoInsnMap.GetOrNew(EH.Start).Add(new TryBlockStart(Identifier));
			PseudoInsnMap.GetOrNew(EH.End).Add(new TryBlockEnd(Identifier));
			PseudoInsnMap.GetOrNew(EH.Handler).Add(new TryCatchHandler(Identifier, EH.GetCatchType(Class.ConstantPool)?.GetName(Class.ConstantPool)));
		}

		using (MemoryStream Mes = new(Code.Bytecode, false)) {
			for (int i = 0; Mes.Position < Mes.Length; i++) {
				int Location = (int) Mes.Position;

				Instruction? Insn = ReadInstruction(Mes, Class, Location);

				Instructions.Add((Insn, Location));

				// TODO: Special case for TableSwitch.
				if (Insn is AbstractSingleBranchInstruction Bop) {
					if (!LabelIndexes.TryGetValue(Bop.TargetLocation, out Label? Lab)) {
						PseudoInsnMap.GetOrNew(Bop.TargetLocation).Add(LabelIndexes[Bop.TargetLocation] = Lab = new($"LAB_{Bop.TargetLocation}"));
					}
					Bop.Target = Lab;
				} else if (Insn is InsnLookupSwitch Ils) {
					if (!LabelIndexes.TryGetValue(Ils.DefaultTargetLocation, out Label? DefaultLab)) {
						PseudoInsnMap.GetOrNew(Ils.DefaultTargetLocation).Add(LabelIndexes[Ils.DefaultTargetLocation] = DefaultLab = new($"LAB_{Ils.DefaultTargetLocation}"));
					}
					Ils.DefaultCase = DefaultLab;
					Dictionary<int, Label> TargetDict = new(Ils.TargetLocations.Count);
					foreach (KeyValuePair<int, int> Case in Ils.TargetLocations) {
						int CaseLocation = Case.Value;
						if (!LabelIndexes.TryGetValue(CaseLocation, out Label? CaseLab)) {
							PseudoInsnMap.GetOrNew(CaseLocation).Add(LabelIndexes[CaseLocation] = CaseLab = new($"LAB_{CaseLocation}"));
						}
						TargetDict[Case.Key] = CaseLab;
					}
					Ils.Cases = TargetDict;
				}
			}
		}

		List<Instruction> FinalInstructions = new(Instructions.Count + PseudoInsnMap.Sum(L => L.Value.Count));

		foreach ((Instruction Instruction, int Location) in Instructions) {
			if (PseudoInsnMap.TryGetValue(Location, out List<PseudoInstruction>? Pseudos))
				FinalInstructions.AddRange(Pseudos);

			FinalInstructions.Add(Instruction);
		}

		return FinalInstructions;
	}

	private static Instruction ReadInstruction(Stream Stream, InternalClass Class, int Address) {
		if (!Stream.ReadByte(out byte Op))
			throw new EndOfStreamException();
		// Luckily, the rules here aren't as loose as they are with Attributes. There's no such thing as "overreading" due to stupid size values,
		// therefore we can just refer to constructors for argless instructions.
		return (Opcode) Op switch {
			Opcode.Nop => new InsnNop(),

			#region Constants
			Opcode.AConst_Null => new InsnPushNull(),

			Opcode.IConst_M1 => new InsnPushInteger(-1),
			Opcode.IConst_0 => new InsnPushInteger(0),
			Opcode.IConst_1 => new InsnPushInteger(1),
			Opcode.IConst_2 => new InsnPushInteger(2),
			Opcode.IConst_3 => new InsnPushInteger(3),
			Opcode.IConst_4 => new InsnPushInteger(4),
			Opcode.IConst_5 => new InsnPushInteger(5),

			Opcode.LConst_0 => new InsnPushLong(0),
			Opcode.LConst_1 => new InsnPushLong(1),

			Opcode.FConst_0 => new InsnPushFloat(0),
			Opcode.FConst_1 => new InsnPushFloat(1),
			Opcode.FConst_2 => new InsnPushFloat(2),

			Opcode.DConst_0 => new InsnPushDouble(0),
			Opcode.DConst_1 => new InsnPushDouble(1),

			Opcode.BiPush => Stream.ReadSByte(out sbyte V) ? new InsnPushInteger(V) : throw StreamUnderread,
			Opcode.SiPush => Stream.ReadShort(out short V) ? new InsnPushInteger(V) : throw StreamUnderread,

			Opcode.Ldc => Stream.ReadByte(out byte LdcIndex) ? ReadLdc(LdcIndex, Class.ConstantPool, false) : throw StreamUnderread,
			Opcode.Ldc_W => Stream.ReadUShort(out ushort LdcIndex) ? ReadLdc(LdcIndex, Class.ConstantPool, false) : throw StreamUnderread,
			Opcode.Ldc2_W => Stream.ReadUShort(out ushort LdcIndex) ? ReadLdc(LdcIndex, Class.ConstantPool, true) : throw StreamUnderread,
			#endregion

			#region Locals (Load)
			Opcode.ILoad => Stream.ReadByte(out byte LoadSlot) ? new InsnLoadInteger(LoadSlot) : throw StreamUnderread,
			Opcode.LLoad => Stream.ReadByte(out byte LoadSlot) ? new InsnLoadLong(LoadSlot) : throw StreamUnderread,
			Opcode.FLoad => Stream.ReadByte(out byte LoadSlot) ? new InsnLoadFloat(LoadSlot) : throw StreamUnderread,
			Opcode.DLoad => Stream.ReadByte(out byte LoadSlot) ? new InsnLoadDouble(LoadSlot) : throw StreamUnderread,
			Opcode.ALoad => Stream.ReadByte(out byte LoadSlot) ? new InsnLoadReference(LoadSlot) : throw StreamUnderread,

			Opcode.ILoad_0 => new InsnLoadInteger(0),
			Opcode.ILoad_1 => new InsnLoadInteger(1),
			Opcode.ILoad_2 => new InsnLoadInteger(2),
			Opcode.ILoad_3 => new InsnLoadInteger(3),

			Opcode.LLoad_0 => new InsnLoadLong(0),
			Opcode.LLoad_1 => new InsnLoadLong(1),
			Opcode.LLoad_2 => new InsnLoadLong(2),
			Opcode.LLoad_3 => new InsnLoadLong(3),

			Opcode.FLoad_0 => new InsnLoadFloat(0),
			Opcode.FLoad_1 => new InsnLoadFloat(1),
			Opcode.FLoad_2 => new InsnLoadFloat(2),
			Opcode.FLoad_3 => new InsnLoadFloat(3),

			Opcode.DLoad_0 => new InsnLoadDouble(0),
			Opcode.DLoad_1 => new InsnLoadDouble(1),
			Opcode.DLoad_2 => new InsnLoadDouble(2),
			Opcode.DLoad_3 => new InsnLoadDouble(3),

			Opcode.ALoad_0 => new InsnLoadReference(0),
			Opcode.ALoad_1 => new InsnLoadReference(1),
			Opcode.ALoad_2 => new InsnLoadReference(2),
			Opcode.ALoad_3 => new InsnLoadReference(3),
			#endregion

			#region Arrays (Load)
			Opcode.IALoad => new InsnLoadFromArray(ArrayType.Int),
			Opcode.LALoad => new InsnLoadFromArray(ArrayType.Long),
			Opcode.FALoad => new InsnLoadFromArray(ArrayType.Float),
			Opcode.DALoad => new InsnLoadFromArray(ArrayType.Double),
			Opcode.AALoad => new InsnLoadFromArray(ArrayType.Reference),
			Opcode.BALoad => new InsnLoadFromArray(ArrayType.ByteBool),
			Opcode.CALoad => new InsnLoadFromArray(ArrayType.Char),
			Opcode.SALoad => new InsnLoadFromArray(ArrayType.Short),
			#endregion

			#region Locals (Store)
			Opcode.IStore => Stream.ReadByte(out byte StoreSlot) ? new InsnStoreInteger(StoreSlot) : throw StreamUnderread,
			Opcode.LStore => Stream.ReadByte(out byte StoreSlot) ? new InsnStoreLong(StoreSlot) : throw StreamUnderread,
			Opcode.FStore => Stream.ReadByte(out byte StoreSlot) ? new InsnStoreFloat(StoreSlot) : throw StreamUnderread,
			Opcode.DStore => Stream.ReadByte(out byte StoreSlot) ? new InsnStoreDouble(StoreSlot) : throw StreamUnderread,
			Opcode.AStore => Stream.ReadByte(out byte StoreSlot) ? new InsnStoreReference(StoreSlot) : throw StreamUnderread,

			Opcode.IStore_0 => new InsnStoreInteger(0),
			Opcode.IStore_1 => new InsnStoreInteger(1),
			Opcode.IStore_2 => new InsnStoreInteger(2),
			Opcode.IStore_3 => new InsnStoreInteger(3),

			Opcode.LStore_0 => new InsnStoreLong(0),
			Opcode.LStore_1 => new InsnStoreLong(1),
			Opcode.LStore_2 => new InsnStoreLong(2),
			Opcode.LStore_3 => new InsnStoreLong(3),

			Opcode.FStore_0 => new InsnStoreFloat(0),
			Opcode.FStore_1 => new InsnStoreFloat(1),
			Opcode.FStore_2 => new InsnStoreFloat(2),
			Opcode.FStore_3 => new InsnStoreFloat(3),

			Opcode.DStore_0 => new InsnStoreDouble(0),
			Opcode.DStore_1 => new InsnStoreDouble(1),
			Opcode.DStore_2 => new InsnStoreDouble(2),
			Opcode.DStore_3 => new InsnStoreDouble(3),

			Opcode.AStore_0 => new InsnStoreReference(0),
			Opcode.AStore_1 => new InsnStoreReference(1),
			Opcode.AStore_2 => new InsnStoreReference(2),
			Opcode.AStore_3 => new InsnStoreReference(3),
			#endregion

			#region Arrays (Store)
			Opcode.IAStore => new InsnStoreInArray(ArrayType.Int),
			Opcode.LAStore => new InsnStoreInArray(ArrayType.Long),
			Opcode.FAStore => new InsnStoreInArray(ArrayType.Float),
			Opcode.DAStore => new InsnStoreInArray(ArrayType.Double),
			Opcode.AAStore => new InsnStoreInArray(ArrayType.Reference),
			Opcode.BAStore => new InsnStoreInArray(ArrayType.ByteBool),
			Opcode.CAStore => new InsnStoreInArray(ArrayType.Char),
			Opcode.SAStore => new InsnStoreInArray(ArrayType.Short),
			#endregion

			#region Stack
			Opcode.Pop => new InsnPop(),
			Opcode.Pop2 => new InsnPop2(),
			Opcode.Dup => new InsnDup(),
			Opcode.Dup_X1 => new InsnDup_X1(),
			Opcode.Dup_X2 => new InsnDup_X2(),
			Opcode.Dup2 => new InsnDup2(),
			Opcode.Dup2_X1 => new InsnDup2_X1(),
			Opcode.Dup2_X2 => new InsnDup2_X2(),
			Opcode.Swap => new InsnSwap(),
			#endregion

			#region Arithmetic
			Opcode.IAdd => new InsnAdd(ArithmeticOperand.Integer),
			Opcode.LAdd => new InsnAdd(ArithmeticOperand.Long),
			Opcode.FAdd => new InsnAdd(ArithmeticOperand.Float),
			Opcode.DAdd => new InsnAdd(ArithmeticOperand.Double),

			Opcode.ISub => new InsnSubtract(ArithmeticOperand.Integer),
			Opcode.LSub => new InsnSubtract(ArithmeticOperand.Long),
			Opcode.FSub => new InsnSubtract(ArithmeticOperand.Float),
			Opcode.DSub => new InsnSubtract(ArithmeticOperand.Double),

			Opcode.IMul => new InsnMultiply(ArithmeticOperand.Integer),
			Opcode.LMul => new InsnMultiply(ArithmeticOperand.Long),
			Opcode.FMul => new InsnMultiply(ArithmeticOperand.Float),
			Opcode.DMul => new InsnMultiply(ArithmeticOperand.Double),

			Opcode.IDiv => new InsnDivide(ArithmeticOperand.Integer),
			Opcode.LDiv => new InsnDivide(ArithmeticOperand.Long),
			Opcode.FDiv => new InsnDivide(ArithmeticOperand.Float),
			Opcode.DDiv => new InsnDivide(ArithmeticOperand.Double),

			Opcode.IRem => new InsnRemainder(ArithmeticOperand.Integer),
			Opcode.LRem => new InsnRemainder(ArithmeticOperand.Long),
			Opcode.FRem => new InsnRemainder(ArithmeticOperand.Float),
			Opcode.DRem => new InsnRemainder(ArithmeticOperand.Double),

			Opcode.INeg => new InsnNegate(ArithmeticOperand.Integer),
			Opcode.LNeg => new InsnNegate(ArithmeticOperand.Long),
			Opcode.FNeg => new InsnNegate(ArithmeticOperand.Float),
			Opcode.DNeg => new InsnNegate(ArithmeticOperand.Double),
			#endregion

			#region Arithmetic (Bitwise)
			Opcode.IShl => new InsnShiftLeft(BitwiseOperand.Integer),
			Opcode.LShl => new InsnShiftLeft(BitwiseOperand.Long),

			Opcode.IShr => new InsnShiftRight(BitwiseOperand.Integer),
			Opcode.LShr => new InsnShiftRight(BitwiseOperand.Long),

			Opcode.IUShr => new InsnUnsignedShiftRight(BitwiseOperand.Integer),
			Opcode.LUShr => new InsnUnsignedShiftRight(BitwiseOperand.Long),

			Opcode.IAnd => new InsnAnd(BitwiseOperand.Integer),
			Opcode.LAnd => new InsnAnd(BitwiseOperand.Long),

			Opcode.IOr => new InsnOr(BitwiseOperand.Integer),
			Opcode.LOr => new InsnOr(BitwiseOperand.Long),

			Opcode.IXor => new InsnXor(BitwiseOperand.Integer),
			Opcode.LXor => new InsnXor(BitwiseOperand.Long),
			#endregion

			Opcode.IInc => Stream.ReadByte(out byte LocalIndex) && Stream.ReadSByte(out sbyte Increment) ? new InsnIncrementInteger(LocalIndex, Increment) : throw StreamUnderread,

			#region Arithmetic (Casts)
			Opcode.I2L => new InsnPrimitiveCast(ArithmeticOperand.Integer, PrimitiveCastResult.Long),
			Opcode.I2F => new InsnPrimitiveCast(ArithmeticOperand.Integer, PrimitiveCastResult.Float),
			Opcode.I2D => new InsnPrimitiveCast(ArithmeticOperand.Integer, PrimitiveCastResult.Double),

			Opcode.L2I => new InsnPrimitiveCast(ArithmeticOperand.Long, PrimitiveCastResult.Integer),
			Opcode.L2F => new InsnPrimitiveCast(ArithmeticOperand.Long, PrimitiveCastResult.Float),
			Opcode.L2D => new InsnPrimitiveCast(ArithmeticOperand.Long, PrimitiveCastResult.Double),

			Opcode.F2I => new InsnPrimitiveCast(ArithmeticOperand.Float, PrimitiveCastResult.Integer),
			Opcode.F2L => new InsnPrimitiveCast(ArithmeticOperand.Float, PrimitiveCastResult.Long),
			Opcode.F2D => new InsnPrimitiveCast(ArithmeticOperand.Float, PrimitiveCastResult.Double),

			Opcode.D2I => new InsnPrimitiveCast(ArithmeticOperand.Double, PrimitiveCastResult.Integer),
			Opcode.D2L => new InsnPrimitiveCast(ArithmeticOperand.Double, PrimitiveCastResult.Long),
			Opcode.D2F => new InsnPrimitiveCast(ArithmeticOperand.Double, PrimitiveCastResult.Float),

			Opcode.I2B => new InsnPrimitiveCast(ArithmeticOperand.Integer, PrimitiveCastResult.Byte),
			Opcode.I2C => new InsnPrimitiveCast(ArithmeticOperand.Integer, PrimitiveCastResult.Char),
			Opcode.I2S => new InsnPrimitiveCast(ArithmeticOperand.Integer, PrimitiveCastResult.Short),
			#endregion

			#region Arithmetic (Comparison)
			Opcode.LCmp => new InsnCompareLong(),
			Opcode.FCmpL => new InsnCompareFloat(WhenNaN.Lesser),
			Opcode.FCmpG => new InsnCompareFloat(WhenNaN.Greater),
			Opcode.DCmpL => new InsnCompareDouble(WhenNaN.Lesser),
			Opcode.DCmpG => new InsnCompareDouble(WhenNaN.Greater),
			#endregion

			#region Control Flow
			Opcode.If_0 => Stream.ReadShort(out short Offset) ? new InsnZeroBranch(BranchCondition.Equal, Address, Offset) : throw StreamUnderread,
			Opcode.If_Not0 => Stream.ReadShort(out short Offset) ? new InsnZeroBranch(BranchCondition.NotEqual, Address, Offset) : throw StreamUnderread,
			Opcode.If_LessThan0 => Stream.ReadShort(out short Offset) ? new InsnZeroBranch(BranchCondition.LessThan, Address, Offset) : throw StreamUnderread,
			Opcode.If_GreaterEqual0 => Stream.ReadShort(out short Offset) ? new InsnZeroBranch(BranchCondition.GreaterEqual, Address, Offset) : throw StreamUnderread,
			Opcode.If_GreaterThan0 => Stream.ReadShort(out short Offset) ? new InsnZeroBranch(BranchCondition.GreatherThan, Address, Offset) : throw StreamUnderread,
			Opcode.If_LessEqual0 => Stream.ReadShort(out short Offset) ? new InsnZeroBranch(BranchCondition.LessEqual, Address, Offset) : throw StreamUnderread,
			Opcode.If_ICmpEqual => Stream.ReadShort(out short Offset) ? new InsnIntegerBranch(BranchCondition.Equal, Address, Offset) : throw StreamUnderread,
			Opcode.If_ICmpNotEqual => Stream.ReadShort(out short Offset) ? new InsnIntegerBranch(BranchCondition.NotEqual, Address, Offset) : throw StreamUnderread,
			Opcode.If_ICmpLessThan => Stream.ReadShort(out short Offset) ? new InsnIntegerBranch(BranchCondition.LessThan, Address, Offset) : throw StreamUnderread,
			Opcode.If_ICmpGreaterEqual => Stream.ReadShort(out short Offset) ? new InsnIntegerBranch(BranchCondition.GreaterEqual, Address, Offset) : throw StreamUnderread,
			Opcode.If_ICmpGreaterThan => Stream.ReadShort(out short Offset) ? new InsnIntegerBranch(BranchCondition.GreatherThan, Address, Offset) : throw StreamUnderread,
			Opcode.If_ICmpLessEqual => Stream.ReadShort(out short Offset) ? new InsnIntegerBranch(BranchCondition.LessEqual, Address, Offset) : throw StreamUnderread,
			Opcode.If_ACmpEqual => Stream.ReadShort(out short Offset) ? new InsnReferenceBranch(ReferenceBranchCondition.Equal, Address, Offset) : throw StreamUnderread,
			Opcode.If_ACmpNotEqual => Stream.ReadShort(out short Offset) ? new InsnReferenceBranch(ReferenceBranchCondition.NotEqual, Address, Offset) : throw StreamUnderread,

			Opcode.Goto => Stream.ReadShort(out short Offset) ? new InsnGoto(Address, Offset) : throw StreamUnderread,
			Opcode.LegacyJsr => throw new NotSupportedException("The 'jsr' instruction is not supported."),
			Opcode.LegacyRet => throw new NotSupportedException("The 'ret' instruction is not supported."),
			Opcode.TableSwitch => ReadTableSwitch(Stream, Address),
			Opcode.LookupSwitch => ReadLookupSwitch(Stream, Address),

			Opcode.IReturn => new InsnReturnInteger(),
			Opcode.LReturn => new InsnReturnLong(),
			Opcode.FReturn => new InsnReturnFloat(),
			Opcode.DReturn => new InsnReturnDouble(),
			Opcode.AReturn => new InsnReturnRef(),
			Opcode.Return => new InsnReturn(),
			#endregion

			Opcode.GetStatic => new InsnGetStatic(ReadFieldRef(Stream, Class.ConstantPool)),
			Opcode.PutStatic => new InsnPutStatic(ReadFieldRef(Stream, Class.ConstantPool)),

			Opcode.GetField => new InsnGetField(ReadFieldRef(Stream, Class.ConstantPool)),
			Opcode.PutField => new InsnPutField(ReadFieldRef(Stream, Class.ConstantPool)),

			#region Invokes
			Opcode.InvokeVirtual => new InsnInvokeVirtual(ReadMethodRef(Stream, Class.ConstantPool)),
			Opcode.InvokeSpecial => new InsnInvokeSpecial(ReadMethodRef(Stream, Class.ConstantPool)),
			Opcode.InvokeStatic => new InsnInvokeStatic(ReadMethodRef(Stream, Class.ConstantPool)),
			Opcode.InvokeInterface => ReadInvokeInterface(Stream, Class.ConstantPool),
			Opcode.InvokeDynamic => ReadInvokeDynamic(Stream, Class),
			#endregion

			Opcode.New => new InsnCheckCast(ReadClassName(Stream, Class.ConstantPool)),
			Opcode.NewArray => Stream.ReadByte(out byte ArrType) ? new InsnNewPrimitiveArray((PrimitiveArrayType) ArrType) : throw StreamUnderread,
			Opcode.ANewArray => new InsnNewArray(ReadClassName(Stream, Class.ConstantPool)),
			Opcode.ArrayLength => new InsnArrayLength(),

			Opcode.AThrow => new InsnThrow(),
			Opcode.CheckCast => new InsnCheckCast(ReadClassName(Stream, Class.ConstantPool)),
			Opcode.InstanceOf => new InsnInstanceOf(ReadClassName(Stream, Class.ConstantPool)),

			Opcode.MonitorEnter => new InsnMonitorEnter(),
			Opcode.MonitorExit => new InsnMonitorExit(),
			Opcode.Wide => (Opcode) Stream.ReadByte() switch {
				Opcode.ILoad => Stream.ReadUShort(out ushort LoadSlot) ? new InsnLoadInteger(LoadSlot) : throw StreamUnderread,
				Opcode.LLoad => Stream.ReadUShort(out ushort LoadSlot) ? new InsnLoadLong(LoadSlot) : throw StreamUnderread,
				Opcode.FLoad => Stream.ReadUShort(out ushort LoadSlot) ? new InsnLoadFloat(LoadSlot) : throw StreamUnderread,
				Opcode.DLoad => Stream.ReadUShort(out ushort LoadSlot) ? new InsnLoadDouble(LoadSlot) : throw StreamUnderread,
				Opcode.ALoad => Stream.ReadUShort(out ushort LoadSlot) ? new InsnLoadReference(LoadSlot) : throw StreamUnderread,
				Opcode.IStore => Stream.ReadUShort(out ushort StoreSlot) ? new InsnStoreInteger(StoreSlot) : throw StreamUnderread,
				Opcode.LStore => Stream.ReadUShort(out ushort StoreSlot) ? new InsnStoreLong(StoreSlot) : throw StreamUnderread,
				Opcode.FStore => Stream.ReadUShort(out ushort StoreSlot) ? new InsnStoreFloat(StoreSlot) : throw StreamUnderread,
				Opcode.DStore => Stream.ReadUShort(out ushort StoreSlot) ? new InsnStoreDouble(StoreSlot) : throw StreamUnderread,
				Opcode.AStore => Stream.ReadUShort(out ushort StoreSlot) ? new InsnStoreReference(StoreSlot) : throw StreamUnderread,
				Opcode.IInc => Stream.ReadUShort(out ushort LocalIndex) && Stream.ReadShort(out short Increment) ? new InsnIncrementInteger(LocalIndex, Increment) : throw StreamUnderread,
				Opcode.LegacyRet => throw new NotSupportedException("The 'wide ret' instruction is not supported."),
				_ => throw new BadInstructionReadException("Invalid opcode received for Wide.")
			},
			Opcode.MultiANewArray => new InsnNewMultiArray(ReadClassName(Stream, Class.ConstantPool), Stream.ReadByteNullable() ?? throw StreamUnderread), // DESIGN: Technically, we could go into ctor and throw due to dimension check.

			Opcode.IfNull => Stream.ReadShort(out short Offset) ? new InsnIfNullBranch(Address, Offset) : throw StreamUnderread,
			Opcode.IfNotNull => Stream.ReadShort(out short Offset) ? new InsnIfNotNullBranch(Address, Offset) : throw StreamUnderread,
			Opcode.Goto_W => Stream.ReadInt(out int Offset) ? new InsnGoto(Address, Offset) : throw StreamUnderread,
			Opcode.LegacyJsr_W => throw new NotSupportedException("The 'jsr_w' instruction is not supported."),

			_ => throw new BadInstructionReadException($"Unknown opcode reached: {(Opcode) Op}")
		};
	}

	private static InsnLookupSwitch ReadLookupSwitch(Stream Stream, int Address) {
		int Alignment = (4 - ((Address + 1) % 4)) % 4;

		if (Stream.CanSeek)
			Stream.Seek(Alignment, SeekOrigin.Current);
		else
			Stream.Read(stackalloc byte[Alignment]);

		int DefaultOffset = Stream.ReadInt();
		int PairCount = Stream.ReadInt();

		if (PairCount < 0)
			throw new BadInstructionReadException("Pair count must be greater than or equal to 0."); // Why is it even signed XDD

		Dictionary<int, int> Pairs = new(PairCount);
		for (int i = 0; i < PairCount; i++) {
			int Match = Stream.ReadInt();
			int Offset = Stream.ReadInt();

			Pairs[Match] = Address + Offset;
		}
		return new(Address + DefaultOffset, Pairs);
	}

	private static InsnTableSwitch ReadTableSwitch(Stream Stream, int Address) {
		int Alignment = (4 - ((Address + 1) % 4)) % 4;

		if (Stream.CanSeek)
			Stream.Seek(Alignment, SeekOrigin.Current);
		else
			Stream.Read(stackalloc byte[Alignment]);

		int DefaultOffset = Stream.ReadInt();
		int MinValue = Stream.ReadInt();
		int MaxValue = Stream.ReadInt();

		if (MinValue > MaxValue)
			throw new BadInstructionReadException("Min value for TableSwitch must not be greater than the max value.");

		int OffsetCount = MaxValue - MinValue + 1;
		List<int> Targets = new(OffsetCount);

		for (int i = 0; i < OffsetCount; i++) {
			int Offset = Stream.ReadInt();

			Targets.Add(Address + Offset);
		}

		return new(Address + DefaultOffset, Targets);
	}

	private static Instruction ReadInvokeDynamic(Stream Stream, InternalClass Class) {
		ConstantInvokeDynamic Cindy = Class.ConstantPool.Value<ConstantInvokeDynamic>(Stream.ReadUShort());

		// Technically 00 will be parsed as NOP anyway, but OpenJDK demands:
		int B1 = Stream.ReadByte();
		int B2 = Stream.ReadByte();

		// if (B1 == -1 || B2 == -1)
		//	return null;

		ConstantNameAndType CalleeNAT = Cindy.GetNameAndType(Class.ConstantPool);
		MethodDescriptor Callee = MethodDescriptor.FromSignature(Class.ConstantPool, CalleeNAT);

		Internals.BootstrapMethod? InternalBootstrap = Cindy.GetBootstrapMethod(Class);

		if (InternalBootstrap == null)
			return new InsnMalformedIndy(Cindy.BootstrapMethodIndex, Callee);

		return new InsnInvokeDynamic(BootstrapMethod.FromInternal(Class, InternalBootstrap), Callee);
	}

	private static InsnInvokeInterface ReadInvokeInterface(Stream Stream, ConstantPool Pool) {
		ConstantInterfaceMethodRef IntRef = Pool.Value<ConstantInterfaceMethodRef>(Stream.ReadUShort());

		int Count = Stream.ReadByte();
		int B2 = Stream.ReadByte();

		// if (Count == -1 || B2 == -1)
		//	return null;

		return new InsnInvokeInterface(IntRef.GetClassName(Pool), MethodDescriptor.FromSignature(Pool, IntRef.GetNameAndType(Pool)));
	}

	private static string ReadClassName(Stream Stream, ConstantPool Pool) {
		if (!Stream.ReadUShort(out ushort Index))
			throw StreamUnderread;

		return Pool.Value<ConstantClass>(Index).GetName(Pool);
	}

	private static ManagedMethodReference ReadMethodRef(Stream Stream, ConstantPool Pool) {
		if (!Stream.ReadUShort(out ushort PoolIndex))
			throw StreamUnderread;

		ConstantInfo Ci = Pool[PoolIndex];

		if (Ci is ConstantMethodRef ConNorm) {
			return new() {
				ClassName = ConNorm.GetClassName(Pool),
				Method = MethodDescriptor.FromSignature(Pool, ConNorm.GetNameAndType(Pool)),
				Interface = false
			};
		} else if (Ci is ConstantInterfaceMethodRef ConInt) {
			return new() {
				ClassName = ConInt.GetClassName(Pool),
				Method = MethodDescriptor.FromSignature(Pool, ConInt.GetNameAndType(Pool)),
				Interface = true
			};
		} else
			throw ExpectedPool(Pool, "ConstantMethodRef or ConstantInterfaceMethodRef", PoolIndex);
	}
	private static ManagedFieldReference ReadFieldRef(Stream Stream, ConstantPool Pool) {
		if (!Stream.ReadUShort(out ushort PoolIndex))
			throw StreamUnderread;

		if (Pool[PoolIndex] is not ConstantFieldRef ConFld)
			throw ExpectedPool(Pool, "ConstantFieldRef", PoolIndex);

		return new() {
			ClassName = ConFld.GetClassName(Pool),
			Field = FieldDescriptor.FromSignature(Pool, ConFld.GetNameAndType(Pool))
		};
	}
	private static Instruction ReadLdc(ushort Index, ConstantPool Pool, bool Wide) {
		ConstantInfo Ci = Pool[Index];
		if (Ci.IsWide && !Wide)
			throw new BadInstructionReadException($"Mismatched state: requested a non-wide LDC, but the subject constant is wide.");
		else if (Ci is ConstantDynamic Cdyn) {
			// This thing is the reason why we'll have to pass Class here and not Pool, hahaha
			// Make sure it never has the type L or J if not Wide. That's illegal, apparently.
			throw new BadInstructionReadException($"Support for {Ci.Tag} as an LDC argument is not implemented at the moment.");
		} else if (Ci is ConstantClass Cc) {
			return new InsnPushClass(Cc.GetName(Pool));
		} else if (Ci is ConstantMethodHandle Cm) {
			return new InsnPushMethodHandle(new MethodHandle(Cm.Kind, ClassMemberReference.FromConstant(Pool, Cm.GetInfo(Pool))));
		} else if (Ci is ConstantMethodType Cmt) {
			return new InsnPushMethodType(MethodTypeDescriptor.FromSignature(Pool, Cmt));
		} else if (Ci is ConstantString Cs) {
			return new InsnPushString(Cs.GetString(Pool));
		} else if (Ci is ConstantInteger Cin)
			return new InsnPushInteger(Cin.Value);
		else if (Ci is ConstantFloat Cf)
			return new InsnPushFloat(Cf.Value);
		else if (Ci is ConstantLong Cl)
			return new InsnPushLong(Cl.Value);
		else if (Ci is ConstantDouble Cd)
			return new InsnPushDouble(Cd.Value);
		else {
			throw new BadInstructionReadException($"Read LDC with an unacceptable constant: {Ci}");
		}
	}

	public struct ManagedFieldReference {
		public string ClassName;
		public FieldDescriptor Field;
	}

	public struct ManagedMethodReference {
		public string ClassName;
		public MethodDescriptor Method;
		public bool Interface;
	}
}