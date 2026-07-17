using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Alluseri.Luna.Bytecode;

// Don't forget to change strings for refactor

public enum ValueKind : uint {
	Integer, Long, Float, Double, Reference
}

internal static class ValueKindExtensions {
	[StackTraceHidden]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void Validate(this ValueKind Type, string Message = "Illegal value kind for this operation.")
	=> Guard.ThrowIfGreaterThan(Type, ValueKind.Reference, Message);

	public static char GetInstructionSign(this ValueKind Kind) => Kind switch {
		ValueKind.Integer => 'i',
		ValueKind.Long => 'l',
		ValueKind.Float => 'f',
		ValueKind.Double => 'd',
		ValueKind.Reference => 'a',
		_ => '?'
	};
}