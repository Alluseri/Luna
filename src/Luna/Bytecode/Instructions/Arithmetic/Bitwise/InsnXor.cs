using Alluseri.Luna.Utils;
using System;
using System.IO;

namespace Alluseri.Luna.Bytecode;

public class InsnXor : Instruction {
	public readonly BitwiseOperand Operand;

	public InsnXor(BitwiseOperand Operand) : base(1) {
		Operand.Validate();

		this.Operand = Operand;
	}

	internal override void Write(Stream Stream, CodeBuilder Builder, int Address) {
		Stream.Write(Opcode.IXor, (uint) Operand);
	}

	public override string ToString() => $"xor.{Operand.GetInstructionSign()}";
}