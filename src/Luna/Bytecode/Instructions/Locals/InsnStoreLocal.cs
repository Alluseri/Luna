using Alluseri.Luna.Analysis;
using System;

namespace Alluseri.Luna.Bytecode;

public class InsnStoreLocal : AbstractLocalsInstruction {
	public readonly ValueKind Kind;

	public InsnStoreLocal(ValueKind Kind, ushort Slot) : base(Slot) {
		Kind.Validate();

		this.Kind = Kind;
	}

	protected override Opcode SmallOpcode => (Opcode) ((uint) Opcode.IStore_0 + ((uint) Kind * 4));
	protected override Opcode LargeOpcode => (Opcode) ((uint) Opcode.IStore + (uint) Kind);

	public override string ToString() => $"store.{Kind.GetInstructionSign()} {Slot}";
}
