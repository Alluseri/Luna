using Alluseri.Luna.Internals;
using Alluseri.Luna.Utils;
using System;
using System.IO;

namespace Alluseri.Luna.Bytecode;

public class InsnRemainder : Instruction {
	ArithmeticOperand Operand;

	public InsnRemainder(ArithmeticOperand Operand) : base(1) {
		if (Operand > ArithmeticOperand.Double)
			throw new ArgumentOutOfRangeException("Illegal arithmetic operand for this operation.");

		this.Operand = Operand;
	}

	internal override void Write(Stream Stream, CodeBuilder Builder) {
		Stream.Write(Opcode.IRem, (uint) Operand);
	}

	public override string ToString() => $"remainder.{Operand.GetInstructionSign()}";
}