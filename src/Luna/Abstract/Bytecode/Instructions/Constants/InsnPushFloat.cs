using Alluseri.Luna.Internals;
using Alluseri.Luna.Utils;
using System.IO;

namespace Alluseri.Luna.Abstract.Bytecode;

public class InsnPushFloat : Instruction {
	public float Value;

	public InsnPushFloat(float Value) {
		this.Value = Value;
	}

	public override void Write(Stream Stream, InternalClass Class) {
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
			Ldc(Stream, Class.ConstantPool.Checkout(new ConstantFloat(Value)));
			break;
		}
	}

	public override string ToString() => $"push.f {Value}F";
}