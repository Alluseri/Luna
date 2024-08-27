using Alluseri.Luna.Internals;
using System;
using System.IO;

namespace Alluseri.Luna.Abstract.Bytecode;

public class InsnPushNull : Instruction {
	public override void Write(Stream Stream, InternalClass Class) {
		Stream.WriteByte((byte) Opcode.AConst_Null);
	}

	public override string ToString() => $"push null";
	public override int GetHashCode() => HashCode.Combine(nameof(InsnPushNull));
	public override bool Equals(object? Other) => Other is InsnPushNull;
}