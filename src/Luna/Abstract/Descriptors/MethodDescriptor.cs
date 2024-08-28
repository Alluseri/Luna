using Alluseri.Luna.Internals;
using System;
using System.Collections.Generic;

namespace Alluseri.Luna.Abstract;

// DESIGN: One or more of these constructors should not make it into prod. I think.

public class MethodDescriptor : Descriptor {
	public TypeDescriptor ReturnType;
	public string Name;
	public CompoundTypeDescriptor Arguments;

	public override string Term => $"{Name}({Arguments}){ReturnType}";
	public string JvmType => $"({Arguments}){ReturnType}"; // TODO: This definitely needs a better name.

	public MethodDescriptor(TypeDescriptor ReturnType, string Name, CompoundTypeDescriptor Arguments) { // Java-style
		this.ReturnType = ReturnType;
		this.Name = Name;
		this.Arguments = Arguments;
	}

	public MethodDescriptor(string Name, CompoundTypeDescriptor Arguments, TypeDescriptor ReturnType) : this(ReturnType, Name, Arguments) { } // LL-style

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