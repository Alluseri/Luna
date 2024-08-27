using Alluseri.Luna.Internals;
using System;
using System.IO;

namespace Alluseri.Luna.Abstract.Bytecode;

public class InsnPushLong : Instruction {
	public long Value;

	public InsnPushLong(long Value) {
		this.Value = Value;
	}
	public InsnPushLong(ulong Value) : this((long) Value) { }

	public override void Write(Stream Stream, InternalClass Class) {
		switch (Value) {
			case 0:
			Stream.WriteByte((byte) Opcode.LConst_0);
			break;
			case 1:
			Stream.WriteByte((byte) Opcode.LConst_1);
			break;
			default:
			Ldc(Stream, Class.ConstantPool.Checkout(new ConstantLong(Value)), true);
			break;
		}
	}

	public override string ToString() => $"ldc {Value}L";
	public override int GetHashCode() => HashCode.Combine(nameof(InsnPushLong));
	public override bool Equals(object? Other) => Other is InsnPushLong I && I.Value == Value;
}