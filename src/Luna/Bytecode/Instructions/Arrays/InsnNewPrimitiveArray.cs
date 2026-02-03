using Alluseri.Luna.Utils;
using System;
using System.IO;

namespace Alluseri.Luna.Bytecode;

public class InsnNewPrimitiveArray : Instruction {
	public readonly PrimitiveArrayType ArrayType;

	public InsnNewPrimitiveArray(PrimitiveArrayType ArrayType) : base(2) {
		this.ArrayType = ArrayType;
		if (ArrayType < PrimitiveArrayType.Boolean || ArrayType > PrimitiveArrayType.Long) // TODO: Do we actually need this? Try to make illegal shit and check if it passes verifier
			throw new ArgumentOutOfRangeException(nameof(ArrayType), "Illegal primitive array type for this operation.");
	}

	internal override void Write(Stream Stream, CodeBuilder Class, int Address) {
		Stream.Write(Opcode.NewArray);
		Stream.Write((byte) ArrayType);
	}

	public override string ToString() => $"newarray.{ArrayType.GetInstructionSign()}";
}