using Alluseri.Luna.Bytecode;
using Alluseri.Luna.Internals;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Alluseri.Luna;

public class StackConstantDynamic : StackConstant {
	public BootstrapMethodBase Bootstrap;
	public FieldDescriptor ResolveTarget;

	public bool IsWide => ResolveTarget.FieldType is PrimitiveTypeDescriptor Ptd && Ptd.Type is PrimitiveType.Long or PrimitiveType.Double;

	private ushort PoolIndex;

	public StackConstantDynamic(BootstrapMethodBase Bootstrap, FieldDescriptor ResolveTarget) {
		this.Bootstrap = Bootstrap;
		this.ResolveTarget = ResolveTarget;
	}

	internal override void CheckoutLdc(CodeBuilder Builder, out int Size) {
		PoolIndex = CheckoutPool(Builder);
		Size = IsWide ? 3 : GetLdcSize(PoolIndex);
	}

	internal override ushort CheckoutPool(CodeBuilder Builder) {
		BootstrapMethodsAttribute? AttrBootstrapMethods = Builder.Attributes.BootstrapMethods;

		if (AttrBootstrapMethods == null)
			Builder.Attributes.AddFirst(AttrBootstrapMethods = new BootstrapMethodsAttribute(new List<Internals.BootstrapMethod>()));

		Internals.BootstrapMethod NewMethod = new(Bootstrap.Handle.Checkout(Builder.Pool), Bootstrap.Arguments.Select(Arg => Arg.CheckoutPool(Builder)).ToArray());

		return Builder.Pool.Checkout(new ConstantDynamic(AttrBootstrapMethods.Checkout(NewMethod), ResolveTarget.CheckoutNameAndType(Builder.Pool)));
	}

	internal override void WriteLdc(Stream Stream) {
		Ldc(Stream, PoolIndex, IsWide);
	}

	public override string ToString() => $"{{ Dynamic {ResolveTarget.FullDescriptor} from {Bootstrap} }}";
	public override string ToLdcString() => $"push.dyn {ResolveTarget.FullDescriptor} from {Bootstrap}";

	public static StackConstantDynamic FromConstantDynamic(InternalClass Class, ConstantDynamic Condy, HashSet<ushort> Fork) {
		return new(
		   BootstrapMethodBase.FromInternal(Class, Condy.GetBootstrapMethod(Class) ?? throw new InvalidDataException($"Got a malformed ConstantDynamic (no BootstrapMethods attribute), recovery from this is not yet implemented."), Fork),
		   FieldDescriptor.FromSignature(Class.ConstantPool, Condy.GetNameAndType(Class.ConstantPool))
		);
	}
}