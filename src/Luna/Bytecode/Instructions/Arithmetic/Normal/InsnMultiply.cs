using Alluseri.Luna.Internals;
using Alluseri.Luna.Utils;
using System;
using System.IO;

namespace Alluseri.Luna.Bytecode;

public class InsnMultiply : Instruction {
	ArithmeticOperand Operand;

	public InsnMultiply(ArithmeticOperand Operand) : base(1) {
		if (Operand > ArithmeticOperand.Double)
			throw new ArgumentOutOfRangeException("Illegal arithmetic operand for this operation.");

		this.Operand = Operand;
	}

	internal override void Write(Stream Stream, CodeBuilder Builder) {
		Stream.Write(Opcode.IMul, (uint) Operand);
	}

	public override string ToString() => $"mul.{Operand.GetInstructionSign()}";
}