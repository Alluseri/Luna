using Alluseri.Luna.Internals;
using Alluseri.Luna.Utils;
using System.IO;

namespace Alluseri.Luna.Bytecode;

public class InsnNew : Instruction {
	public string ClassName;
	private ushort PoolIndex;

	public InsnNew(string ClassName) : base(3) {
		this.ClassName = ClassName;
	}

	internal override void Checkout(ConstantPool Pool) {
		PoolIndex = Pool.Checkout(new ConstantClass(Pool.CheckoutUtf8(ClassName)));
	}

	internal override void Write(Stream Stream, CodeBuilder Builder) {
		Stream.Write(Opcode.New);
		Stream.Write(PoolIndex);
	}

	public override string ToString() => $"new {ClassName}";
}