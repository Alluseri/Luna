using Alluseri.Luna.Utils;
using System;
using System.IO;

namespace Alluseri.Luna.Bytecode;

public class InsnOr : Instruction {
	public readonly BitwiseOperand Operand;

	public InsnOr(BitwiseOperand Operand) : base(1) {
		Operand.Validate();

		this.Operand = Operand;
	}

	internal override void Write(Stream Stream, CodeBuilder Builder, int Address) {
		Stream.Write(Opcode.IOr, (uint) Operand);
	}

	public override string ToString() => $"or.{Operand.GetInstructionSign()}";
}