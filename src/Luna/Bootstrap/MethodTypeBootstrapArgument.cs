using Alluseri.Luna.Internals;
using Alluseri.Luna.Utils;

namespace Alluseri.Luna;

public class MethodTypeBootstrapArgument : BootstrapArgument {
	public MethodTypeDescriptor Descriptor;

	public MethodTypeBootstrapArgument(MethodTypeDescriptor Descriptor) {
		this.Descriptor = Descriptor;
	}

	public override ushort Checkout(ConstantPool Pool) => Pool.Checkout(new ConstantMethodType(Pool.CheckoutUtf8(Descriptor.Term)));
}