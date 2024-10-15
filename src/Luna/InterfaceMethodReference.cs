using Alluseri.Luna.Internals;
using Alluseri.Luna.Utils;

namespace Alluseri.Luna;

public class InterfaceMethodReference : ClassMemberReference<MethodDescriptor> {
	public InterfaceMethodReference(string ClassName, MethodDescriptor Descriptor) : base(ClassName, Descriptor) { }

	public override ushort Checkout(ConstantPool Pool) => Pool.Checkout(
		new ConstantInterfaceMethodRef(
			Pool.Checkout(new ConstantClass(Pool.CheckoutUtf8(ClassName))),
			Descriptor.Checkout(Pool)
		)
	);
}