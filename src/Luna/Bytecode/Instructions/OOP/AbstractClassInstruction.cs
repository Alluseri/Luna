using Alluseri.Luna.Utils;
using System.IO;

namespace Alluseri.Luna.Bytecode;

public abstract class AbstractClassInstruction : Instruction {
	public ReferenceTypeDescriptor ClassDescriptor;
	public string ClassName {
		get => ClassDescriptor.SymbolicTerm;
		set => ClassDescriptor = ReferenceTypeDescriptor.ParseSymbolic(value);
	}

	protected abstract Opcode Opcode { get; }

	public AbstractClassInstruction(string ClassName) : base(3) {
		this.ClassDescriptor = ReferenceTypeDescriptor.ParseSymbolic(ClassName);
	}
	public AbstractClassInstruction(ReferenceTypeDescriptor ClassDescriptor) : base(3) {
		this.ClassDescriptor = ClassDescriptor;
	}

	internal override void Write(Stream Stream, CodeBuilder Builder) {
		Stream.Write(Opcode);
		Stream.Write(ClassDescriptor.CheckoutSymbolic(Builder.Pool));
	}
}