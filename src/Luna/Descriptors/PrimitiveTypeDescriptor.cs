using Alluseri.Luna.Internals;
using System;

namespace Alluseri.Luna;

public class PrimitiveTypeDescriptor : TypeDescriptor {
	public readonly PrimitiveType Type;

	public PrimitiveTypeDescriptor(PrimitiveType Type) : base(Type switch {
		PrimitiveType.Byte => "B",
		PrimitiveType.Char => "C",
		PrimitiveType.Double => "D",
		PrimitiveType.Float => "F",
		PrimitiveType.Integer => "I",
		PrimitiveType.Long => "J",
		PrimitiveType.Short => "S",
		PrimitiveType.Boolean => "Z",
		PrimitiveType.Void => "V",
		_ => throw new ArgumentException($"{Type} is not a supported primitive type!")
	}) {
		this.Type = Type;
	}

	public static PrimitiveTypeDescriptor? ForCharacter(char C) {
		return C switch {
			'B' => new(PrimitiveType.Byte),
			'C' => new(PrimitiveType.Char),
			'D' => new(PrimitiveType.Double),
			'F' => new(PrimitiveType.Float),
			'I' => new(PrimitiveType.Integer),
			'J' => new(PrimitiveType.Long),
			'S' => new(PrimitiveType.Short),
			'Z' => new(PrimitiveType.Boolean),
			'V' => new(PrimitiveType.Void),
			_ => null
		};
	}

	public static implicit operator PrimitiveTypeDescriptor(PrimitiveType Type) => new(Type);
}

public static class PrimitiveTypeExtensions {
	// TODO: Better names for all this shit
	// TODO: This doesn't look clean at all. Something is wrong with those methods.
	// Who even needs IsTheSameAs? Wtf was I drinking when making this
	public static bool IsTheSameAs(this PrimitiveType Type, Descriptor Descriptor) => Descriptor is PrimitiveTypeDescriptor PTD && PTD.Type == Type;
	public static VerificationType ToVerificationType(this PrimitiveType Type) => Type switch {
		PrimitiveType.Byte => new IntegerVerificationType(),
		PrimitiveType.Char => new IntegerVerificationType(),
		PrimitiveType.Double => new DoubleVerificationType(),
		PrimitiveType.Float => new FloatVerificationType(),
		PrimitiveType.Integer => new IntegerVerificationType(),
		PrimitiveType.Long => new LongVerificationType(),
		PrimitiveType.Short => new IntegerVerificationType(),
		PrimitiveType.Boolean => new IntegerVerificationType(),
		_ => new TopVerificationType() // TODO: Or should we throw?
	};
}

public enum PrimitiveType {
	Byte, Char, Double, Float, Integer, Long, Short, Boolean, Void
}