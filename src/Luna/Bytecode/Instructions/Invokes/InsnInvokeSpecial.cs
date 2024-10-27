namespace Alluseri.Luna.Bytecode;

public class InsnInvokeSpecial : AbstractInsnInvoke {
	public InsnInvokeSpecial(ReferenceTypeDescriptor ClassDescriptor, MethodDescriptor Method, bool Interface = false) : base(ClassDescriptor, Method, Interface) { }
	public InsnInvokeSpecial(string ClassName, MethodDescriptor Method, bool Interface = false) : base(ClassName, Method, Interface) { }
	public InsnInvokeSpecial(CodeReader.ManagedMethodReference Mref) : base(Mref) { }

	protected override Opcode Opcode => Opcode.InvokeSpecial;
	protected override string Instruction => "invokespecial";
}