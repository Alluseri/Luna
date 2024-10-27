namespace Alluseri.Luna.Bytecode;

public class InsnCheckCast : AbstractClassInstruction {
	protected override Opcode Opcode => Opcode.CheckCast;

	public InsnCheckCast(string ClassName) : base(ClassName) { }
	public InsnCheckCast(ReferenceTypeDescriptor ClassDescriptor) : base(ClassDescriptor) { }

	public override string ToString() => $"checkcast {ClassName}";
}