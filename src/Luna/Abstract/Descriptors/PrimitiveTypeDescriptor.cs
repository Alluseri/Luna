using System;

namespace Alluseri.Luna.Abstract;

public class PrimitiveTypeDescriptor : TypeDescriptor {
	public readonly PrimitiveType Type;

	public PrimitiveTypeDescriptor(PrimitiveType Type) : base(Type switch {
		PrimitiveType.Byte => "B",
		PrimitiveType.Char => "C",
		PrimitiveType.Double => "D",
		PrimitiveType.Float => "F",
		PrimitiveType.Int => "I",
		PrimitiveType.Long => "J",
		PrimitiveType.Short => "S",
		PrimitiveType.Boolean => "Z",
		PrimitiveType.Void => "V",
		_ => throw new ArgumentException($"{Type} is not a supported primitive type!")
	}) {
		this.Type = Type;
	}
}

public enum PrimitiveType {
	Byte, Char, Double, Float, Int, Long, Short, Boolean, Void
}