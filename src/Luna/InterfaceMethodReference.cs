using Alluseri.Luna.Internals;
using Alluseri.Luna.Utils;

namespace Alluseri.Luna;

public class InterfaceMethodReference : ClassMemberReference<MethodDescriptor> {
	public InterfaceMethodReference(string ClassName, MethodDescriptor Descriptor) : base(ClassName, Descriptor) { }
	public InterfaceMethodReference(ReferenceTypeDescriptor ClassDescriptor, MethodDescriptor Descriptor) : base(ClassDescriptor, Descriptor) { }

	public override ushort Checkout(ConstantPool Pool) => Pool.Checkout(
		new ConstantInterfaceMethodRef(
			ClassDescriptor.CheckoutSymbolic(Pool),
			Descriptor.Checkout(Pool)
		)
	);
}