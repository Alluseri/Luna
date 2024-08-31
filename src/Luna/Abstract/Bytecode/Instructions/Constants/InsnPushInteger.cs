using Alluseri.Luna.Internals;
using Alluseri.Luna.Utils;
using System;
using System.IO;

namespace Alluseri.Luna.Abstract.Bytecode;

public class InsnPushInteger : Instruction {
	public int Value;

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

	public override void Write(Stream Stream, InternalClass Class) {
		checked { // DEBUGTRACE: This shall be removed in production
			if (Value >= -1 && Value < 6) {
				Stream.Write(Opcode.IConst_0, Value);
			} else if (Value >= sbyte.MinValue && Value <= sbyte.MaxValue) {
				Stream.Write(Opcode.BiPush);
				Stream.Write((sbyte) Value);
			} else if (Value >= short.MinValue && Value <= short.MaxValue) {
				Stream.Write(Opcode.SiPush);
				Stream.Write((short) Value);
			} else {
				Ldc(Stream, Class.ConstantPool.Checkout(new ConstantInteger(Value)));
			}
		}
	}

	public override string ToString() => $"push.i {Value}";
}