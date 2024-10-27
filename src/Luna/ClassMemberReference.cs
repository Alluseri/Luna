using Alluseri.Luna.Internals;
using System.IO;

namespace Alluseri.Luna;

public abstract class ClassMemberReference {
	public ReferenceTypeDescriptor ClassDescriptor;
	public string ClassName {
		get => ClassDescriptor.SymbolicTerm;
		set => ClassDescriptor = ReferenceTypeDescriptor.ParseSymbolic(value);
	}

	internal ClassMemberReference(ReferenceTypeDescriptor ClassDescriptor) {
		this.ClassDescriptor = ClassDescriptor;
	}

	public abstract ushort Checkout(ConstantPool Pool);

	public static ClassMemberReference FromConstant(ConstantPool Pool, ConstantInfo Info) => Info switch { // holy boilerplate
		ConstantMethodRef CMethod => new MethodReference(CMethod.GetClassName(Pool), MethodDescriptor.FromSignature(Pool, CMethod.GetNameAndType(Pool))),
		ConstantInterfaceMethodRef CIMethod => new InterfaceMethodReference(CIMethod.GetClassName(Pool), MethodDescriptor.FromSignature(Pool, CIMethod.GetNameAndType(Pool))),
		ConstantFieldRef CField => new FieldReference(CField.GetClassName(Pool), FieldDescriptor.FromSignature(Pool, CField.GetNameAndType(Pool))),
		_ => throw new InvalidDataException($"Cannot represent {Info} as a managed class member reference.")
	};
}

public abstract class ClassMemberReference<D>(ReferenceTypeDescriptor ClassDescriptor, D Descriptor) : ClassMemberReference(ClassDescriptor) where D : Descriptor {
	public D Descriptor = Descriptor;

	public ClassMemberReference(string ClassName, D Descriptor) : this(ReferenceTypeDescriptor.ParseSymbolic(ClassName), Descriptor) { }

	public override string ToString() => $"{ClassDescriptor}.{Descriptor}";
}