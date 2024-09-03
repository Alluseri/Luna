using Alluseri.Luna.Internals;
using Alluseri.Luna.Utils;
using System;
using System.IO;

namespace Alluseri.Luna.Abstract.Bytecode;

public class InsnShiftLeft : Instruction {
	BitwiseOperand Operand;

	public InsnShiftLeft(BitwiseOperand Operand) : base(1) {
		if (Operand > BitwiseOperand.Long)
			throw new ArgumentOutOfRangeException("Illegal arithmetic operand for this operation.");

		this.Operand = Operand;
	}

	internal override void Write(Stream Stream, InternalClass Class) {
		Stream.Write(Opcode.IShl, (uint) Operand);
	}

	public override string ToString() => $"shl.{Operand.GetInstructionSign()}";
}