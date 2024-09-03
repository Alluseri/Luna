using Alluseri.Luna.Internals;
using Alluseri.Luna.Utils;
using System;
using System.IO;

namespace Alluseri.Luna.Abstract.Bytecode;

public class InsnPushInteger : Instruction {
	public int Value;

	private ushort PoolIndex;

	public InsnPushInteger(int Value) {
		this.Value = Value;
	}
	public InsnPushInteger(uint Value) {
		this.Value = (int) Value;
	}
	public InsnPushInteger(short Value) {
		this.Value = Value;
	}
	public InsnPushInteger(ushort Value) {
		this.Value = Value;
	}
	public InsnPushInteger(byte Value) {
		this.Value = Value;
	}
	public InsnPushInteger(sbyte Value) {
		this.Value = Value;
	}

	internal override void Checkout(ConstantPool Pool) {
		PoolIndex = 0;
		Size = Value switch {
			>= -1 and < 6 => 1,
			>= sbyte.MinValue and <= sbyte.MaxValue => 2,
			>= short.MinValue and <= short.MaxValue => 3,
			_ => GetLdcSize(PoolIndex = Pool.Checkout(new ConstantInteger(Value)))
		};
	}

	internal override void Write(Stream Stream, InternalClass Class) {
		checked { // DEBUGTRACE: This shall be removed in production
			if (PoolIndex == 0) {
				switch (Size) {
					case 1:
					Stream.Write(Opcode.IConst_0, Value);
					break;
					case 2:
					Stream.Write(Opcode.BiPush);
					Stream.Write((sbyte) Value);
					break;
					case 3:
					Stream.Write(Opcode.SiPush);
					Stream.Write((short) Value);
					break;
				}
			} else
				Ldc(Stream, PoolIndex);
		}
	}

	public override string ToString() => $"push.i {Value}";
}