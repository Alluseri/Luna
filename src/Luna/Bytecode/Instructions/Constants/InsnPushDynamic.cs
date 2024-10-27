using Alluseri.Luna.Internals;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Alluseri.Luna.Bytecode;

public class InsnPushDynamic : Instruction {
	public BootstrapMethod Bootstrap;
	public FieldDescriptor ResolveTarget;
	private ushort PoolIndex;

	public InsnPushDynamic(BootstrapMethod Bootstrap, FieldDescriptor ResolveTarget) {
		this.Bootstrap = Bootstrap;
		this.ResolveTarget = ResolveTarget;
	}

	internal override void Checkout(CodeBuilder Builder) { // yucky
		BootstrapMethodsAttribute? AttrBootstrapMethods = Builder.Attributes.BootstrapMethods;

		if (AttrBootstrapMethods == null)
			Builder.Attributes.AddFirst(AttrBootstrapMethods = new BootstrapMethodsAttribute(new List<Internals.BootstrapMethod>()));

		Internals.BootstrapMethod NewMethod = new(Bootstrap.Handle.Checkout(Builder.Pool), Bootstrap.Arguments.Select(Arg => Arg.Checkout(Builder.Pool, AttrBootstrapMethods)).ToArray());

		// TODO: This size calculation must be offset due to l/d issue
		Size = GetLdcSize(PoolIndex = Builder.Pool.Checkout(new ConstantDynamic(AttrBootstrapMethods.Checkout(NewMethod), ResolveTarget.CheckoutNameAndType(Builder.Pool))));
	}

	internal override void Write(Stream Stream, CodeBuilder Builder) {
		Ldc(Stream, PoolIndex, ResolveTarget.FieldType is PrimitiveTypeDescriptor Ptd && Ptd.Type is PrimitiveType.Long or PrimitiveType.Double);
	}

	public override string ToString() => $"push.dyn {ClassName}";
}