using Alluseri.Luna.Internals;
using Alluseri.Luna.Utils;
using System;
using System.IO;

namespace Alluseri.Luna.Abstract.Bytecode;

public class InsnPushString : Instruction {
	public string Value;

	public InsnPushString(string Value) {
		this.Value = Value;
	}

	public override void Write(Stream Stream, InternalClass Class) {
		Ldc(Stream, Class.ConstantPool.Checkout(new ConstantString(Class.ConstantPool.CheckoutUtf8(Value))), true);
	}

	public override string ToString() => $"ldc \"{Value}\""; // TODO: Escape
}