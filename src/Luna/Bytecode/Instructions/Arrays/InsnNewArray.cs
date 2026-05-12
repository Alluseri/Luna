using Alluseri.Luna.Utils;
using System.IO;

namespace Alluseri.Luna.Bytecode;

public class InsnNewArray : Instruction {
	public ReferenceTypeDescriptor TypeDescriptor;
	public string TypeName {
		get => TypeDescriptor.SymbolicTerm;
		set => TypeDescriptor = ReferenceTypeDescriptor.ParseSymbolic(value);
	}

	public InsnNewArray(ReferenceTypeDescriptor TypeDescriptor) : base(3) {
		this.TypeDescriptor = TypeDescriptor;
	}
	public InsnNewArray(string TypeName) : this(ReferenceTypeDescriptor.ParseSymbolic(TypeName)) { }

	internal override void Write(Stream Stream, CodeBuilder Builder, int Address) {
		Stream.Write(Opcode.ANewArray);
		Stream.Write(TypeDescriptor.CheckoutSymbolic(Builder.Pool));
	}

	public override string ToString() => $"newarray.a {TypeName}";
}