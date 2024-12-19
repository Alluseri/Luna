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
internal static class PrimitiveArrayTypeExtensions {
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