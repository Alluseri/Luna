using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

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

public enum WhenNaN : uint {
	Lesser, Greater
}

// TODO: Methinks we can just get rid of the Message argument entirely since it's always the same thing
// TODO: "Arithmetic"OperandExtensions? Are we sure about that?
internal static class ArithmeticOperandExtensions {
	[StackTraceHidden]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void Validate(this ArithmeticOperand Operand, string Message = "Illegal arithmetic operand for this operation.")
	=> Guard.ThrowIfGreaterThan(Operand, ArithmeticOperand.Double, Message);

	[StackTraceHidden]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void Validate(this BitwiseOperand Operand, string Message = "Illegal bitwise operand for this operation.")
	=> Guard.ThrowIfGreaterThan(Operand, BitwiseOperand.Long, Message);

	[StackTraceHidden]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void Validate(this WhenNaN When, string Message = "Illegal operation substitute for NaN provided.")
	=> Guard.ThrowIfGreaterThan(When, WhenNaN.Greater, Message);

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

	public static char GetInstructionSign(this WhenNaN Op) => Op switch {
		WhenNaN.Greater => 'g',
		WhenNaN.Lesser => 'l',
		_ => '?'
	};
}