using System;

namespace Alluseri.Luna;

// TODO: This system is so confusing (blame JVM engineers)

public class ObjectTypeDescriptor : ReferenceTypeDescriptor {
	private readonly string _ObjectType;
	public override string SymbolicTerm => _ObjectType;

	// Requires / separators
	public ObjectTypeDescriptor(string ObjectType) : base($"L{ObjectType};") {
		_ObjectType = ObjectType;
	}

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