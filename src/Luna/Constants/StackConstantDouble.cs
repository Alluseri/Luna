using Alluseri.Luna.Bytecode;
using Alluseri.Luna.Internals;
using Alluseri.Luna.Utils;
using System.IO;

namespace Alluseri.Luna;

public class StackConstantDouble : StackConstant {
	public double Value;

	private ushort PoolIndex;

	public StackConstantDouble(double Value) {
		this.Value = Value;
	}

	internal override void CheckoutLdc(CodeBuilder Builder, out int Size) {
		if ((Size = Value switch {
			0 or 1 => 1,
			_ => 3,
		}) == 3) {
			PoolIndex = CheckoutPool(Builder);
		}
	}

	internal override ushort CheckoutPool(CodeBuilder Builder) => Builder.Pool.Checkout(new ConstantDouble(Value));

	internal override void WriteLdc(Stream Stream) {
		switch (Value) {
			case 0:
			Stream.Write(Opcode.DConst_0);
			break;
			case 1:
			Stream.Write(Opcode.DConst_1);
			break;
			default:
			Ldc(Stream, PoolIndex, true);
			break;
		}
	}

	public override string ToString() => $"{{ Double {Value}D }}";
	public override string ToLdcString() => $"push.d {Value}D";
}