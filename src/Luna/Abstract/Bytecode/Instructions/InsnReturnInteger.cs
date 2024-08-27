using Alluseri.Luna.Internals;
using System;
using System.IO;

namespace Alluseri.Luna.Abstract.Bytecode;

public class InsnReturnInteger : Instruction {
	public override void Write(Stream Stream, InternalClass Class) {
		Stream.WriteByte((byte) Opcode.IReturn);
	}

	public override string ToString() => $"return.i";
	public override int GetHashCode() => HashCode.Combine(nameof(InsnReturnInteger));
	public override bool Equals(object? Other) => Other is InsnReturnInteger;
}