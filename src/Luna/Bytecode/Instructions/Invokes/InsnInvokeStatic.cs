using Alluseri.Luna.Internals;
using Alluseri.Luna.Utils;
using System;
using System.IO;

namespace Alluseri.Luna.Bytecode;

public class InsnInvokeStatic(string ClassName, MethodDescriptor Method, bool Interface = false) : AbstractInsnInvoke(ClassName, Method, Interface) {
	protected override Opcode Opcode => Opcode.InvokeStatic;
	protected override string Instruction => "invokestatic";
}