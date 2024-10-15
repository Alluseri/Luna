using Alluseri.Luna.Internals;
using Alluseri.Luna.Utils;
using System.IO;

namespace Alluseri.Luna.Bytecode;

public class InsnPushClass : Instruction {
	public string ClassName;
	private ushort PoolIndex;

	public InsnPushClass(string ClassName) {
		this.ClassName = ClassName;
	}

	internal override void Checkout(ConstantPool Pool) {
		Size = GetLdcSize(PoolIndex = Pool.Checkout(new ConstantClass(Pool.CheckoutUtf8(ClassName))));
	}

	internal override void Write(Stream Stream, CodeBuilder Builder) {
		Ldc(Stream, PoolIndex);
	}

	public override string ToString() => $"push.a {ClassName}.class";
}