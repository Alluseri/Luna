using Alluseri.Luna.Internals;
using Alluseri.Luna.Utils;
using System;
using System.IO;

namespace Alluseri.Luna.Abstract.Bytecode;

public class InsnNop : Instruction {
	public override void Write(Stream Stream, InternalClass Class) {
		Stream.WriteByte((byte) Opcode.Nop);
	}

	public override string ToString() => "nop";
	public override int GetHashCode() => HashCode.Combine(nameof(InsnNop));
	public override bool Equals(object? Other) => Other is InsnNop;
}