namespace Alluseri.Luna.Bytecode;

public class InsnStoreReference : AbstractLocalsInstruction {
	public InsnStoreReference(ushort Slot) : base(Slot) { }

	protected override Opcode SmallOpcode => Opcode.AStore_0;
	protected override Opcode LargeOpcode => Opcode.AStore;

	public override string ToString() => $"store.a {Slot}";
}