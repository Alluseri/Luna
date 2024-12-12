using Alluseri.Luna.Bytecode;
using System.IO;

namespace Alluseri.Luna;

public class StackConstantMethodHandle : StackConstant {
	public MethodHandle Value; // DESIGN: A better name? "Handle" is tautology.

	private ushort PoolIndex;

	public StackConstantMethodHandle(MethodHandle Value) {
		this.Value = Value;
	}

	internal override void CheckoutLdc(CodeBuilder Builder, out int Size) => Size = GetLdcSize(PoolIndex = CheckoutPool(Builder));

	internal override ushort CheckoutPool(CodeBuilder Builder) => Value.Checkout(Builder.Pool);

	internal override void WriteLdc(Stream Stream) {
		Ldc(Stream, PoolIndex);
	}

	public override string ToString() => $"{{ MethodHandle {Value} }}";
	public override string ToLdcString() => $"push.mh {Value}";
}