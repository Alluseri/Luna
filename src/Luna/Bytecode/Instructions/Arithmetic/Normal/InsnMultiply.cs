using Alluseri.Luna.Internals;
using Alluseri.Luna.Utils;
using System;
using System.IO;

namespace Alluseri.Luna.Bytecode;

public class InsnMultiply : Instruction {
	public readonly ArithmeticOperand Operand;

	public InsnMultiply(ArithmeticOperand Operand) : base(1) {
		Operand.Validate();

		this.Operand = Operand;
	}

	internal override void Write(Stream Stream, CodeBuilder Builder, int Address) {
		Stream.Write(Opcode.IMul, (uint) Operand);
	}

	public override string ToString() => $"mul.{Operand.GetInstructionSign()}";
}