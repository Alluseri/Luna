using Alluseri.Luna.Internals;
using Alluseri.Luna.Utils;
using System.IO;
using System.Linq;

namespace Alluseri.Luna.Bytecode;

public class InsnInvokeInterface : Instruction {
	public ReferenceTypeDescriptor Class;
	public MethodDescriptor Method;

	public string ClassName {
		get => Class.SymbolicTerm;
		set => Class = ReferenceTypeDescriptor.ParseSymbolic(value);
	}

	public InsnInvokeInterface(ReferenceTypeDescriptor Class, MethodDescriptor Method) : base(5) {
		this.Class = Class;
		this.Method = Method;
	}
	public InsnInvokeInterface(string ClassName, MethodDescriptor Method) : base(5) {
		this.Class = ReferenceTypeDescriptor.ParseSymbolic(ClassName);
		this.Method = Method;
	}

	internal override void Write(Stream Stream, CodeBuilder Builder, int Address) {
		Stream.Write(Opcode.InvokeInterface);
		Stream.Write(Builder.Pool.Checkout(new ConstantInterfaceMethodRef(
			Builder.Pool.Checkout(new ConstantClass(Builder.Pool.CheckoutUTF8(ClassName))),
			Method.Checkout(Builder.Pool)
		)));
		// This is normally redundant, so if it overflows, it's not a big deal (I hope it's not checked):
		Stream.WriteByte((byte) Method.Arguments.Descriptors.Sum(X => X is PrimitiveTypeDescriptor PTD && PTD.Type is PrimitiveType.Double or PrimitiveType.Long ? 2 : 1)); // TODO: This could be optimized
		Stream.WriteByte(0);
	}

	public override string ToString() => $"invokeinterface {ClassName}.{Method.FullDescriptor}";
}