namespace Alluseri.Luna.Bytecode;

public class InsnInstanceOf : AbstractClassInstruction {
	protected override Opcode Opcode => Opcode.InstanceOf;

	public InsnInstanceOf(string ClassName) : base(ClassName) { }
	public InsnInstanceOf(ReferenceTypeDescriptor ClassDescriptor) : base(ClassDescriptor) { }

	public override string ToString() => $"instanceof {ClassName}";
}