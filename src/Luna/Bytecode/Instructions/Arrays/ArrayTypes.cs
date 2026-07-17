using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Alluseri.Luna.Bytecode;

public enum PrimitiveArrayType : uint {
	Boolean = 4,
	Char = 5,
	Float = 6,
	Double = 7,
	Byte = 8,
	Short = 9,
	Int = 10,
	Long = 11
}
public enum ArrayType : uint {
	Int,
	Long,
	Float,
	Double,
	Reference,
	ByteBool,
	Char,
	Short
}
// TODO: Could use "[CallerArgumentExpression(nameof(Type))] string? ParamName" here and everywhere else
internal static class PrimitiveArrayTypeExtensions {
	[StackTraceHidden]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void Validate(this ArrayType Type, string Message = "Illegal array type for this operation.")
	=> Guard.ThrowIfGreaterThan(Type, ArrayType.Short, Message);
	[StackTraceHidden]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void Validate(this PrimitiveArrayType PrimitiveType, string Message = "Illegal primitive array type for this operation.")
	=> Guard.ThrowIfOutOfBounds(PrimitiveType, PrimitiveArrayType.Boolean, PrimitiveArrayType.Long, Message);

	public static char GetInstructionSign(this PrimitiveArrayType Op) => Op switch {
		PrimitiveArrayType.Boolean => 'z',
		PrimitiveArrayType.Char => 'c',
		PrimitiveArrayType.Float => 'f',
		PrimitiveArrayType.Double => 'd',
		PrimitiveArrayType.Byte => 'b',
		PrimitiveArrayType.Short => 's',
		PrimitiveArrayType.Int => 'i',
		PrimitiveArrayType.Long => 'l',
		_ => '?'
	};

	public static string GetInstructionSign(this ArrayType Op) => Op switch {

		ArrayType.ByteBool => "bz",
		ArrayType.Char => "c",
		ArrayType.Float => "f",
		ArrayType.Double => "d",
		ArrayType.Reference => "a",
		ArrayType.Short => "s",
		ArrayType.Int => "i",
		ArrayType.Long => "l",
		_ => "?"
	};
}