using Alluseri.Luna.Internals;
using Alluseri.Luna.Utils;
using System;
using System.IO;

namespace Alluseri.Luna.Abstract.Bytecode;

public class InsnDup : Instruction {
	public override void Write(Stream Stream, InternalClass Class) {
		Stream.WriteByte((byte) Opcode.Dup);
	}

	public override string ToString() => "dup";
	public override int GetHashCode() => HashCode.Combine(nameof(InsnDup));
	public override bool Equals(object? Other) => Other is InsnDup;
}

public class InsnDup_X1 : Instruction {
	public override void Write(Stream Stream, InternalClass Class) {
		Stream.WriteByte((byte) Opcode.Dup_X1);
	}

	public override string ToString() => "dup_x1";
	public override int GetHashCode() => HashCode.Combine(nameof(InsnDup_X1));
	public override bool Equals(object? Other) => Other is InsnDup_X1;
}

public class InsnDup_X2 : Instruction {
	public override void Write(Stream Stream, InternalClass Class) {
		Stream.WriteByte((byte) Opcode.Dup_X2);
	}

	public override string ToString() => "dup_x2";
	public override int GetHashCode() => HashCode.Combine(nameof(InsnDup_X2));
	public override bool Equals(object? Other) => Other is InsnDup_X2;
}