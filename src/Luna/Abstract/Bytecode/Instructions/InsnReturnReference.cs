using Alluseri.Luna.Internals;
using System;
using System.IO;

namespace Alluseri.Luna.Abstract.Bytecode;

public class InsnReturnReference : Instruction { // DESIGN: Or ReturnRef is a better name cuz shorter?
	public override void Write(Stream Stream, InternalClass Class) {
		Stream.WriteByte((byte) Opcode.AReturn);
	}

	public override string ToString() => $"return.a";
	public override int GetHashCode() => HashCode.Combine(nameof(InsnReturnReference));
	public override bool Equals(object? Other) => Other is InsnReturnReference;
}