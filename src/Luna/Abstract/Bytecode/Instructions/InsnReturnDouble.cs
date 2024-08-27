using Alluseri.Luna.Internals;
using System;
using System.IO;

namespace Alluseri.Luna.Abstract.Bytecode;

public class InsnReturnDouble : Instruction {
	public override void Write(Stream Stream, InternalClass Class) {
		Stream.WriteByte((byte) Opcode.DReturn);
	}

	public override string ToString() => $"return.d";
	public override int GetHashCode() => HashCode.Combine(nameof(InsnReturnDouble));
	public override bool Equals(object? Other) => Other is InsnReturnDouble;
}