using Alluseri.Luna.Bytecode;
using Alluseri.Luna.Internals;
using System.IO;

namespace Alluseri.Luna;

public class StackConstantString : StackConstant {
	public string Value;

	private ushort PoolIndex;

	public StackConstantString(string Value) {
		this.Value = Value;
	}

	internal override void CheckoutLdc(CodeBuilder Builder, out int Size) => Size = GetLdcSize(PoolIndex = CheckoutPool(Builder));

	internal override ushort CheckoutPool(CodeBuilder Builder) => Builder.Pool.Checkout(new ConstantString(Builder.Pool.CheckoutUTF8(Value)));

	internal override void WriteLdc(Stream Stream) {
		Ldc(Stream, PoolIndex);
	}

	public override string ToString() => $"{{ String \"{Value}\" }}"; // TODO: Escape (make a utility method god damn it)
	public override string ToLdcString() => $"ldc \"{Value}\""; // TODO: Escape
}