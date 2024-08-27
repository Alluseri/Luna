using Alluseri.Luna.Internals;
using System;
using System.IO;

namespace Alluseri.Luna.Abstract.Bytecode;

public class InsnPop2 : Instruction {
	public override void Write(Stream Stream, InternalClass Class) {
		Stream.WriteByte((byte) Opcode.Pop2);
	}

	public override string ToString() => "pop";
	public override int GetHashCode() => HashCode.Combine(nameof(InsnPop2));
	public override bool Equals(object? Other) => Other is InsnPop2;
}