using Alluseri.Luna.Internals;
using Alluseri.Luna.Utils;
using System;
using System.Linq;

namespace Alluseri.Luna;

public class DynamicBootstrapArgument : BootstrapArgument {
	public BootstrapMethod Bootstrap;
	public FieldDescriptor ResolveTarget;

	public DynamicBootstrapArgument(BootstrapMethod Bootstrap, FieldDescriptor ResolveTarget) {
		this.Bootstrap = Bootstrap;
		this.ResolveTarget = ResolveTarget;
	}

	public override ushort Checkout(ConstantPool Pool) => throw new NotSupportedException($"{nameof(DynamicBootstrapArgument)} has to be checked out using the Checkout(ConstantPool, BootstrapMethodsAttribute) method.");
	public ushort Checkout(ConstantPool Pool, BootstrapMethodsAttribute BootstrapMethods) {
		Internals.BootstrapMethod NewMethod = new(Bootstrap.Handle.Checkout(Pool), Bootstrap.Arguments.Select(Arg => Arg.Checkout(Pool)).ToArray());

		return Pool.Checkout(new ConstantDynamic(
			BootstrapMethods.Checkout(NewMethod),
			ResolveTarget.CheckoutNameAndType(Pool)
		));
	}
}