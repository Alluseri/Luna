using Alluseri.Luna.Internals;
using System;
using System.IO;

namespace Alluseri.Luna.Abstract.Bytecode;

public class InsnReturnLong : Instruction {
	public override void Write(Stream Stream, InternalClass Class) {
		Stream.WriteByte((byte) Opcode.LReturn);
	}

	public override string ToString() => $"return.l";
	public override int GetHashCode() => HashCode.Combine(nameof(InsnReturnLong));
	public override bool Equals(object? Other) => Other is InsnReturnLong;
}