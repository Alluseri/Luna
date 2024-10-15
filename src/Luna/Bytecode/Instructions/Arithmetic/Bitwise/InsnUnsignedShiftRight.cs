using Alluseri.Luna.Utils;
using System;
using System.IO;

namespace Alluseri.Luna.Bytecode;

public class InsnUnsignedShiftRight : Instruction {
	BitwiseOperand Operand;

	public InsnUnsignedShiftRight(BitwiseOperand Operand) : base(1) {
		if (Operand > BitwiseOperand.Long)
			throw new ArgumentOutOfRangeException(nameof(Operand), "Illegal bitwise operand for this operation.");

		this.Operand = Operand;
	}

	internal override void Write(Stream Stream, CodeBuilder Builder) {
		Stream.Write(Opcode.IUShr, (uint) Operand);
	}

	public override string ToString() => $"ushr.{Operand.GetInstructionSign()}";
}