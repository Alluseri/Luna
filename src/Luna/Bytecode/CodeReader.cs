using Alluseri.Luna.Internals;
using Alluseri.Luna.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using static Alluseri.Luna.BadInstructionReadException;

namespace Alluseri.Luna.Bytecode;

// TODO: This is generally not very well-protected against memory leak attacks because I usually don't check integers. That is a big mistake.

public class CodeReader {
	public readonly InternalClass Class;

	public CodeReader(InternalClass Class) {
		this.Class = Class;
	}

	public List<Instruction> Read(CodeAttribute Code) {
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
			PseudoInsnMap.GetOrNew(EH.Handler).Add(new TryBlockCatchHandler(Identifier, EH.GetCatchType(Class.ConstantPool)?.GetName(Class.ConstantPool)));
		}

		using (MemoryStream Mes = new(Code.Bytecode, false)) {
			for (int i = 0; Mes.Position < Mes.Length; i++) {
				int Address = (int) Mes.Position;

				Instruction? Insn = ReadInstruction(Mes, Class, Address);

				Instructions.Add((Insn, Address));

				if (Insn is AbstractSingleBranchInstruction Bop) {
					if (!LabelIndexes.TryGetValue(Bop.TargetLocation, out Label? Lab)) {
						PseudoInsnMap.GetOrNew(Bop.TargetLocation).Add(LabelIndexes[Bop.TargetLocation] = Lab = new($"LAB_{Bop.TargetLocation}"));
					}
					Bop.Target = Lab;
				} else if (Insn is InsnLookupSwitch Ils) {
					if (!LabelIndexes.TryGetValue(Ils.DefaultCaseLocation, out Label? DefaultLab)) {
						PseudoInsnMap.GetOrNew(Ils.DefaultCaseLocation).Add(LabelIndexes[Ils.DefaultCaseLocation] = DefaultLab = new($"LAB_{Ils.DefaultCaseLocation}"));
					}
					Ils.DefaultCase = DefaultLab;
					SortedList<int, Label> CaseMap = new(Ils.CaseLocations.Count);
					foreach (KeyValuePair<int, int> Case in Ils.CaseLocations) {
						int CaseLocation = Case.Value;
						if (!LabelIndexes.TryGetValue(CaseLocation, out Label? CaseLab)) {
							PseudoInsnMap.GetOrNew(CaseLocation).Add(LabelIndexes[CaseLocation] = CaseLab = new($"LAB_{CaseLocation}"));
						}
						CaseMap[Case.Key] = CaseLab;
					}
					Ils.Cases = CaseMap;
				} else if (Insn is InsnTableSwitch Its) {
					if (!LabelIndexes.TryGetValue(Its.DefaultTargetLocation, out Label? DefaultLab)) {
						PseudoInsnMap.GetOrNew(Its.DefaultTargetLocation).Add(LabelIndexes[Its.DefaultTargetLocation] = DefaultLab = new($"LAB_{Its.DefaultTargetLocation}"));
					}
					Its.DefaultCase = DefaultLab;
					List<Label> CaseList = new(Its.TargetLocations.Count);
					foreach (int CaseLocation in Its.TargetLocations) {
						if (!LabelIndexes.TryGetValue(CaseLocation, out Label? CaseLab)) {
							PseudoInsnMap.GetOrNew(CaseLocation).Add(LabelIndexes[CaseLocation] = CaseLab = new($"LAB_{CaseLocation}"));
						}
						CaseList.Add(CaseLab);
					}
					Its.Cases = CaseList;
				}
			}
		}

		List<Instruction> FinalInstructions = new(Instructions.Count + PseudoInsnMap.Sum(L => L.Value.Count));

