using Alluseri.Luna.Internals;
using Alluseri.Luna.Utils;
using System;
using System.IO;

namespace Alluseri.Luna.Abstract.Bytecode;

public class InsnAnd : Instruction {
	public override int Size => 1;

	BitwiseOperand Operand;

	public InsnAnd(BitwiseOperand Operand) {
		if (Operand > BitwiseOperand.Long)
			throw new ArgumentOutOfRangeException("Illegal arithmetic operand for this operation.");

		this.Operand = Operand;
	}

	public override void Write(Stream Stream, InternalClass Class) {
		Stream.Write(Opcode.IAnd, (uint) Operand);
	}

	public override string ToString() => $"and.{Operand.GetInstructionSign()}";
}