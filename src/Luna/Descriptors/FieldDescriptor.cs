using Alluseri.Luna.Internals;
using Alluseri.Luna.Utils;
using System;
using System.Collections.Generic;

namespace Alluseri.Luna;

public class FieldDescriptor : NamedDescriptor {
	public TypeDescriptor FieldType;

	public override string Term => FieldType.Term;
	public override string FullDescriptor => $"{FieldType} {Name}"; // TODO: Reconsider the name after Term is renamed to something more proper

	public FieldDescriptor(TypeDescriptor FieldType, string Name) : base(Name) { // Java-style
		this.FieldType = FieldType;
		this.Name = Name;
	}

	public FieldDescriptor(string Name, TypeDescriptor FieldType) : this(FieldType, Name) { } // Recaf-style

	public ushort CheckoutNameAndType(ConstantPool Pool) => Pool.Checkout(new ConstantNameAndType(
		Pool.CheckoutUTF8(Name),
		Pool.CheckoutUTF8(Term)
	));
	public ushort CheckoutFieldRef(ConstantPool Pool, string ClassName) => Pool.Checkout(new ConstantFieldRef(
		Pool.Checkout(new ConstantClass(Pool.CheckoutUTF8(ClassName))),
		Pool.Checkout(new ConstantNameAndType(
			Pool.CheckoutUTF8(Name),
			Pool.CheckoutUTF8(Term)
		))
	));
	public ushort CheckoutFieldRef(ConstantPool Pool, ReferenceTypeDescriptor ClassDescriptor) => Pool.Checkout(new ConstantFieldRef(
		ClassDescriptor.CheckoutSymbolic(Pool),
		Pool.Checkout(new ConstantNameAndType(
			Pool.CheckoutUTF8(Name),
			Pool.CheckoutUTF8(Term)
		))
	));

	public static FieldDescriptor FromSignature(string Name, string Signature)
	=> new(Name, TypeDescriptor.Parse(Signature));

	public static FieldDescriptor FromSignature(ConstantPool Pool, ConstantNameAndType Signature)
	=> new(new(Signature.GetName(Pool)), TypeDescriptor.Parse(Signature.GetDescriptor(Pool)));
}