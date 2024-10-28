using Alluseri.Luna.Internals;
using Alluseri.Luna.Utils;
using System.IO;

namespace Alluseri.Luna.Bytecode;

public class InsnPushMethodType : Instruction {
	public MethodTypeDescriptor MethodType;
	private ushort PoolIndex;

	public InsnPushMethodType(MethodTypeDescriptor MethodType) {
		this.MethodType = MethodType;
	}

	internal override void Checkout(CodeBuilder Builder, int Address) {
		Size = GetLdcSize(PoolIndex = MethodType.Checkout(Builder.Pool));
	}

	internal override void Write(Stream Stream, CodeBuilder Builder, int Address) {
		Ldc(Stream, PoolIndex);
	}

	public override string ToString() => $"push.mt {MethodType}";
}