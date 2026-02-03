using Alluseri.Luna.Utils;
using System;
using System.IO;

namespace Alluseri.Luna.Bytecode;

public class InsnStoreInArray : Instruction {
	public readonly ArrayType ArrayType;

	public InsnStoreInArray(ArrayType ArrayType) : base(1) {
		if (ArrayType > ArrayType.Short)
			throw new ArgumentOutOfRangeException(nameof(ArrayType), "Illegal array type for this operation.");

		this.ArrayType = ArrayType;
	}

	internal override void Write(Stream Stream, CodeBuilder Builder, int Address) {
		Stream.Write(Opcode.IAStore, (uint) ArrayType);
	}

	public override string ToString() => $"storeinarray.{ArrayType.GetInstructionSign()}";
}