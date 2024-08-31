namespace Alluseri.Luna.Abstract.Bytecode;

public enum ArithmeticOperand : uint {
	Integer, Long, Float, Double
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
	public static char GetInstructionSign(this BitwiseOperand Op) => Op switch {
		BitwiseOperand.Integer => 'i',
		BitwiseOperand.Long => 'l',
		_ => '?'
	};
}