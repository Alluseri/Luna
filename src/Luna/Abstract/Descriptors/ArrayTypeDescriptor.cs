using System;

namespace Alluseri.Luna.Abstract;

public class ArrayTypeDescriptor : TypeDescriptor {
	public ArrayTypeDescriptor(TypeDescriptor Type, byte Depth) : base($"{new string('[', Depth)}{Type}") { } // Wow... this hurts me

	public static ArrayTypeDescriptor Parse(string Value) {

		throw new ArgumentException($"{Value} is not a valid array type descriptor!");
	}
}