using Alluseri.Luna.Utils;
using System;
using System.IO;

namespace Alluseri.Luna.Bytecode;

public class InsnCompareFloat : Instruction {
	public readonly WhenNaN WhenNaN;

	public InsnCompareFloat(WhenNaN WhenNaN) : base(1) {
		WhenNaN.Validate();

		this.WhenNaN = WhenNaN;
	}

	internal override void Write(Stream Stream, CodeBuilder Builder, int Address) {
		Stream.Write(Opcode.FCmpL, (uint) WhenNaN);
	}

	public override string ToString() => $"fcmp{WhenNaN.GetInstructionSign()}";
}