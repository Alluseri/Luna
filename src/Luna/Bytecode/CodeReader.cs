using Alluseri.Luna.Internals;
using Alluseri.Luna.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Alluseri.Luna.Bytecode;

// TODO: Make Instruction.Read() internal because labels are UB.

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
			Instruction? Insn;

			for (int i = 0; Mes.Position < Mes.Length; i++) {
				int Location = (int) Mes.Position;

				if ((Insn = ReadInstruction(Mes, Class, Location)) == null) {
					Console.WriteLine("Last instruction before failure was " + Instructions.Last().Instruction); // DEBUGTRACE
					return null;
				}

				Instructions.Add((Insn, Location));

				// TODO: Special case for TableSwitch, LookupSwitch and other multibranches. Worry later.
				if (Insn is AbstractSingleBranchInstruction Bop) {
					if (!LabelIndexes.TryGetValue(Bop.TargetLocation, out Label? Lab)) {
						PseudoInsnMap.GetOrNew(Bop.TargetLocation).Add(LabelIndexes[Bop.TargetLocation] = Lab = new($"LAB_{Bop.TargetLocation}"));
					}
					Bop.Target = Lab;
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

	private static Instruction? ReadInstruction(Stream Stream, InternalClass Class, int Address) {
		if (!Stream.ReadByte(out byte Op))
			return null;
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

			Opcode.BiPush => Stream.ReadSByte(out sbyte V) ? new InsnPushInteger(V) : null,
			Opcode.SiPush => Stream.ReadShort(out short V) ? new InsnPushInteger(V) : null,

			Opcode.Ldc => Stream.ReadByte(out byte LdcIndex) ? ReadLdc(LdcIndex, Class.ConstantPool, false) : null,
			Opcode.Ldc_W => Stream.ReadUShort(out ushort LdcIndex) ? ReadLdc(LdcIndex, Class.ConstantPool, false) : null,
			Opcode.Ldc2_W => Stream.ReadUShort(out ushort LdcIndex) ? ReadLdc(LdcIndex, Class.ConstantPool, true) : null,
			#endregion

			#region Locals (Load)
			Opcode.ILoad => Stream.ReadByte(out byte LoadSlot) ? new InsnLoadInteger(LoadSlot) : null,
			Opcode.LLoad => Stream.ReadByte(out byte LoadSlot) ? new InsnLoadLong(LoadSlot) : null,
			Opcode.FLoad => Stream.ReadByte(out byte LoadSlot) ? new InsnLoadFloat(LoadSlot) : null,
			Opcode.DLoad => Stream.ReadByte(out byte LoadSlot) ? new InsnLoadDouble(LoadSlot) : null,
			Opcode.ALoad => Stream.ReadByte(out byte LoadSlot) ? new InsnLoadRef(LoadSlot) : null,

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

			Opcode.ALoad_0 => new InsnLoadRef(0),
			Opcode.ALoad_1 => new InsnLoadRef(1),
			Opcode.ALoad_2 => new InsnLoadRef(2),
			Opcode.ALoad_3 => new InsnLoadRef(3),
			#endregion

			#region Arrays (Load)
			Opcode.IALoad => new InsnLoadIntegerFromArray(),
			Opcode.LALoad => new InsnLoadLongFromArray(),
			Opcode.FALoad => new InsnLoadFloatFromArray(),
			Opcode.DALoad => new InsnLoadDoubleFromArray(),
			Opcode.AALoad => new InsnLoadRefFromArray(),
			Opcode.BALoad => new InsnLoadByteFromArray(),
			Opcode.CALoad => new InsnLoadCharFromArray(),
			Opcode.SALoad => new InsnLoadShortFromArray(),
			#endregion

			#region Locals (Store)
			Opcode.IStore => Stream.ReadByte(out byte StoreSlot) ? new InsnStoreInteger(StoreSlot) : null,
			Opcode.LStore => Stream.ReadByte(out byte StoreSlot) ? new InsnStoreLong(StoreSlot) : null,
			Opcode.FStore => Stream.ReadByte(out byte StoreSlot) ? new InsnStoreFloat(StoreSlot) : null,
			Opcode.DStore => Stream.ReadByte(out byte StoreSlot) ? new InsnStoreDouble(StoreSlot) : null,
			Opcode.AStore => Stream.ReadByte(out byte StoreSlot) ? new InsnStoreRef(StoreSlot) : null,

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

			Opcode.AStore_0 => new InsnStoreRef(0),
			Opcode.AStore_1 => new InsnStoreRef(1),
			Opcode.AStore_2 => new InsnStoreRef(2),
			Opcode.AStore_3 => new InsnStoreRef(3),
			#endregion

			#region Arrays (Store) | INCOMPLETE
			// Opcode.IAStore => ,
			// Opcode.LAStore => ,
			// Opcode.FAStore => ,
			// Opcode.DAStore => ,
			// Opcode.AAStore => ,
			// Opcode.BAStore => ,
			// Opcode.CAStore => ,
			// Opcode.SAStore => ,
			#endregion

			#region Stack Manipulation
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

			// Opcode.IInc => new Insn()

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
			// Opcode.LCmp => new Insn(),
			// Opcode.FCmpL => new Insn(),
			// Opcode.FCmpG => new Insn(),
			// Opcode.DCmpL => new Insn(),
			// Opcode.DCmpG => new Insn(),
			#endregion

			#region Control Flow
			Opcode.If_0 => Stream.ReadShort(out short Offset) ? new InsnZeroBranch(BranchCondition.Equal, Address, Offset) : null,
			Opcode.If_Not0 => Stream.ReadShort(out short Offset) ? new InsnZeroBranch(BranchCondition.NotEqual, Address, Offset) : null,
			Opcode.If_LessThan0 => Stream.ReadShort(out short Offset) ? new InsnZeroBranch(BranchCondition.LessThan, Address, Offset) : null,
			Opcode.If_GreaterEqual0 => Stream.ReadShort(out short Offset) ? new InsnZeroBranch(BranchCondition.GreaterEqual, Address, Offset) : null,
			Opcode.If_GreaterThan0 => Stream.ReadShort(out short Offset) ? new InsnZeroBranch(BranchCondition.GreatherThan, Address, Offset) : null,
			Opcode.If_LessEqual0 => Stream.ReadShort(out short Offset) ? new InsnZeroBranch(BranchCondition.LessEqual, Address, Offset) : null,
			Opcode.If_ICmpEqual => Stream.ReadShort(out short Offset) ? new InsnIntegerBranch(BranchCondition.Equal, Address, Offset) : null,
			Opcode.If_ICmpNotEqual => Stream.ReadShort(out short Offset) ? new InsnIntegerBranch(BranchCondition.NotEqual, Address, Offset) : null,
			Opcode.If_ICmpLessThan => Stream.ReadShort(out short Offset) ? new InsnIntegerBranch(BranchCondition.LessThan, Address, Offset) : null,
			Opcode.If_ICmpGreaterEqual => Stream.ReadShort(out short Offset) ? new InsnIntegerBranch(BranchCondition.GreaterEqual, Address, Offset) : null,
			Opcode.If_ICmpGreaterThan => Stream.ReadShort(out short Offset) ? new InsnIntegerBranch(BranchCondition.GreatherThan, Address, Offset) : null,
			Opcode.If_ICmpLessEqual => Stream.ReadShort(out short Offset) ? new InsnIntegerBranch(BranchCondition.LessEqual, Address, Offset) : null,
			Opcode.If_ACmpEqual => Stream.ReadShort(out short Offset) ? new InsnReferenceBranch(ReferenceBranchCondition.Equal, Address, Offset) : null,
			Opcode.If_ACmpNotEqual => Stream.ReadShort(out short Offset) ? new InsnReferenceBranch(ReferenceBranchCondition.NotEqual, Address, Offset) : null,

			Opcode.Goto => Stream.ReadShort(out short Offset) ? new InsnGoto(Address, Offset) : null,
			Opcode.JumpSubr => throw new NotSupportedException("The 'jsr' instruction is not supported."),
			Opcode.RetSubr => throw new NotSupportedException("The 'ret' instruction is not supported."),
			// TableSwitch,
			// LookupSwitch,

			Opcode.IReturn => new InsnReturnInteger(),
			Opcode.LReturn => new InsnReturnLong(),
			Opcode.FReturn => new InsnReturnFloat(),
			Opcode.DReturn => new InsnReturnDouble(),
			Opcode.AReturn => new InsnReturnRef(),
			Opcode.Return => new InsnReturn(),
			#endregion

			Opcode.GetStatic => ReadFieldRef(Stream, Class.ConstantPool, out (string ClassName, FieldDescriptor Field)? Meta) ? new InsnGetStatic(Meta!.Value.ClassName, Meta!.Value.Field) : null,
			Opcode.PutStatic => ReadFieldRef(Stream, Class.ConstantPool, out (string ClassName, FieldDescriptor Field)? Meta) ? new InsnPutStatic(Meta!.Value.ClassName, Meta!.Value.Field) : null,

			Opcode.GetField => ReadFieldRef(Stream, Class.ConstantPool, out (string ClassName, FieldDescriptor Field)? Meta) ? new InsnGetField(Meta!.Value.ClassName, Meta!.Value.Field) : null,
			Opcode.PutField => ReadFieldRef(Stream, Class.ConstantPool, out (string ClassName, FieldDescriptor Field)? Meta) ? new InsnPutField(Meta!.Value.ClassName, Meta!.Value.Field) : null,

			#region Invokes
			Opcode.InvokeVirtual => ReadMethodRef(Stream, Class.ConstantPool, out (string ClassName, MethodDescriptor Method, bool Interface)? Meta) ? new InsnInvokeVirtual(Meta!.Value.ClassName, Meta.Value.Method, Meta.Value.Interface) : null,
			Opcode.InvokeSpecial => ReadMethodRef(Stream, Class.ConstantPool, out (string ClassName, MethodDescriptor Method, bool Interface)? Meta) ? new InsnInvokeSpecial(Meta!.Value.ClassName, Meta.Value.Method, Meta.Value.Interface) : null,
			Opcode.InvokeStatic => ReadMethodRef(Stream, Class.ConstantPool, out (string ClassName, MethodDescriptor Method, bool Interface)? Meta) ? new InsnInvokeStatic(Meta!.Value.ClassName, Meta.Value.Method, Meta.Value.Interface) : null,
			// Opcode.InvokeInterface => ,
			Opcode.InvokeDynamic => ReadInvokeDynamic(Stream, Class),
			#endregion

			Opcode.New => ReadClassName(Stream, Class.ConstantPool, out string? CName) ? new InsnCheckCast(CName!) : null,
			// NewArray
			// ANewArray
			Opcode.ArrayLength => new InsnArrayLength(),

			Opcode.AThrow => new InsnThrow(),
			Opcode.CheckCast => ReadClassName(Stream, Class.ConstantPool, out string? CName) ? new InsnCheckCast(CName!) : null,

			// Jesus

			Opcode.Goto_W => Stream.ReadInt(out int Offset) ? new InsnGoto(Address, Offset) : null,

			// Some other sht

			_ => throw new InvalidDataException($"Unknown opcode reached: {(Opcode) Op}") // DEBUGTRACE: In the future, this will just return null.
		};
	}

	private static Instruction ReadInvokeDynamic(Stream Stream, InternalClass Class) {
		ConstantInvokeDynamic Cindy = Class.ConstantPool.Value<ConstantInvokeDynamic>(Stream.ReadUShort());

		ConstantNameAndType CalleeNAT = Cindy.GetNameAndType(Class.ConstantPool);
		MethodDescriptor Callee = MethodDescriptor.FromSignature(Class.ConstantPool, CalleeNAT);

		Internals.BootstrapMethod? InternalBootstrap = Cindy.GetBootstrapMethod(Class);

		if (InternalBootstrap == null) {
			// TODO: Pop all arguments, this is already possible by querying the Callee args, but nothing supports returning arrays of instructions
			// TODO: Update to the above, just use InsnMalformedIndy
			Console.WriteLine("Captured an INDY in a class without BootstrapMethods."); // DEBUGTRACE

			return new InsnPushNull(); // TODO: Proper typing for values returned
		}

		return new InsnInvokeDynamic(BootstrapMethod.FromInternal(Class, InternalBootstrap), Callee);
	}

	private static bool ReadClassName(Stream Stream, ConstantPool Pool, out string? Value) {
		Value = null;
		if (!Stream.ReadUShort(out ushort Index))
			return false;
		Value = Pool.Value<ConstantClass>(Index).GetName(Pool);
		return true;
	}

	// DESIGN: Consider a less ugly way to do this, PLEASE PLEASE PLEASE:
	private static bool ReadMethodRef(Stream Stream, ConstantPool Pool, out (string ClassName, MethodDescriptor Method, bool Interface)? MethodRef) {
		MethodRef = null;
		if (!Stream.ReadUShort(out ushort PoolIndex))
			return false;
		ConstantInfo Ci = Pool[PoolIndex];
		if (Ci is ConstantMethodRef ConNorm) {
			MethodRef = (ConNorm.GetClassName(Pool), MethodDescriptor.FromSignature(Pool, ConNorm.GetNameAndType(Pool)), false);
		} else if (Ci is ConstantInterfaceMethodRef ConInt) {
			MethodRef = (ConInt.GetClassName(Pool), MethodDescriptor.FromSignature(Pool, ConInt.GetNameAndType(Pool)), true);
		} else
			return false;
		return true;
	}
	private static bool ReadFieldRef(Stream Stream, ConstantPool Pool, out (string ClassName, FieldDescriptor Method)? FieldRef) {
		FieldRef = null;
		if (!Stream.ReadUShort(out ushort PoolIndex) || Pool[PoolIndex] is not ConstantFieldRef ConFld)
			return false;
		FieldRef = (ConFld.GetClassName(Pool), FieldDescriptor.FromSignature(Pool, ConFld.GetNameAndType(Pool)));
		return true;
	}
	private static Instruction? ReadLdc(ushort Index, ConstantPool Pool, bool Wide) {
		ConstantInfo Ci = Pool[Index];
		if (Ci.IsWide && !Wide)
			return null; // That is not valid.
		else if (Ci is ConstantDynamic Cdyn) {
			// I HAVE NO IDEA WHAT TO DO HERE!
			// Make sure it never has the type L or J if not Wide. That's illegal, apparently.
			throw new InvalidDataException($"Support for {Ci.Tag} as an LDC argument is not implemented at the moment.");
		} else if (Ci is ConstantClass Cc) {
			return new InsnPushClass(Cc.GetName(Pool));
		} else if (Ci is ConstantMethodHandle Cm) {
			// No idea
			throw new InvalidDataException($"Support for {Ci.Tag} as an LDC argument is not implemented at the moment.");
		} else if (Ci is ConstantMethodType Cmt) {
			// No idea
			throw new InvalidDataException($"Support for {Ci.Tag} as an LDC argument is not implemented at the moment.");
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
			throw new InvalidDataException($"Found LDC with an unacceptable constant: {Ci}"); // DEBUGTRACE
			return null;
		}
		return null; // Can remove later
	}
}