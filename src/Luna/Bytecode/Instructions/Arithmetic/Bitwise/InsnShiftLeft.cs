using Alluseri.Luna.Utils;
using System;
using System.IO;

namespace Alluseri.Luna.Bytecode;

public class InsnShiftLeft : Instruction {
	public readonly BitwiseOperand Operand;

	public InsnShiftLeft(BitwiseOperand Operand) : base(1) {
		Operand.Validate();

		this.Operand = Operand;
	}

	internal override void Write(Stream Stream, CodeBuilder Builder, int Address) {
		Stream.Write(Opcode.IShl, (uint) Operand);
	}

	public override string ToString() => $"shl.{Operand.GetInstructionSign()}";
}