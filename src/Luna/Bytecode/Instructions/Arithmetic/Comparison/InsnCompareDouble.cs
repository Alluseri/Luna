using Alluseri.Luna.Utils;
using System;
using System.IO;

namespace Alluseri.Luna.Bytecode;

public class InsnCompareDouble : Instruction {
	public readonly WhenNaN WhenNaN;

	public InsnCompareDouble(WhenNaN WhenNaN) : base(1) {
		if (WhenNaN > WhenNaN.Greater)
			throw new ArgumentOutOfRangeException(nameof(WhenNaN), "Illegal operation substitute for NaN provided.");

		this.WhenNaN = WhenNaN;
	}

	internal override void Write(Stream Stream, CodeBuilder Builder, int Address) {
		Stream.Write(Opcode.DCmpL, (uint) WhenNaN);
	}

	public override string ToString() => $"dcmp{WhenNaN.GetInstructionSign()}";
}