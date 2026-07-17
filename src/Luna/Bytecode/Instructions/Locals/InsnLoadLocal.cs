using Alluseri.Luna.Analysis;
using System;

namespace Alluseri.Luna.Bytecode;

public class InsnLoadLocal : AbstractLocalsInstruction {
	public readonly ValueKind Kind;

	public InsnLoadLocal(ValueKind Kind, ushort Slot) : base(Slot) {
		Kind.Validate();

		this.Kind = Kind;
	}

	protected override Opcode SmallOpcode => (Opcode) ((uint) Opcode.ILoad_0 + ((uint) Kind * 4));
	protected override Opcode LargeOpcode => (Opcode) ((uint) Opcode.ILoad + (uint) Kind);

	public override string ToString() => $"load.{Kind.GetInstructionSign()} {Slot}";
}
