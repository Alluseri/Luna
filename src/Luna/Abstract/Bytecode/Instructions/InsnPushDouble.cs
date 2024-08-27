using Alluseri.Luna.Internals;
using System;
using System.IO;

namespace Alluseri.Luna.Abstract.Bytecode;

public class InsnPushDouble : Instruction {
	public double Value;

	public InsnPushDouble(double Value) {
		this.Value = Value;
	}

	public override void Write(Stream Stream, InternalClass Class) {
		switch (Value) {
			case 0:
			Stream.WriteByte((byte) Opcode.DConst_0);
			break;
			case 1:
			Stream.WriteByte((byte) Opcode.DConst_1);
			break;
			default:
			Ldc(Stream, Class.ConstantPool.Checkout(new ConstantDouble(Value)), true);
			break;
		}
	}

	public override string ToString() => $"push {Value}D";
	public override int GetHashCode() => HashCode.Combine(nameof(InsnPushDouble));
	public override bool Equals(object? Other) => Other is InsnPushDouble I && I.Value == Value;
}