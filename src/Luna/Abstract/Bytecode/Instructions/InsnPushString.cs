using Alluseri.Luna.Internals;
using Alluseri.Luna.Utils;
using System;
using System.IO;

namespace Alluseri.Luna.Abstract.Bytecode;

// DESIGN: Make Constants structs? They seem just perfect to be allocated on the stack given their use cases.

public class InsnPushString : Instruction {
	public string Value;

	public InsnPushString(string Value) {
		this.Value = Value;
	}

	public override void Write(Stream Stream, InternalClass Class) {
		Ldc(Stream, Class.ConstantPool.Checkout(new ConstantString(Class.ConstantPool.CheckoutUtf8(Value))), true);
	}

	public override string ToString() => $"ldc \"{Value}\""; // TODO: Escape
	public override int GetHashCode() => HashCode.Combine(nameof(InsnPushString));
	public override bool Equals(object? Other) => Other is InsnPushString I && I.Value == Value;
}