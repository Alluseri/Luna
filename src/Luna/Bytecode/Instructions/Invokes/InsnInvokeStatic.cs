using Alluseri.Luna.Internals;
using Alluseri.Luna.Utils;
using System;
using System.IO;

namespace Alluseri.Luna.Bytecode;

public class InsnInvokeStatic : AbstractInsnInvoke {
	public InsnInvokeStatic(ReferenceTypeDescriptor ClassDescriptor, MethodDescriptor Method, bool Interface = false) : base(ClassDescriptor, Method, Interface) { }
	public InsnInvokeStatic(string ClassName, MethodDescriptor Method, bool Interface = false) : base(ClassName, Method, Interface) { }
	public InsnInvokeStatic(CodeReader.ManagedMethodReference Mref) : base(Mref) { }

	protected override Opcode Opcode => Opcode.InvokeStatic;
	protected override string Instruction => "invokestatic";
}