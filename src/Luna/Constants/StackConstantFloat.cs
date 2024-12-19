using Alluseri.Luna.Bytecode;
using Alluseri.Luna.Internals;
using Alluseri.Luna.Utils;
using System.IO;

namespace Alluseri.Luna;

public class StackConstantFloat : StackConstant {
	public float Value;

	private ushort PoolIndex;

	public StackConstantFloat(float Value) {
		this.Value = Value;
	}

	internal override void CheckoutLdc(CodeBuilder Builder, out int Size) {
		Size = Value switch {
			0 or 1 or 2 => 1,
			_ => GetLdcSize(PoolIndex = CheckoutPool(Builder))
		};
	}

	internal override ushort CheckoutPool(CodeBuilder Builder) => Builder.Pool.Checkout(new ConstantFloat(Value));

	internal override void WriteLdc(Stream Stream) {
		switch (Value) {
			case 0:
			Stream.Write(Opcode.FConst_0);
			break;
			case 1:
			Stream.Write(Opcode.FConst_1);
			break;
			case 2:
			Stream.Write(Opcode.FConst_2);
			break;
			default:
			Ldc(Stream, PoolIndex);
			break;
		}
	}

	public override string ToString() => $"{{ Float {Value}F }}";
	public override string ToLdcString() => $"push.f {Value}F";
}