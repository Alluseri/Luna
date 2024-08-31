using Alluseri.Luna.Internals;
using Alluseri.Luna.Utils;
using System.IO;

namespace Alluseri.Luna.Abstract.Bytecode;

public class InsnPushDouble : Instruction {
	public double Value;

	public InsnPushDouble(double Value) {
		this.Value = Value;
	}

	public override void Write(Stream Stream, InternalClass Class) {
		switch (Value) {
			case 0:
			Stream.Write(Opcode.DConst_0);
			break;
			case 1:
			Stream.Write(Opcode.DConst_1);
			break;
			default:
			Ldc(Stream, Class.ConstantPool.Checkout(new ConstantDouble(Value)), true);
			break;
		}
	}

	public override string ToString() => $"push.d {Value}D";
}