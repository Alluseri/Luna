using Alluseri.Luna.Internals;
using Alluseri.Luna.Utils;
using System.IO;

namespace Alluseri.Luna.Bytecode;

public class InsnPushFloat : Instruction {
	public float Value;
	private ushort PoolIndex;

	public InsnPushFloat(float Value) {
		this.Value = Value;
	}

	internal override void Checkout(ConstantPool Pool) {
		Size = Value switch {
			0 or 1 or 2 => 1,
			_ => GetLdcSize(PoolIndex = Pool.Checkout(new ConstantFloat(Value)))
		};
	}

	internal override void Write(Stream Stream, CodeBuilder Builder) {
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

	public override string ToString() => $"push.f {Value}F";
}