using Alluseri.Luna.Utils;
using System;
using System.IO;

namespace Alluseri.Luna.Bytecode;

public class InsnNewPrimitiveArray : Instruction {
	public readonly PrimitiveArrayType ArrayType;

	public InsnNewPrimitiveArray(PrimitiveArrayType ArrayType) : base(2) {
		ArrayType.Validate();

		this.ArrayType = ArrayType;
	}

	internal override void Write(Stream Stream, CodeBuilder Class, int Address) {
		Stream.Write(Opcode.NewArray);
		Stream.Write((byte) ArrayType);
	}

	public override string ToString() => $"newarray.{ArrayType.GetInstructionSign()}";
}