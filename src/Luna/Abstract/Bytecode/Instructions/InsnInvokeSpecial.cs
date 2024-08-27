using Alluseri.Luna.Internals;
using Alluseri.Luna.Utils;
using System;
using System.IO;

namespace Alluseri.Luna.Abstract.Bytecode;

public class InsnInvokeSpecial : Instruction {
	public FullClassNameDescriptor ClassName;
	public MethodDescriptor Method;

	public InsnInvokeSpecial(FullClassNameDescriptor ClassName, MethodDescriptor Method) {
		this.ClassName = ClassName;
		this.Method = Method;
	}

	public override void Write(Stream Stream, InternalClass Class) {
		Stream.Write(Opcode.InvokeSpecial);
		Stream.Write
	}

	public override string ToString() => $"invokespecial {Method}";
	public override int GetHashCode() => HashCode.Combine(nameof(InsnInvokeSpecial));
	public override bool Equals(object? Other) => Other is InsnInvokeSpecial I && I.ClassName.Equals(ClassName) && I.Method.Equals(ClassName);
}