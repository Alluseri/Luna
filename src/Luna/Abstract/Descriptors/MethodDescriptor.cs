using Alluseri.Luna.Internals;
using Alluseri.Luna.Utils;
using System;
using System.Collections.Generic;

namespace Alluseri.Luna.Abstract;

public class MethodDescriptor : Descriptor {
	public TypeDescriptor ReturnType;
	public string Name;
	public CompoundTypeDescriptor Arguments;

	public override string Term => $"({Arguments}){ReturnType}";
	public string FullDescriptor => $"{Name}({Arguments}){ReturnType}"; // TODO: Reconsider the name after Term is renamed to something more proper

	public MethodDescriptor(TypeDescriptor ReturnType, string Name, CompoundTypeDescriptor Arguments) { // Java-style
		this.ReturnType = ReturnType;
		this.Name = Name;
		this.Arguments = Arguments;
	}

	public MethodDescriptor(string Name, CompoundTypeDescriptor Arguments, TypeDescriptor ReturnType) : this(ReturnType, Name, Arguments) { } // LL-style

	public ushort Checkout(ConstantPool Pool) => Pool.Checkout(new ConstantNameAndType(
		Pool.CheckoutUtf8(Name),
		Pool.CheckoutUtf8(Term)
	));

	public static MethodDescriptor FromSignature(string Name, string Signature) {
		ReadOnlySpan<char> R = Signature;
		if (R[0] != '(')
			throw new FormatException($"The type descriptor '{R}' is not valid.");
		List<TypeDescriptor> Td = new();
		int Offset = 1;
		while (R[Offset] != ')')
			Td.Add(TypeDescriptor.Parse(R, ref Offset));
		CompoundTypeDescriptor Arguments = new(Td);
		TypeDescriptor ReturnType = TypeDescriptor.Parse(R[(Offset + 1)..]);
		return new(Name, Arguments, ReturnType);
	}
	public static MethodDescriptor FromSignature(ConstantPool Pool, ConstantNameAndType Signature) {
		string Name = new(Signature.GetName(Pool));
		ReadOnlySpan<char> R = Signature.GetDescriptor(Pool);
		if (R[0] != '(')
			throw new FormatException($"The type descriptor '{R}' is not valid.");
		List<TypeDescriptor> Td = new();
		int Offset = 1;
		while (R[Offset] != ')')
			Td.Add(TypeDescriptor.Parse(R, ref Offset));
		CompoundTypeDescriptor Arguments = new(Td);
		TypeDescriptor ReturnType = TypeDescriptor.Parse(R[(Offset + 1)..]);
		return new(Name, Arguments, ReturnType);
	}
}