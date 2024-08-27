using Alluseri.Luna.Internals;
using System;
using System.IO;

namespace Alluseri.Luna.Abstract.Bytecode;

public class InsnReturn : Instruction {
	public override void Write(Stream Stream, InternalClass Class) {
		Stream.WriteByte((byte) Opcode.Return);
	}

	public override string ToString() => $"return";
	public override int GetHashCode() => HashCode.Combine(nameof(InsnReturn));
	public override bool Equals(object? Other) => Other is InsnReturn;
}