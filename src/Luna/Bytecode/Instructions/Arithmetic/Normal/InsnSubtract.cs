using Alluseri.Luna.Internals;
using Alluseri.Luna.Utils;
using System;
using System.IO;

namespace Alluseri.Luna.Bytecode;

public class InsnSubtract : Instruction {
	public readonly ArithmeticOperand Operand;

	public InsnSubtract(ArithmeticOperand Operand) : base(1) {
		Operand.Validate();

		this.Operand = Operand;
	}

	internal override void Write(Stream Stream, CodeBuilder Builder, int Address) {
		Stream.Write(Opcode.ISub, (uint) Operand);
	}

	public override string ToString() => $"sub.{Operand.GetInstructionSign()}";
}