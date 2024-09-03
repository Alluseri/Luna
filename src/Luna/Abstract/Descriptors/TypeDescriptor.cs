using System;

namespace Alluseri.Luna.Abstract;

public abstract class TypeDescriptor : ConstantDescriptor {
	public TypeDescriptor(string Term) : base(Term) { }

	// TODO: Make more beautiful
	public static TypeDescriptor Parse(ReadOnlySpan<char> Descriptor, ref int Offset) {
		if (Descriptor.Length == 0)
			throw new FormatException($"The type descriptor to be parsed is empty.");
		PrimitiveTypeDescriptor? Primitive = PrimitiveTypeDescriptor.ForCharacter(Descriptor[Offset]);
		if (Primitive != null) {
			Offset++;
			return Primitive;
		}
		ReadOnlySpan<char> Sliced = Descriptor[Offset..];
		ObjectTypeDescriptor? Obj = ObjectTypeDescriptor.Parse(Sliced, ref Offset);
		if (Obj != null)
			return Obj;
		ArrayTypeDescriptor? Arr = ArrayTypeDescriptor.Parse(Sliced, ref Offset);
		if (Arr != null)
			return Arr;
		throw new FormatException($"The type descriptor '{Descriptor}' is not valid.");
	}

	public static TypeDescriptor Parse(ReadOnlySpan<char> Descriptor) {
		if (Descriptor.Length == 0)
			throw new FormatException($"The type descriptor to be parsed is empty.");
		PrimitiveTypeDescriptor? Primitive = PrimitiveTypeDescriptor.ForCharacter(Descriptor[0]);
		if (Primitive != null) {
			return Primitive;
		}
		ObjectTypeDescriptor? Obj = ObjectTypeDescriptor.Parse(Descriptor);
		if (Obj != null)
			return Obj;
		ArrayTypeDescriptor? Arr = ArrayTypeDescriptor.Parse(Descriptor);
		if (Arr != null)
			return Arr;
		throw new FormatException($"The type descriptor '{Descriptor}' is not valid.");
	}
}