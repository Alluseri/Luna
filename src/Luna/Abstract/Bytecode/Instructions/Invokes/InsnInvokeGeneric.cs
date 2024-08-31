using Alluseri.Luna.Internals;
using Alluseri.Luna.Utils;
using System;
using System.IO;

namespace Alluseri.Luna.Abstract.Bytecode;

// TODO: Benchmark vs fields in ctor.
// DESIGN: internal?

public abstract class AbstractInsnInvoke : Instruction {
	public override int Size => 3;

	protected abstract Opcode Opcode { get; }
	protected abstract string Instruction { get; }

	public string ClassName;
	public MethodDescriptor Method;
	public bool Interface;

	public AbstractInsnInvoke(string ClassName, MethodDescriptor Method, bool Interface) {
		this.ClassName = ClassName;
		this.Method = Method;
		this.Interface = Interface;
	}

	public override void Write(Stream Stream, InternalClass Class) {
		Stream.Write(Opcode);
		Stream.Write(Class.ConstantPool.Checkout(Interface ? new ConstantInterfaceMethodRef(
			Class.ConstantPool.Checkout(new ConstantClass(Class.ConstantPool.CheckoutUtf8(ClassName))),
			Method.Checkout(Class.ConstantPool)
		) : new ConstantMethodRef(
			Class.ConstantPool.Checkout(new ConstantClass(Class.ConstantPool.CheckoutUtf8(ClassName))),
			Method.Checkout(Class.ConstantPool)
		)));
	}

	public override string ToString() => $"{Instruction}{(Interface ? ".i" : "")} {ClassName}.{Method.FullDescriptor}"; // TODO: Maybe a better indicator of an interface invocation?
}