using Alluseri.Luna.Internals;
using System;
using System.IO;

namespace Alluseri.Luna.Abstract.Bytecode;

public class InsnReturnFloat : Instruction {
	public override void Write(Stream Stream, InternalClass Class) {
		Stream.WriteByte((byte) Opcode.FReturn);
	}

	public override string ToString() => $"return.f";
	public override int GetHashCode() => HashCode.Combine(nameof(InsnReturnFloat));
	public override bool Equals(object? Other) => Other is InsnReturnFloat;
}