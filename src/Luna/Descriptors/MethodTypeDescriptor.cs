using Alluseri.Luna.Internals;
using Alluseri.Luna.Utils;
using System;
using System.Collections.Generic;

namespace Alluseri.Luna;

public class MethodTypeDescriptor : Descriptor {
	public TypeDescriptor ReturnType;
	public CompoundTypeDescriptor Arguments;

	public override string Term => $"({Arguments}){ReturnType}";

	public MethodTypeDescriptor(TypeDescriptor ReturnType, CompoundTypeDescriptor Arguments) { // Java-style
		this.ReturnType = ReturnType;
		this.Arguments = Arguments;
	}

	public MethodTypeDescriptor(CompoundTypeDescriptor Arguments, TypeDescriptor ReturnType) : this(ReturnType, Arguments) { } // LL-style

	public ushort Checkout(ConstantPool Pool) => Pool.Checkout(new ConstantMethodType(Pool.CheckoutUtf8(Term)));

	public static MethodTypeDescriptor FromSignature(string Signature) {
		ReadOnlySpan<char> R = Signature;
		if (R[0] != '(')
			throw new FormatException($"The type descriptor '{R}' is not valid.");
		List<TypeDescriptor> Td = new();
		int Offset = 1;
		while (R[Offset] != ')')
			Td.Add(TypeDescriptor.Parse(R, ref Offset));
		CompoundTypeDescriptor Arguments = new(Td);
		TypeDescriptor ReturnType = TypeDescriptor.Parse(R[(Offset + 1)..]);
		return new(Arguments, ReturnType);
	}
	public static MethodTypeDescriptor FromSignature(ConstantPool Pool, ConstantMethodType Signature) => FromSignature(Signature.GetDescriptor(Pool));
}