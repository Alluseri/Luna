using Alluseri.Luna.Utils;
using System;
using System.IO;

namespace Alluseri.Luna.Bytecode;

public class InsnShiftRight : Instruction {
	public readonly BitwiseOperand Operand;

	public InsnShiftRight(BitwiseOperand Operand) : base(1) {
		Operand.Validate();

		this.Operand = Operand;
	}

	internal override void Write(Stream Stream, CodeBuilder Builder, int Address) {
		Stream.Write(Opcode.IShr, (uint) Operand);
	}

	public override string ToString() => $"shr.{Operand.GetInstructionSign()}";
}