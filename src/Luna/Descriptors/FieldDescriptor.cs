using Alluseri.Luna.Internals;
using Alluseri.Luna.Utils;
using System;
using System.Collections.Generic;

namespace Alluseri.Luna;

public class FieldDescriptor : Descriptor {
	public TypeDescriptor FieldType;
	public string Name;

	public override string Term => FieldType.Term;
	public string FullDescriptor => $"{FieldType} {Name}"; // TODO: Reconsider the name after Term is renamed to something more proper

	public FieldDescriptor(TypeDescriptor FieldType, string Name) { // Java-style
		this.FieldType = FieldType;
		this.Name = Name;
	}

	public FieldDescriptor(string Name, TypeDescriptor FieldType) : this(FieldType, Name) { } // Recaf-style

	public ushort CheckoutNameAndType(ConstantPool Pool) => Pool.Checkout(new ConstantNameAndType(
		Pool.CheckoutUtf8(Name),
		Pool.CheckoutUtf8(Term)
	));
	public ushort CheckoutFieldRef(ConstantPool Pool, string ClassName) => Pool.Checkout(new ConstantFieldRef(
		Pool.Checkout(new ConstantClass(Pool.CheckoutUtf8(ClassName))),
		Pool.Checkout(new ConstantNameAndType(
			Pool.CheckoutUtf8(Name),
			Pool.CheckoutUtf8(Term)
		))
	));

	public static FieldDescriptor FromSignature(string Name, string Signature)
	=> new(Name, TypeDescriptor.Parse(Signature));

	public static FieldDescriptor FromSignature(ConstantPool Pool, ConstantNameAndType Signature)
	=> new(new(Signature.GetName(Pool)), TypeDescriptor.Parse(Signature.GetDescriptor(Pool)));
}