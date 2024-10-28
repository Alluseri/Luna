using Alluseri.Luna.Internals;
using Alluseri.Luna.Utils;
using System.IO;

namespace Alluseri.Luna.Bytecode;

public abstract class AbstractInsnInvoke : Instruction {
	protected abstract Opcode Opcode { get; }
	protected abstract string Instruction { get; }

	public ReferenceTypeDescriptor ClassDescriptor;
	public MethodDescriptor Method;
	public bool Interface;

	public string ClassName {
		get => ClassDescriptor.SymbolicTerm;
		set => ClassDescriptor = ReferenceTypeDescriptor.ParseSymbolic(value);
	}

	public AbstractInsnInvoke(ReferenceTypeDescriptor ClassDescriptor, MethodDescriptor Method, bool Interface) : base(3) {
		this.ClassDescriptor = ClassDescriptor;
		this.Method = Method;
		this.Interface = Interface;
	}
	public AbstractInsnInvoke(string ClassName, MethodDescriptor Method, bool Interface) : base(3) {
		this.ClassDescriptor = ReferenceTypeDescriptor.ParseSymbolic(ClassName);
		this.Method = Method;
		this.Interface = Interface;
	}
	public AbstractInsnInvoke(CodeReader.ManagedMethodReference Mref) : this(Mref.ClassName, Mref.Method, Mref.Interface) { }

	internal override void Write(Stream Stream, CodeBuilder Builder, int Address) {
		Stream.Write(Opcode);
		Stream.Write(Builder.Pool.Checkout(Interface ? new ConstantInterfaceMethodRef(
			ClassDescriptor.CheckoutSymbolic(Builder.Pool),
			Method.Checkout(Builder.Pool)
		) : new ConstantMethodRef(
			ClassDescriptor.CheckoutSymbolic(Builder.Pool),
			Method.Checkout(Builder.Pool)
		)));
	}

	public override string ToString() => $"{Instruction}{(Interface ? ".i" : "")} {ClassName}.{Method.FullDescriptor}"; // TODO: Maybe a better indicator of an interface invocation?
}