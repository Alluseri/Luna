using Alluseri.Luna.Internals;
using System;
using System.IO;

namespace Alluseri.Luna.Abstract.Bytecode;

public class InsnSwap : Instruction {
	public override void Write(Stream Stream, InternalClass Class) {
		Stream.WriteByte((byte) Opcode.Swap);
	}

	public override string ToString() => $"swap";
	public override int GetHashCode() => HashCode.Combine(nameof(InsnSwap));
	public override bool Equals(object? Other) => Other is InsnSwap;
}