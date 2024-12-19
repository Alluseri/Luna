using Alluseri.Luna.Bytecode;
using System.IO;

namespace Alluseri.Luna;

public class StackConstantClass : StackConstant {
	public ReferenceTypeDescriptor Descriptor;
	public string ClassName {
		get => Descriptor.SymbolicTerm;
		set => Descriptor = ReferenceTypeDescriptor.ParseSymbolic(value);
	}

	private ushort PoolIndex;

	public StackConstantClass(string Descriptor) {
		this.Descriptor = ReferenceTypeDescriptor.ParseSymbolic(Descriptor);
	}
	public StackConstantClass(ReferenceTypeDescriptor Descriptor) {
		this.Descriptor = Descriptor;
	}

	internal override void CheckoutLdc(CodeBuilder Builder, out int Size) => Size = GetLdcSize(PoolIndex = CheckoutPool(Builder));

	internal override ushort CheckoutPool(CodeBuilder Builder) => Descriptor.CheckoutSymbolic(Builder.Pool);

	internal override void WriteLdc(Stream Stream) => Ldc(Stream, PoolIndex);

	public override string ToString() => $"{{ Class {Descriptor} }}";
	public override string ToLdcString() => $"push.class {ClassName}";
}