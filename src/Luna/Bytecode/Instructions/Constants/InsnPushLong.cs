using Alluseri.Luna.Internals;
using Alluseri.Luna.Utils;
using System.IO;

namespace Alluseri.Luna.Bytecode;

public class InsnPushLong : Instruction {
	public long Value;

	public InsnPushLong(long Value) {
		this.Value = Value;
	}

	internal override void Checkout(ConstantPool Pool) {
		Size = Value switch {
			0 or 1 => 1,
			_ => 3,
		};
	}

	internal override void Write(Stream Stream, CodeBuilder Builder) {
		switch (Value) {
			case 0:
			Stream.Write(Opcode.LConst_0);
			break;
			case 1:
			Stream.Write(Opcode.LConst_1);
			break;
			default:
			Ldc(Stream, Builder.Pool.Checkout(new ConstantLong(Value)), true);
			break;
		}
	}

	public override string ToString() => $"push.l {Value}L";
}