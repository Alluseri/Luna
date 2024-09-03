using Alluseri.Luna.Internals;
using Alluseri.Luna.Utils;
using System;
using System.IO;

namespace Alluseri.Luna.Abstract.Bytecode;

public class InsnPushString : Instruction {
	public string Value;
	private ushort PoolIndex;

	public InsnPushString(string Value) {
		this.Value = Value;
	}

	internal override void Checkout(ConstantPool Pool) {
		Size = GetLdcSize(PoolIndex = Pool.Checkout(new ConstantString(Pool.CheckoutUtf8(Value))));
	}

	internal override void Write(Stream Stream, InternalClass Class) {
		Ldc(Stream, PoolIndex);
	}

	public override string ToString() => $"ldc \"{Value}\""; // TODO: Escape
}