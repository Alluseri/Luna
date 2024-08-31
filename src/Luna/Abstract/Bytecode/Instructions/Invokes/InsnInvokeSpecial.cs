using Alluseri.Luna.Internals;
using Alluseri.Luna.Utils;
using System;
using System.IO;

namespace Alluseri.Luna.Abstract.Bytecode;

public class InsnInvokeSpecial(string ClassName, MethodDescriptor Method, bool Interface = false) : AbstractInsnInvoke(ClassName, Method, Interface) {
	protected override Opcode Opcode => Opcode.InvokeSpecial;
	protected override string Instruction => "invokespecial";
}