using Alluseri.Luna.Internals;
using Alluseri.Luna.Utils;

namespace Alluseri.Luna;

public class MethodTypeBootstrapArgument : BootstrapArgument {
	public MethodTypeDescriptor Descriptor;

	public MethodTypeBootstrapArgument(MethodTypeDescriptor Descriptor) {
		this.Descriptor = Descriptor;
	}

	protected override ushort Checkout(ConstantPool Pool) => Descriptor.Checkout(Pool);

	public override string ToString() => $"{{ BArg::MethodType {Descriptor} }}";
}