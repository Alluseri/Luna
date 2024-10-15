using Alluseri.Luna.Internals;
using Alluseri.Luna.Utils;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Alluseri.Luna.Bytecode;

public class InsnInvokeDynamic : Instruction {
	// Bootstrap(Callee, ...)(StackArgs)

	BootstrapMethod Bootstrap;
	MethodDescriptor Callee;

	public InsnInvokeDynamic(BootstrapMethod Bootstrap, MethodDescriptor Callee) : base(5) {
		this.Bootstrap = Bootstrap;
		this.Callee = Callee;
	}

	internal override void Write(Stream Stream, CodeBuilder Builder) {
		BootstrapMethodsAttribute? AttrBootstrapMethods = Builder.Attributes.BootstrapMethods;

		if (AttrBootstrapMethods == null)
			Builder.Attributes.AddFirst(AttrBootstrapMethods = new BootstrapMethodsAttribute(new List<Internals.BootstrapMethod>()));

		Internals.BootstrapMethod NewMethod = new(Bootstrap.Handle.Checkout(Builder.Pool), Bootstrap.Arguments.Select(Arg => Arg.Checkout(Builder.Pool)).ToArray());

		ushort IndyPool = Builder.Pool.Checkout(new ConstantInvokeDynamic(AttrBootstrapMethods.Checkout(NewMethod), Callee.Checkout(Builder.Pool)));

		Stream.Write(Opcode.InvokeDynamic);
		Stream.Write(IndyPool);
		Stream.WriteByte(0);
		Stream.WriteByte(0);
	}

	public override string ToString() => $"invokedynamic {Callee} from {Bootstrap}";
}