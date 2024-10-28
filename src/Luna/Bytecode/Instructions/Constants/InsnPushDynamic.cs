using Alluseri.Luna.Internals;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Alluseri.Luna.Bytecode;

public class InsnPushDynamic : Instruction {
	public BootstrapMethod Bootstrap;
	public FieldDescriptor ResolveTarget;
	private ushort PoolIndex;

	public bool IsWide => ResolveTarget.FieldType is PrimitiveTypeDescriptor Ptd && Ptd.Type is PrimitiveType.Long or PrimitiveType.Double;

	public InsnPushDynamic(BootstrapMethod Bootstrap, FieldDescriptor ResolveTarget) {
		this.Bootstrap = Bootstrap;
		this.ResolveTarget = ResolveTarget;
	}

	internal override void Checkout(CodeBuilder Builder, int Address) { // yucky	
		BootstrapMethodsAttribute? AttrBootstrapMethods = Builder.Attributes.BootstrapMethods;

		if (AttrBootstrapMethods == null)
			Builder.Attributes.AddFirst(AttrBootstrapMethods = new BootstrapMethodsAttribute(new List<Internals.BootstrapMethod>()));

		Internals.BootstrapMethod NewMethod = new(Bootstrap.Handle.Checkout(Builder.Pool), Bootstrap.Arguments.Select(Arg => Arg.Checkout(Builder.Pool, AttrBootstrapMethods)).ToArray());

		PoolIndex = Builder.Pool.Checkout(new ConstantDynamic(AttrBootstrapMethods.Checkout(NewMethod), ResolveTarget.CheckoutNameAndType(Builder.Pool)));

		if (IsWide && false)
			Size = 3;
		else
			Size = GetLdcSize(PoolIndex);
	}

	internal override void Write(Stream Stream, CodeBuilder Builder, int Address) {
		Ldc(Stream, PoolIndex, IsWide);
	}

	public override string ToString() => $"push.dyn {ResolveTarget.FullDescriptor} from {Bootstrap}";
}