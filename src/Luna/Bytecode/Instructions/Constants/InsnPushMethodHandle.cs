using Alluseri.Luna.Internals;
using Alluseri.Luna.Utils;
using System.IO;

namespace Alluseri.Luna.Bytecode;

public class InsnPushMethodHandle : Instruction {
	public MethodHandle Handle;
	private ushort PoolIndex;

	public InsnPushMethodHandle(MethodHandle Handle) {
		this.Handle = Handle;
	}

	internal override void Checkout(ConstantPool Pool) {
		Size = GetLdcSize(PoolIndex = Handle.Checkout(Pool));
	}

	internal override void Write(Stream Stream, CodeBuilder Builder) {
		Ldc(Stream, PoolIndex);
	}

	public override string ToString() => $"push.mh {Handle}";
}