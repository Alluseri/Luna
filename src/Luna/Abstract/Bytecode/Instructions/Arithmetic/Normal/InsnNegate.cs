using Alluseri.Luna.Internals;
using Alluseri.Luna.Utils;
using System;
using System.IO;

namespace Alluseri.Luna.Abstract.Bytecode;

public class InsnNegate : Instruction {
	public override int Size => 1;

	ArithmeticOperand Operand;

	public InsnNegate(ArithmeticOperand Operand) {
		if (Operand > ArithmeticOperand.Double)
			throw new ArgumentOutOfRangeException("Illegal arithmetic operand for this operation.");

		this.Operand = Operand;
	}

	public override void Write(Stream Stream, InternalClass Class) {
		Stream.Write(Opcode.INeg, (uint) Operand);
	}

	public override string ToString() => $"neg.{Operand.GetInstructionSign()}";
}