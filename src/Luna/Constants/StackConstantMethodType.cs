using Alluseri.Luna.Bytecode;
using System.IO;

namespace Alluseri.Luna;

public class StackConstantMethodType : StackConstant {
	public MethodTypeDescriptor Descriptor;

	private ushort PoolIndex;

	public StackConstantMethodType(MethodTypeDescriptor Descriptor) {
		this.Descriptor = Descriptor;
	}

	internal override void CheckoutLdc(CodeBuilder Builder, out int Size) => Size = GetLdcSize(PoolIndex = CheckoutPool(Builder));

	internal override ushort CheckoutPool(CodeBuilder Builder) => Descriptor.Checkout(Builder.Pool);

	internal override void WriteLdc(Stream Stream) {
		Ldc(Stream, PoolIndex);
	}

	public override string ToString() => $"{{ MethodType {Descriptor} }}";
	public override string ToLdcString() => $"push.mt {Descriptor}";
}