using Alluseri.Luna.Utils;
using System;
using System.IO;

namespace Alluseri.Luna.Bytecode;

public class InsnUnsignedShiftRight : Instruction {
	public readonly BitwiseOperand Operand;

	public InsnUnsignedShiftRight(BitwiseOperand Operand) : base(1) {
		Operand.Validate();

		this.Operand = Operand;
	}

	internal override void Write(Stream Stream, CodeBuilder Builder, int Address) {
		Stream.Write(Opcode.IUShr, (uint) Operand);
	}

	public override string ToString() => $"ushr.{Operand.GetInstructionSign()}";
}