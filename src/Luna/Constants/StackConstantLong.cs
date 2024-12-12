using Alluseri.Luna.Bytecode;
using Alluseri.Luna.Internals;
using Alluseri.Luna.Utils;
using System.IO;

namespace Alluseri.Luna;

public class StackConstantLong : StackConstant {
	public long Value;

	private ushort PoolIndex;

	public StackConstantLong(long Value) {
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

	internal override ushort CheckoutPool(CodeBuilder Builder) => Builder.Pool.Checkout(new ConstantLong(Value));

	internal override void WriteLdc(Stream Stream) {
		switch (Value) {
			case 0:
			Stream.Write(Opcode.LConst_0);
			break;
			case 1:
			Stream.Write(Opcode.LConst_1);
			break;
			default:
			Ldc(Stream, PoolIndex, true);
			break;
		}
	}

	public override string ToString() => $"{{ Long {Value}L }}";
	public override string ToLdcString() => $"push.l {Value}L";
}