using Alluseri.Luna.Bytecode;
using Alluseri.Luna.Internals;
using Alluseri.Luna.Utils;
using System.IO;

namespace Alluseri.Luna;

public class StackConstantInteger : StackConstant {
	public int Value;

	private int Size;
	private ushort PoolIndex;

	public StackConstantInteger(int Value) {
		this.Value = Value;
	}
	public StackConstantInteger(short Value) {
		this.Value = Value;
	}
	public StackConstantInteger(ushort Value) {
		this.Value = Value;
	}
	public StackConstantInteger(byte Value) {
		this.Value = Value;
	}
	public StackConstantInteger(sbyte Value) {
		this.Value = Value;
	}

	internal override void CheckoutLdc(CodeBuilder Builder, out int Size) {
		PoolIndex = 0;
		this.Size = Size = Value switch {
			>= -1 and < 6 => 1,
			>= sbyte.MinValue and <= sbyte.MaxValue => 2,
			>= short.MinValue and <= short.MaxValue => 3,
			_ => GetLdcSize(PoolIndex = CheckoutPool(Builder))
		};
	}

	internal override ushort CheckoutPool(CodeBuilder Builder) => Builder.Pool.Checkout(new ConstantInteger(Value));

	internal override void WriteLdc(Stream Stream) {
		checked { // DEBUGTRACE: This shall be removed in production
			if (PoolIndex == 0) {
				switch (Size) {
					case 1:
					Stream.Write(Opcode.IConst_0, Value);
					break;
					case 2:
					Stream.Write(Opcode.BiPush);
					Stream.Write((sbyte) Value);
					break;
					case 3:
					Stream.Write(Opcode.SiPush);
					Stream.Write((short) Value);
					break;
				}
			} else
				Ldc(Stream, PoolIndex);
		}
	}

	public override string ToString() => $"{{ Integer {Value} }}";
	public override string ToLdcString() => $"push.i {Value}";
}