		foreach ((Instruction Instruction, int Address) in Instructions) {
			if (PseudoInsnMap.TryGetValue(Address, out List<PseudoInstruction>? Pseudos))
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

			Opcode.IConst_M1 => new InsnPush(new StackConstantInteger(-1)),
			Opcode.IConst_0 => new InsnPush(new StackConstantInteger(0)),
			Opcode.IConst_1 => new InsnPush(new StackConstantInteger(1)),
			Opcode.IConst_2 => new InsnPush(new StackConstantInteger(2)),
			Opcode.IConst_3 => new InsnPush(new StackConstantInteger(3)),
			Opcode.IConst_4 => new InsnPush(new StackConstantInteger(4)),
			Opcode.IConst_5 => new InsnPush(new StackConstantInteger(5)),

			Opcode.LConst_0 => new InsnPush(new StackConstantLong(0)),
			Opcode.LConst_1 => new InsnPush(new StackConstantLong(1)),

			Opcode.FConst_0 => new InsnPush(new StackConstantFloat(0)),
			Opcode.FConst_1 => new InsnPush(new StackConstantFloat(1)),
			Opcode.FConst_2 => new InsnPush(new StackConstantFloat(2)),

			Opcode.DConst_0 => new InsnPush(new StackConstantDouble(0)),
			Opcode.DConst_1 => new InsnPush(new StackConstantDouble(1)),

			Opcode.BiPush => Stream.ReadSByte(out sbyte V) ? new InsnPush(new StackConstantInteger(V)) : throw StreamUnderread,
			Opcode.SiPush => Stream.ReadShort(out short V) ? new InsnPush(new StackConstantInteger(V)) : throw StreamUnderread,

			Opcode.Ldc => Stream.ReadByte(out byte LdcIndex) ? ReadLdc(LdcIndex, Class, false) : throw StreamUnderread,
			Opcode.Ldc_W => Stream.ReadUShort(out ushort LdcIndex) ? ReadLdc(LdcIndex, Class, false) : throw StreamUnderread,
			Opcode.Ldc2_W => Stream.ReadUShort(out ushort LdcIndex) ? ReadLdc(LdcIndex, Class, true) : throw StreamUnderread,
			#endregion

			#region Locals (Load)
			Opcode.ILoad or Opcode.LLoad or Opcode.FLoad or Opcode.DLoad or Opcode.ALoad => Stream.ReadByte(out byte LoadSlot) ? new InsnLoadLocal(GetSequentialKind((Opcode) Op, Opcode.ILoad), LoadSlot) : throw StreamUnderread,
			Opcode.ILoad_0 or Opcode.ILoad_1 or Opcode.ILoad_2 or Opcode.ILoad_3 or
			Opcode.LLoad_0 or Opcode.LLoad_1 or Opcode.LLoad_2 or Opcode.LLoad_3 or
			Opcode.FLoad_0 or Opcode.FLoad_1 or Opcode.FLoad_2 or Opcode.FLoad_3 or
			Opcode.DLoad_0 or Opcode.DLoad_1 or Opcode.DLoad_2 or Opcode.DLoad_3 or
			Opcode.ALoad_0 or Opcode.ALoad_1 or Opcode.ALoad_2 or Opcode.ALoad_3 => ReadCompactLoadLocal((Opcode) Op),
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
			Opcode.IStore or Opcode.LStore or Opcode.FStore or Opcode.DStore or Opcode.AStore => Stream.ReadByte(out byte StoreSlot) ? new InsnStoreLocal(GetSequentialKind((Opcode) Op, Opcode.IStore), StoreSlot) : throw StreamUnderread,
			Opcode.IStore_0 or Opcode.IStore_1 or Opcode.IStore_2 or Opcode.IStore_3 or
			Opcode.LStore_0 or Opcode.LStore_1 or Opcode.LStore_2 or Opcode.LStore_3 or
			Opcode.FStore_0 or Opcode.FStore_1 or Opcode.FStore_2 or Opcode.FStore_3 or
			Opcode.DStore_0 or Opcode.DStore_1 or Opcode.DStore_2 or Opcode.DStore_3 or
			Opcode.AStore_0 or Opcode.AStore_1 or Opcode.AStore_2 or Opcode.AStore_3 => ReadCompactStoreLocal((Opcode) Op),
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
			Opcode.If_GreaterThan0 => Stream.ReadShort(out short Offset) ? new InsnZeroBranch(BranchCondition.GreaterThan, Address, Offset) : throw StreamUnderread,
			Opcode.If_LessEqual0 => Stream.ReadShort(out short Offset) ? new InsnZeroBranch(BranchCondition.LessEqual, Address, Offset) : throw StreamUnderread,
			Opcode.If_ICmpEqual => Stream.ReadShort(out short Offset) ? new InsnIntegerBranch(BranchCondition.Equal, Address, Offset) : throw StreamUnderread,
			Opcode.If_ICmpNotEqual => Stream.ReadShort(out short Offset) ? new InsnIntegerBranch(BranchCondition.NotEqual, Address, Offset) : throw StreamUnderread,
			Opcode.If_ICmpLessThan => Stream.ReadShort(out short Offset) ? new InsnIntegerBranch(BranchCondition.LessThan, Address, Offset) : throw StreamUnderread,
			Opcode.If_ICmpGreaterEqual => Stream.ReadShort(out short Offset) ? new InsnIntegerBranch(BranchCondition.GreaterEqual, Address, Offset) : throw StreamUnderread,
			Opcode.If_ICmpGreaterThan => Stream.ReadShort(out short Offset) ? new InsnIntegerBranch(BranchCondition.GreaterThan, Address, Offset) : throw StreamUnderread,
			Opcode.If_ICmpLessEqual => Stream.ReadShort(out short Offset) ? new InsnIntegerBranch(BranchCondition.LessEqual, Address, Offset) : throw StreamUnderread,
			Opcode.If_ACmpEqual => Stream.ReadShort(out short Offset) ? new InsnReferenceBranch(ReferenceBranchCondition.Equal, Address, Offset) : throw StreamUnderread,
			Opcode.If_ACmpNotEqual => Stream.ReadShort(out short Offset) ? new InsnReferenceBranch(ReferenceBranchCondition.NotEqual, Address, Offset) : throw StreamUnderread,

			Opcode.Goto => Stream.ReadShort(out short Offset) ? new InsnGoto(Address, Offset) : throw StreamUnderread,
			Opcode.LegacyJsr => throw new NotSupportedException("The 'jsr' instruction is not supported."),
			Opcode.LegacyRet => throw new NotSupportedException("The 'ret' instruction is not supported."),
			Opcode.TableSwitch => ReadTableSwitch(Stream, Address),
			Opcode.LookupSwitch => ReadLookupSwitch(Stream, Address),

			Opcode.IReturn or Opcode.LReturn or Opcode.FReturn or Opcode.DReturn or Opcode.AReturn => new InsnReturn(GetSequentialKind((Opcode) Op, Opcode.IReturn)),
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

			Opcode.New => new InsnNew(ReadClassName(Stream, Class.ConstantPool)),
			Opcode.NewArray => Stream.ReadByte(out byte ArrType) ? new InsnNewPrimitiveArray((PrimitiveArrayType) ArrType) : throw StreamUnderread,
			Opcode.ANewArray => new InsnNewArray(ReadClassName(Stream, Class.ConstantPool)),
			Opcode.ArrayLength => new InsnArrayLength(),

			Opcode.AThrow => new InsnThrow(),
			Opcode.CheckCast => new InsnCheckCast(ReadClassName(Stream, Class.ConstantPool)),
			Opcode.InstanceOf => new InsnInstanceOf(ReadClassName(Stream, Class.ConstantPool)),

			Opcode.MonitorEnter => new InsnMonitorEnter(),
			Opcode.MonitorExit => new InsnMonitorExit(),
			Opcode.Wide => ReadWide(Stream),
			Opcode.MultiANewArray => new InsnNewMultiArray(ReadClassName(Stream, Class.ConstantPool), Stream.ReadByteNullable() ?? throw StreamUnderread), // DESIGN: Technically, we could go into ctor and throw due to dimension check.

			Opcode.IfNull => Stream.ReadShort(out short Offset) ? new InsnIfNullBranch(Address, Offset) : throw StreamUnderread,
			Opcode.IfNotNull => Stream.ReadShort(out short Offset) ? new InsnIfNotNullBranch(Address, Offset) : throw StreamUnderread,
			Opcode.Goto_W => Stream.ReadInt(out int Offset) ? new InsnGoto(Address, Offset) : throw StreamUnderread,
			Opcode.LegacyJsr_W => throw new NotSupportedException("The 'jsr_w' instruction is not supported."),

			_ => throw new BadInstructionReadException($"Unknown opcode reached: {(Opcode) Op}")
		};
	}

	private static Instruction ReadWide(Stream Stream) {
		if (!Stream.ReadByte(out byte Op))
			throw StreamUnderread;

		return (Opcode) Op switch {
			Opcode.ILoad or Opcode.LLoad or Opcode.FLoad or Opcode.DLoad or Opcode.ALoad => Stream.ReadUShort(out ushort LoadSlot) ? new InsnLoadLocal(GetSequentialKind((Opcode) Op, Opcode.ILoad), LoadSlot) : throw StreamUnderread,
			Opcode.IStore or Opcode.LStore or Opcode.FStore or Opcode.DStore or Opcode.AStore => Stream.ReadUShort(out ushort StoreSlot) ? new InsnStoreLocal(GetSequentialKind((Opcode) Op, Opcode.IStore), StoreSlot) : throw StreamUnderread,
			Opcode.IInc => Stream.ReadUShort(out ushort LocalIndex) && Stream.ReadShort(out short Increment) ? new InsnIncrementInteger(LocalIndex, Increment) : throw StreamUnderread,
			Opcode.LegacyRet => throw new NotSupportedException("The 'wide ret' instruction is not supported."),
			_ => throw new BadInstructionReadException("Invalid opcode received for Wide.")
		};
	}

	private static InsnLoadLocal ReadCompactLoadLocal(Opcode Op) {
		uint Offset = (uint) Op - (uint) Opcode.ILoad_0;
		return new(GetCompactKind(Offset), GetCompactSlot(Offset));
	}

	private static InsnStoreLocal ReadCompactStoreLocal(Opcode Op) {
		uint Offset = (uint) Op - (uint) Opcode.IStore_0;
		return new(GetCompactKind(Offset), GetCompactSlot(Offset));
	}

	private static ValueKind GetSequentialKind(Opcode Op, Opcode BaseOpcode) => (ValueKind) ((uint) Op - (uint) BaseOpcode);
	private static ValueKind GetCompactKind(uint Offset) => (ValueKind) (Offset / 4);
	private static ushort GetCompactSlot(uint Offset) => (ushort) (Offset % 4);

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
		int Alignment = (4 - ((Address + 1) % 4)) % 4; // God, what the fuck did I cook?

		if (Stream.CanSeek)
			Stream.Seek(Alignment, SeekOrigin.Current);
		else
			Stream.Read(stackalloc byte[Alignment]);

		int DefaultOffset = Stream.ReadInt();
		int MinValue = Stream.ReadInt();
		int MaxValue = Stream.ReadInt();

		if (MinValue > MaxValue)
			throw new BadInstructionReadException("Min. value for TableSwitch must not be greater than the max value.");

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

		return new InsnInvokeDynamic((BootstrapMethod) BootstrapMethod.FromInternal(Class, InternalBootstrap), Callee); // IMPORTANT TODO: Remove the cast and make sure INDY works with BootstrapMethodBase (unless we make it unnecessary by absolutely destroying every possible way of there being a CyclicBM on first argument level)
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
	private static InsnPush ReadLdc(ushort Index, InternalClass Class, bool Wide) {
		ConstantPool Pool = Class.ConstantPool;
		ConstantInfo Ci = Pool[Index];
		if (Ci.IsWide && !Wide)
			throw new BadInstructionReadException($"Mismatched state: requested a non-wide LDC, but the subject constant is wide.");
		// StackConstant Sci = StackConstant.FromConstant(Class, Ci);
		StackConstant Sci = StackConstant.FromConstant(Class, Index);

		if (!Wide && Sci is StackConstantDynamic Scd && Scd.IsWide)
			throw new BadInstructionReadException($"Mismatched state: requested a non-wide LDC (Condy), but the subject constant is wide.");

		return new(Sci);
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
