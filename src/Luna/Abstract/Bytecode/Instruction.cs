using Alluseri.Luna.Internals;
using Alluseri.Luna.Utils;
using System;
using System.IO;

namespace Alluseri.Luna.Abstract.Bytecode;

// TODO: IMPORTANT: Read below.
// DESIGN: IMPORTANT: I think readonly InternalClass has to go. Which is honestly fine. Consider adding bootstrap method attributes, for example. We'll worry about it later.

// DESIGN: "Original opcode" form

public abstract class Instruction : Linkable<Instruction> {
	public Instruction() {

	}

	protected Stream Ldc(Stream Stream, ushort PoolIndex, bool Wide = false) {
		if (Wide || PoolIndex > byte.MaxValue) {
			Stream.WriteByte((byte) (Wide ? Opcode.Ldc2_W : Opcode.Ldc_W));
			Stream.Write(PoolIndex);
		} else {
			Stream.WriteByte((byte) Opcode.Ldc);
			Stream.Write((byte) PoolIndex);
		}
		return Stream;
	}

	public abstract override bool Equals(object? Other);
	public abstract override int GetHashCode();
	public abstract override string ToString();

	public abstract void Write(Stream Stream, InternalClass Class);
	public virtual bool CanConsume(StackFrame Frame) => true; // TODO: Better name please

	public static Instruction? Read(Stream Stream, InternalClass Class) {
		if (!Stream.ReadByte(out byte Op))
			return null;
		// Luckily, the rules here aren't as tight as they are with Attributes. There's no such thing as "overreading" due to stupid size values,
		// therefore we can just refer to constructors for argless instructions.
		return (Opcode) Op switch {
			Opcode.Nop => new InsnNop(),

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

			// Opcode.BiPush => ,
			// Opcode.SiPush => ,

			Opcode.Ldc => Stream.ReadByte(out byte LdcIndex) ? ReadLdc(LdcIndex, Class.ConstantPool, false) : null,
			Opcode.Ldc_W => Stream.ReadUShort(out ushort LdcIndex) ? ReadLdc(LdcIndex, Class.ConstantPool, false) : null,
			Opcode.Ldc2_W => Stream.ReadUShort(out ushort LdcIndex) ? ReadLdc(LdcIndex, Class.ConstantPool, true) : null,

			Opcode.ILoad => Stream.ReadByte(out byte LoadSlot) ? new InsnLoadInteger(LoadSlot) : null,
			Opcode.LLoad => Stream.ReadByte(out byte LoadSlot) ? new InsnLoadLong(LoadSlot) : null,
			Opcode.FLoad => Stream.ReadByte(out byte LoadSlot) ? new InsnLoadFloat(LoadSlot) : null,
			Opcode.DLoad => Stream.ReadByte(out byte LoadSlot) ? new InsnLoadDouble(LoadSlot) : null,
			Opcode.ALoad => Stream.ReadByte(out byte LoadSlot) ? new InsnLoadReference(LoadSlot) : null,

			Opcode.ILoad_0 => new InsnLoadInteger(0),
			Opcode.ILoad_1 => new InsnLoadInteger(0),
			Opcode.ILoad_2 => new InsnLoadInteger(0),
			Opcode.ILoad_3 => new InsnLoadInteger(0),

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

			// Opcode.IALoad => ,
			// Opcode.LALoad => ,
			// Opcode.FALoad => ,
			// Opcode.DALoad => ,
			// Opcode.AALoad => ,
			// Opcode.BALoad => ,
			// Opcode.CALoad => ,
			// Opcode.SALoad => ,

			Opcode.IStore => Stream.ReadByte(out byte StoreSlot) ? new InsnStoreInteger(StoreSlot) : null,
			Opcode.LStore => Stream.ReadByte(out byte StoreSlot) ? new InsnStoreLong(StoreSlot) : null,
			Opcode.FStore => Stream.ReadByte(out byte StoreSlot) ? new InsnStoreFloat(StoreSlot) : null,
			Opcode.DStore => Stream.ReadByte(out byte StoreSlot) ? new InsnStoreDouble(StoreSlot) : null,
			Opcode.AStore => Stream.ReadByte(out byte StoreSlot) ? new InsnStoreReference(StoreSlot) : null,

			Opcode.IStore_0 => new InsnStoreInteger(0),
			Opcode.IStore_1 => new InsnStoreInteger(0),
			Opcode.IStore_2 => new InsnStoreInteger(0),
			Opcode.IStore_3 => new InsnStoreInteger(0),

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

			// Some more dumb shit here

			Opcode.Pop => new InsnPop(),
			Opcode.Pop2 => new InsnPop2(),
			Opcode.Dup => new InsnDup(),
			Opcode.Dup_X1 => new InsnDup_X1(),
			Opcode.Dup_X2 => new InsnDup_X2(),
			Opcode.Dup2 => new InsnDup2(),
			Opcode.Dup2_X1 => new InsnDup2_X1(),
			Opcode.Dup2_X2 => new InsnDup2_X2(),
			Opcode.Swap => new InsnSwap(),

			// A shit ton more of opcodes

			Opcode.IReturn => new InsnReturnInteger(),
			Opcode.LReturn => new InsnReturnLong(),
			Opcode.FReturn => new InsnReturnFloat(),
			Opcode.DReturn => new InsnReturnDouble(),
			Opcode.AReturn => new InsnReturnReference(),
			Opcode.Return => new InsnReturn(),

			// And just a few more
			_ => throw new InvalidDataException($"Unknown opcode reached: {(Opcode) Op}") // DEBUGTRACE: In the future, this will just return null.
		};
	}
	private static Instruction? ReadLdc(ushort Index, ConstantPool Pool, bool Wide) {
		ConstantInfo Ci = Pool[Index];
		if (Ci.IsWide && !Wide)
			return null; // That is not valid.
		else if (Ci is ConstantDynamic Cdyn) {
			// I HAVE NO IDEA WHAT TO DO HERE!
			// Make sure it never has the type L or J if not Wide. That's illegal, apparently.
		} else if (Ci is ConstantClass Cc) {
			// No idea
		} else if (Ci is ConstantMethodHandle Cm) {
			// No idea
		} else if (Ci is ConstantMethodType Cmt) {
			// No idea
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
			Console.WriteLine($"Found LDC with an unacceptable constant: {Ci}"); // DEBUGTRACE
			return null;
		}
		return null; // Can remove later
	}
}