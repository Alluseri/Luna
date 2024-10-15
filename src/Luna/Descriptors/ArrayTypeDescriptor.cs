using System;
using System.Runtime;

namespace Alluseri.Luna;

public class ArrayTypeDescriptor : TypeDescriptor {
	public ArrayTypeDescriptor(TypeDescriptor Type, byte Depth) : base($"{new string('[', Depth)}{Type}") { } // Wow... this hurts me

	internal static new ArrayTypeDescriptor? Parse(ReadOnlySpan<char> Value, ref int Offset) {
		if (Value[0] != '[')
			return null;
		int Depth = 1;
		while (Value[Depth] == '[')
			Depth++;
		if (Depth > 0xFF)
			throw new FormatException($"The array type descriptor '{Value}' is not valid: the depth is {Depth}.");
		byte TDepth = (byte) Depth;
		TypeDescriptor? K = TypeDescriptor.Parse(Value, ref Depth);
		Offset += Depth;
		return new(K, TDepth);
	}

	internal static new ArrayTypeDescriptor? Parse(ReadOnlySpan<char> Value) {
		if (Value[0] != '[')
			return null;
		byte Depth = 1;
		checked {
			while (Value[Depth] == '[')
				Depth++;
		}
		return new(TypeDescriptor.Parse(Value[Depth..]), Depth);
	}
}