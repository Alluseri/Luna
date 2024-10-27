namespace Alluseri.Luna.Bytecode;

public class InsnNew : AbstractClassInstruction {
	protected override Opcode Opcode => Opcode.New;

	public InsnNew(string ClassName) : base(ClassName) { }
	public InsnNew(ReferenceTypeDescriptor ClassDescriptor) : base(ClassDescriptor) { }

	public override string ToString() => $"new {ClassName}";
}