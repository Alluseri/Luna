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

	public override void Write(Stream Stream, InternalClass Class) {
		if (Value >= -1 && Value < 6) {
			Stream.Write(Opcode.IConst_0, Value);
		} else {
			Ldc(Stream, Class.ConstantPool.Checkout(new ConstantInteger(Value)));
		}
	}

	public override string ToString() => $"push {Value} I"; // DESIGN: The space is kinda weird provided some others don't have it.
	public override int GetHashCode() => HashCode.Combine(nameof(InsnPushInteger));
	public override bool Equals(object? Other) => Other is InsnPushInteger I && I.Value == Value;
}