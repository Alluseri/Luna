using Alluseri.Luna.Internals;
using System.IO;

namespace Alluseri.Luna;

public abstract class ClassMemberReference(string ClassName) {
	public string ClassName = ClassName;

	public abstract ushort Checkout(ConstantPool Pool);

	public static ClassMemberReference FromConstant(ConstantPool Pool, ConstantInfo Info) => Info switch { // holy boilerplate
		ConstantMethodRef CMethod => new MethodReference(CMethod.GetClassName(Pool), MethodDescriptor.FromSignature(Pool, CMethod.GetNameAndType(Pool))),
		ConstantInterfaceMethodRef CIMethod => new InterfaceMethodReference(CIMethod.GetClassName(Pool), MethodDescriptor.FromSignature(Pool, CIMethod.GetNameAndType(Pool))),
		ConstantFieldRef CField => new FieldReference(CField.GetClassName(Pool), FieldDescriptor.FromSignature(Pool, CField.GetNameAndType(Pool))),
		_ => throw new InvalidDataException($"Cannot represent {Info} as a managed class member reference.")
	};

	public override string ToString() => $"{}";
}

public abstract class ClassMemberReference<D>(string ClassName, D Descriptor) : ClassMemberReference(ClassName) where D : Descriptor {
	public D Descriptor = Descriptor;
}