namespace Alluseri.Luna.Bytecode;

public class InsnLoadReference : AbstractLocalsInstruction {
	public InsnLoadReference(ushort Slot) : base(Slot) { }

	protected override Opcode SmallOpcode => Opcode.ALoad_0;
	protected override Opcode LargeOpcode => Opcode.ALoad;

	public override string ToString() => $"load.a {Slot}";
}