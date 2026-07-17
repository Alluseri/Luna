using Alluseri.Luna.Internals;
using Alluseri.Luna.Utils;
using System;
using System.IO;

namespace Alluseri.Luna.Bytecode;

public class InsnRemainder : Instruction {
	public readonly ArithmeticOperand Operand;

	public InsnRemainder(ArithmeticOperand Operand) : base(1) {
		Operand.Validate();

		this.Operand = Operand;
	}

	internal override void Write(Stream Stream, CodeBuilder Builder, int Address) {
		Stream.Write(Opcode.IRem, (uint) Operand);
	}

	public override string ToString() => $"remainder.{Operand.GetInstructionSign()}";
}