using Alluseri.Luna.Internals;
using System;
using System.IO;

namespace Alluseri.Luna.Abstract.Bytecode;

public class InsnPop : Instruction {
	public override void Write(Stream Stream, InternalClass Class) {
		Stream.WriteByte((byte) Opcode.Pop);
	}

	public override string ToString() => "pop";
	public override int GetHashCode() => HashCode.Combine(nameof(InsnPop));
	public override bool Equals(object? Other) => Other is InsnPop;
}