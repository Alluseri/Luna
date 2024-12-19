using Alluseri.Luna.Internals;
using Alluseri.Luna.Utils;

namespace Alluseri.Luna;

public class FieldReference : ClassMemberReference<FieldDescriptor> {
	public FieldReference(ReferenceTypeDescriptor ClassDescriptor, FieldDescriptor Descriptor) : base(ClassDescriptor, Descriptor) { }
	public FieldReference(string ClassName, FieldDescriptor Descriptor) : base(ClassName, Descriptor) { }

	public override ushort Checkout(ConstantPool Pool) => Descriptor.CheckoutFieldRef(Pool, ClassDescriptor);
}