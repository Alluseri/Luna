using Alluseri.Luna.Utils;
using System;
using System.IO;

namespace Alluseri.Luna.Bytecode;

public class InsnCompareFloat : Instruction {
	WhenNaN WhenNaN;

	public InsnCompareFloat(WhenNaN WhenNaN) : base(1) {
		if (WhenNaN > WhenNaN.Greater)
			throw new ArgumentOutOfRangeException(nameof(WhenNaN), "Illegal operation substitute for NaN provided.");

		this.WhenNaN = WhenNaN;
	}

	internal override void Write(Stream Stream, CodeBuilder Builder) {
		Stream.Write(Opcode.FCmpL, (uint) WhenNaN);
	}

	public override string ToString() => $"fcmp{WhenNaN.GetInstructionSign()}";
}