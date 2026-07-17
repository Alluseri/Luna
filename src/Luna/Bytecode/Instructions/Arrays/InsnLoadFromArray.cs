using Alluseri.Luna.Utils;
using System;
using System.IO;

namespace Alluseri.Luna.Bytecode;

public class InsnLoadFromArray : Instruction {
	public readonly ArrayType ArrayType;

	public InsnLoadFromArray(ArrayType ArrayType) : base(1) {
		ArrayType.Validate();

		this.ArrayType = ArrayType;
	}

	internal override void Write(Stream Stream, CodeBuilder Builder, int Address) {
		Stream.Write(Opcode.IALoad, (uint) ArrayType);
	}

	public override string ToString() => $"loadfromarray.{ArrayType.GetInstructionSign()}";
}