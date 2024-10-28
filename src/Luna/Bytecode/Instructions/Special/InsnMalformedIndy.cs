using Alluseri.Luna.Internals;
using Alluseri.Luna.Utils;
using System.IO;

namespace Alluseri.Luna.Bytecode;

public class InsnMalformedIndy : Instruction {
	ushort BootstrapMethodIndex;
	MethodDescriptor Callee;

	public InsnMalformedIndy(ushort BootstrapMethodIndex, MethodDescriptor Callee) : base(5) {
		this.BootstrapMethodIndex = BootstrapMethodIndex;
		this.Callee = Callee;
	}

	internal override void Write(Stream Stream, CodeBuilder Builder, int Address) {
		Stream.Write(Opcode.InvokeDynamic);
		Stream.Write(Builder.Pool.Checkout(new ConstantInvokeDynamic(BootstrapMethodIndex, Callee.Checkout(Builder.Pool))));
		Stream.WriteByte(0);
		Stream.WriteByte(0);
	}

	public override string ToString() => $"malformed invokedynamic {Callee} from {BootstrapMethodIndex}";
}