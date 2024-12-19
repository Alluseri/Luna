using Alluseri.Luna.Internals;
using Alluseri.Luna.Utils;

namespace Alluseri.Luna;

public class MethodReference : ClassMemberReference<MethodDescriptor> {
	public MethodReference(string ClassName, MethodDescriptor Descriptor) : base(ClassName, Descriptor) { }
	public MethodReference(ReferenceTypeDescriptor ClassDescriptor, MethodDescriptor Descriptor) : base(ClassDescriptor, Descriptor) { }

	public override ushort Checkout(ConstantPool Pool) => Pool.Checkout(
		new ConstantMethodRef(
			ClassDescriptor.CheckoutSymbolic(Pool),
			Descriptor.Checkout(Pool)
		)
	);
}