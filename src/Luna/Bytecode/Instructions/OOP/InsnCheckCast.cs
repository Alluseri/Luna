using Alluseri.Luna.Internals;
using Alluseri.Luna.Utils;
using System.IO;

namespace Alluseri.Luna.Bytecode;

public class InsnCheckCast : Instruction {
	public string ClassName;
	private ushort PoolIndex;

	public InsnCheckCast(string ClassName) : base(3) {
		this.ClassName = ClassName;
	}

	internal override void Checkout(ConstantPool Pool) {
		PoolIndex = Pool.Checkout(new ConstantClass(Pool.CheckoutUtf8(ClassName)));
	}

	internal override void Write(Stream Stream, CodeBuilder Builder) {
		Stream.Write(Opcode.CheckCast);
		Stream.Write(PoolIndex);
	}

	public override string ToString() => $"checkcast {ClassName}";
}