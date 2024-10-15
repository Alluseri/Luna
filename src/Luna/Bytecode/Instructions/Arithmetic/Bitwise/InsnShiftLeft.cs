using Alluseri.Luna.Utils;
using System;
using System.IO;

namespace Alluseri.Luna.Bytecode;

public class InsnShiftLeft : Instruction {
	BitwiseOperand Operand;

	public InsnShiftLeft(BitwiseOperand Operand) : base(1) {
		if (Operand > BitwiseOperand.Long)
			throw new ArgumentOutOfRangeException(nameof(Operand), "Illegal bitwise operand for this operation.");

		this.Operand = Operand;
	}

	internal override void Write(Stream Stream, CodeBuilder Builder) {
		Stream.Write(Opcode.IShl, (uint) Operand);
	}

	public override string ToString() => $"shl.{Operand.GetInstructionSign()}";
}