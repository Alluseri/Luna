using Alluseri.Luna.Internals;
using Alluseri.Luna.Utils;
using System.IO;

namespace Alluseri.Luna.Bytecode;

public class InsnPushString : Instruction {
	public string Value;
	private ushort PoolIndex;

	public InsnPushString(string Value) {
		this.Value = Value;
	}

	internal override void Checkout(CodeBuilder Builder, int Address) {
		Size = GetLdcSize(PoolIndex = Builder.Pool.Checkout(new ConstantString(Builder.Pool.CheckoutUTF8(Value))));
	}

	internal override void Write(Stream Stream, CodeBuilder Builder, int Address) {
		Ldc(Stream, PoolIndex);
	}

	public override string ToString() => $"ldc \"{Value}\""; // TODO: Escape
}