using Alluseri.Luna.Internals;
using Alluseri.Luna.Utils;
using System;
using System.IO;

namespace Alluseri.Luna.Abstract.Bytecode;

public class InsnAnd : Instruction {
	BitwiseOperand Operand;

	public InsnAnd(BitwiseOperand Operand) : base(1) {
		if (Operand > BitwiseOperand.Long)
			throw new ArgumentOutOfRangeException("Illegal arithmetic operand for this operation.");

		this.Operand = Operand;
	}

	internal override void Write(Stream Stream, InternalClass Class) {
		Stream.Write(Opcode.IAnd, (uint) Operand);
	}

	public override string ToString() => $"and.{Operand.GetInstructionSign()}";
}