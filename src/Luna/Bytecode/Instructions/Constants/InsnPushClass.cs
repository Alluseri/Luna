using Alluseri.Luna.Internals;
using System.IO;

namespace Alluseri.Luna.Bytecode;

public class InsnPushClass : Instruction {
	public ReferenceTypeDescriptor Descriptor;
	public string ClassName {
		get => Descriptor.SymbolicTerm;
		set => Descriptor = ReferenceTypeDescriptor.ParseSymbolic(value);
	}
	private ushort PoolIndex;

	public InsnPushClass(string Descriptor) {
		this.Descriptor = ReferenceTypeDescriptor.ParseSymbolic(Descriptor);
	}
	public InsnPushClass(ReferenceTypeDescriptor Descriptor) {
		this.Descriptor = Descriptor;
	}

	internal override void Checkout(ConstantPool Pool) {
		Size = GetLdcSize(PoolIndex = Descriptor.CheckoutSymbolic(Pool));
	}

	internal override void Write(Stream Stream, CodeBuilder Builder) {
		Ldc(Stream, PoolIndex);
	}

	public override string ToString() => $"push.class {ClassName}";
}