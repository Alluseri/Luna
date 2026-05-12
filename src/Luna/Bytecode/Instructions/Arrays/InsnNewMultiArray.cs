using Alluseri.Luna.Utils;
using System;
using System.IO;

namespace Alluseri.Luna.Bytecode;

public class InsnNewMultiArray : Instruction {
	public ReferenceTypeDescriptor TypeDescriptor;
	public string TypeName {
		get => TypeDescriptor.SymbolicTerm;
		set => TypeDescriptor = ReferenceTypeDescriptor.ParseSymbolic(value);
	}
	public byte Dimensions;

	public InsnNewMultiArray(ReferenceTypeDescriptor TypeDescriptor, byte Dimensions) : base(4) {
		if (Dimensions == 0)
			throw new ArgumentOutOfRangeException(nameof(Dimensions));

		this.TypeDescriptor = TypeDescriptor;
		this.Dimensions = Dimensions;
	}
	public InsnNewMultiArray(string TypeName, byte Dimensions) : base(4) {
		if (Dimensions == 0)
			throw new ArgumentOutOfRangeException(nameof(Dimensions));

		this.TypeDescriptor = ReferenceTypeDescriptor.ParseSymbolic(TypeName);
		this.Dimensions = Dimensions;
	}

	internal override void Write(Stream Stream, CodeBuilder Builder, int Address) {
		Stream.Write(Opcode.ANewArray);
		Stream.Write(TypeDescriptor.CheckoutSymbolic(Builder.Pool));
		Stream.Write(Dimensions);
	}

	public override string ToString() => $"newmultiarray.a {TypeName} {Dimensions}";
}