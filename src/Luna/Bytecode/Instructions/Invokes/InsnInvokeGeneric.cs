using Alluseri.Luna.Internals;
using Alluseri.Luna.Utils;
using System;
using System.IO;

namespace Alluseri.Luna.Bytecode;

// TODO: Benchmark vs fields in ctor.
// DESIGN: internal?

public abstract class AbstractInsnInvoke : Instruction {
	protected abstract Opcode Opcode { get; }
	protected abstract string Instruction { get; }

	public string ClassName;
	public MethodDescriptor Method;
	public bool Interface;

	public AbstractInsnInvoke(string ClassName, MethodDescriptor Method, bool Interface) : base(3) {
		this.ClassName = ClassName;
		this.Method = Method;
		this.Interface = Interface;
	}

	internal override void Write(Stream Stream, CodeBuilder Builder) {
		Stream.Write(Opcode);
		Stream.Write(Builder.Pool.Checkout(Interface ? new ConstantInterfaceMethodRef(
			Builder.Pool.Checkout(new ConstantClass(Builder.Pool.CheckoutUtf8(ClassName))),
			Method.Checkout(Builder.Pool)
		) : new ConstantMethodRef(
			Builder.Pool.Checkout(new ConstantClass(Builder.Pool.CheckoutUtf8(ClassName))),
			Method.Checkout(Builder.Pool)
		)));
	}

	public override string ToString() => $"{Instruction}{(Interface ? ".i" : "")} {ClassName}.{Method.FullDescriptor}"; // TODO: Maybe a better indicator of an interface invocation?
}