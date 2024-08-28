using Alluseri.Luna.Internals;
using Alluseri.Luna.Utils;
using System;
using System.IO;

namespace Alluseri.Luna.Abstract.Bytecode;

public class InsnInvokeSpecial : Instruction {
	public string ClassName;
	public MethodDescriptor Method;
	public bool Interface;

	public InsnInvokeSpecial(string ClassName, MethodDescriptor Method, bool Interface = false) {
		this.ClassName = ClassName;
		this.Method = Method;
		this.Interface = Interface;
	}

	public override void Write(Stream Stream, InternalClass Class) {
		Stream.Write(Opcode.InvokeSpecial);
		// Stream.Write();
	}

	public override string ToString() => $"invokespecial {Method}";
	public override int GetHashCode() => HashCode.Combine(nameof(InsnInvokeSpecial));
	public override bool Equals(object? Other) => Other is InsnInvokeSpecial I && I.ClassName.Equals(ClassName) && I.Method.Equals(ClassName);
}