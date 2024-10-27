using Alluseri.Luna.Internals;
using Alluseri.Luna.Utils;

namespace Alluseri.Luna;

#pragma warning disable CS8618
public class ClassBootstrapArgument : BootstrapArgument {
	public ReferenceTypeDescriptor Descriptor;
	public string ClassName {
		get => Descriptor.SymbolicTerm;
		set => Descriptor = ReferenceTypeDescriptor.ParseSymbolic(value);
	}

	public ClassBootstrapArgument(string Descriptor) {
		this.Descriptor = ReferenceTypeDescriptor.ParseSymbolic(Descriptor);
	}
	public ClassBootstrapArgument(ReferenceTypeDescriptor Descriptor) {
		this.Descriptor = Descriptor;
	}

	protected override ushort Checkout(ConstantPool Pool) => Descriptor.CheckoutSymbolic(Pool);

	public override string ToString() => $"{{ BArg::Class {Descriptor} }}";
}