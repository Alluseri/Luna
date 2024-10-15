using System;

namespace Alluseri.Luna;

public class ObjectTypeDescriptor : TypeDescriptor {
	public ObjectTypeDescriptor(string ObjectType) : base($"L{ObjectType};") { } // Requires / separators 

	internal static new ObjectTypeDescriptor? Parse(ReadOnlySpan<char> Value, ref int Offset) {
		if (Value[0] != 'L')
			return null;
		int LocalOffset = Value.IndexOf(';');
		if (LocalOffset == -1)
			throw new FormatException($"The type descriptor '{Value}' is not valid.");
		Offset += LocalOffset + 1;
		return new(new(Value[1..LocalOffset]));
	}

	internal static new ObjectTypeDescriptor? Parse(ReadOnlySpan<char> Value) {
		if (Value[0] != 'L')
			return null;
		int LocalOffset = Value.IndexOf(';');
		if (LocalOffset == -1)
			throw new FormatException($"The type descriptor '{Value}' is not valid.");
		return new(new(Value[1..LocalOffset]));
	}
}