using Alluseri.Luna.Internals;
using Alluseri.Luna.Utils;

namespace Alluseri.Luna;

public class MethodReference : ClassMemberReference<MethodDescriptor> {
	public MethodReference(string ClassName, MethodDescriptor Descriptor) : base(ClassName, Descriptor) { }

	public override ushort Checkout(ConstantPool Pool) => Pool.Checkout(
		new ConstantMethodRef(
			Pool.Checkout(new ConstantClass(Pool.CheckoutUtf8(ClassName))),
			Descriptor.Checkout(Pool)
		)
	);
}