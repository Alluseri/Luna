using Alluseri.Luna.Internals;
using Alluseri.Luna.Utils;
using System;
using System.IO;

namespace Alluseri.Luna.Bytecode;

public class InsnInvokeVirtual : AbstractInsnInvoke {
	public InsnInvokeVirtual(ReferenceTypeDescriptor ClassDescriptor, MethodDescriptor Method, bool Interface = false) : base(ClassDescriptor, Method, Interface) { }
	public InsnInvokeVirtual(string ClassName, MethodDescriptor Method, bool Interface = false) : base(ClassName, Method, Interface) { }
	public InsnInvokeVirtual(CodeReader.ManagedMethodReference Mref) : base(Mref) { }

	protected override Opcode Opcode => Opcode.InvokeVirtual;
	protected override string Instruction => "invokevirtual";
}