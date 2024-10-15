namespace Alluseri.Luna.Bytecode;

public enum ArithmeticOperand : uint {
	Integer, Long, Float, Double
}

public enum PrimitiveCastResult : uint {
	Integer, Long, Float, Double, Byte, Char, Short
}

public enum BitwiseOperand : uint {
	Integer, Long
}

internal static class ArithmeticOperandExtensions {
	public static char GetInstructionSign(this ArithmeticOperand Op) => Op switch {
		ArithmeticOperand.Integer => 'i',
		ArithmeticOperand.Long => 'l',
		ArithmeticOperand.Float => 'f',
		ArithmeticOperand.Double => 'd',
		_ => '?'
	};
	public static char GetInstructionSign(this PrimitiveCastResult Op) => Op switch {
		PrimitiveCastResult.Integer => 'i',
		PrimitiveCastResult.Long => 'l',
		PrimitiveCastResult.Float => 'f',
		PrimitiveCastResult.Double => 'd',
		PrimitiveCastResult.Byte => 'b',
		PrimitiveCastResult.Char => 'c',
		PrimitiveCastResult.Short => 's',
		_ => '?'
	};
	public static char GetInstructionSign(this BitwiseOperand Op) => Op switch {
		BitwiseOperand.Integer => 'i',
		BitwiseOperand.Long => 'l',
		_ => '?'
	};
}