using Alluseri.Luna.Analysis;
using Alluseri.Luna.Utils;
using System;
using System.Diagnostics;
using System.IO;

namespace Alluseri.Luna.Bytecode;

public class InsnReturn : Instruction {
	public readonly ValueKind? Kind;

	public InsnReturn(ValueKind? Kind = null) : base(1) {
		Kind?.Validate();

		this.Kind = Kind;
	}

	internal override void Write(Stream Stream, CodeBuilder Builder, int Address) {
		if (Kind is ValueKind K)
			Stream.Write(Opcode.IReturn, (uint) K);
		else
			Stream.Write(Opcode.Return);
	}

	public override string ToString() => Kind is ValueKind K ? $"return.{K.GetInstructionSign()}" : "return";
}
