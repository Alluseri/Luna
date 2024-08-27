using Alluseri.Luna.Internals;
using Alluseri.Luna.Utils;
using System;
using System.IO;

namespace Alluseri.Luna.Abstract.Bytecode;

public class InsnDup2 : Instruction {
	public override void Write(Stream Stream, InternalClass Class) {
		Stream.WriteByte((byte) Opcode.Dup2);
	}

	public override string ToString() => "dup2";
	public override int GetHashCode() => HashCode.Combine(nameof(InsnDup2));
	public override bool Equals(object? Other) => Other is InsnDup2;
}

public class InsnDup2_X1 : Instruction {
	public override void Write(Stream Stream, InternalClass Class) {
		Stream.WriteByte((byte) Opcode.Dup2_X1);
	}

	public override string ToString() => "dup2_x1";
	public override int GetHashCode() => HashCode.Combine(nameof(InsnDup2_X1));
	public override bool Equals(object? Other) => Other is InsnDup2_X1;
}

public class InsnDup2_X2 : Instruction {
	public override void Write(Stream Stream, InternalClass Class) {
		Stream.WriteByte((byte) Opcode.Dup2_X2);
	}

	public override string ToString() => "dup2_x2";
	public override int GetHashCode() => HashCode.Combine(nameof(InsnDup2_X2));
	public override bool Equals(object? Other) => Other is InsnDup2_X2;
}