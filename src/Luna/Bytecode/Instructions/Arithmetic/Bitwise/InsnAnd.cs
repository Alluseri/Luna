using Alluseri.Luna.Utils;
using System;
using System.IO;

namespace Alluseri.Luna.Bytecode;

public class InsnAnd : Instruction {
	public readonly BitwiseOperand Operand;

	public InsnAnd(BitwiseOperand Operand) : base(1) {
		Operand.Validate();

		this.Operand = Operand;
	}

	internal override void Write(Stream Stream, CodeBuilder Builder, int Address) {
		Stream.Write(Opcode.IAnd, (uint) Operand);
	}

	public override string ToString() => $"and.{Operand.GetInstructionSign()}";
}