using Alluseri.Luna.Utils;
using System;
using System.IO;

namespace Alluseri.Luna.Bytecode;

public class InsnPrimitiveCast : Instruction {
	// This one is explicitly readonly which doesn't match the rest because of additional checks needing to be done at runtime.
	// I'm not lazy, it just makes the code really ugly.
	// I might look into changing this at one point.

	public readonly ArithmeticOperand From;
	public readonly PrimitiveCastResult To;
	private readonly Opcode Opcode;

	public InsnPrimitiveCast(ArithmeticOperand From, PrimitiveCastResult To) : base(1) {
		Opcode = GetOpcode(this.From = From, this.To = To) ?? throw new ArgumentException($"There is no opcode that can cast from {From} to {To}. Try ${nameof(MacroPrimitiveCast)} instead.");
	}

	// This is public for the purpose of self-checking, if necessary(writers should opt into using MacroPrimitiveCast instead):
	public static Opcode? GetOpcode(ArithmeticOperand From, PrimitiveCastResult To) {
		if ((uint) From == (uint) To || (From != ArithmeticOperand.Integer && To >= PrimitiveCastResult.Byte))
			return null;
		return From switch {
			ArithmeticOperand.Integer => To switch {
				PrimitiveCastResult.Long => Opcode.I2L,
				PrimitiveCastResult.Float => Opcode.I2F,
				PrimitiveCastResult.Double => Opcode.I2D,
				PrimitiveCastResult.Byte => Opcode.I2B,
				PrimitiveCastResult.Char => Opcode.I2C,
				PrimitiveCastResult.Short => Opcode.I2S,
				_ => null
			},
			ArithmeticOperand.Long => To switch {
				PrimitiveCastResult.Integer => Opcode.L2I,
				PrimitiveCastResult.Float => Opcode.L2F,
				PrimitiveCastResult.Double => Opcode.L2D,
				_ => null
			},
			ArithmeticOperand.Double => To switch {
				PrimitiveCastResult.Integer => Opcode.D2I,
				PrimitiveCastResult.Float => Opcode.D2F,
				PrimitiveCastResult.Long => Opcode.D2L,
				_ => null
			},
			ArithmeticOperand.Float => To switch {
				PrimitiveCastResult.Integer => Opcode.F2I,
				PrimitiveCastResult.Double => Opcode.F2D,
				PrimitiveCastResult.Long => Opcode.F2L,
				_ => null
			},
			_ => null
		};
	}

	internal override void Write(Stream Stream, CodeBuilder Builder) {
		Stream.Write(Opcode);
	}

	public override string ToString() => $"{From.GetInstructionSign()}2{To.GetInstructionSign()}";
}