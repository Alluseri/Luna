using Alluseri.Luna.Internals;
using Alluseri.Luna.Utils;
using System;
using System.IO;

namespace Alluseri.Luna.Abstract.Bytecode;

public class InsnShiftLeft : Instruction {
	public override int Size => 1;

	BitwiseOperand Operand;

	public InsnShiftLeft(BitwiseOperand Operand) {
		if (Operand > BitwiseOperand.Long)
			throw new ArgumentOutOfRangeException("Illegal arithmetic operand for this operation.");

		this.Operand = Operand;
	}

	public override void Write(Stream Stream, InternalClass Class) {
		Stream.Write(Opcode.IShl, (uint) Operand);
	}

	public override string ToString() => $"shl.{Operand.GetInstructionSign()}";
}