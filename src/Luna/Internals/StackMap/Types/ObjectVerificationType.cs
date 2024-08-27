using Alluseri.Luna.Utils;
using System;
using System.IO;

namespace Alluseri.Luna.Internals;

public class ObjectVerificationType : VerificationType {
	public readonly ushort PoolIndex;

	public ObjectVerificationType(ushort PoolIndex) : base(VerificationTag.Object) {
		this.PoolIndex = PoolIndex;
	}

	public override int Size => 3;

	public ConstantClass GetClass(ConstantPool Pool) => (ConstantClass) Pool[PoolIndex];

	public override int GetHashCode() => HashCode.Combine(Tag, PoolIndex);
	public override bool Equals(object? Object) => Object is ObjectVerificationType VT && VT.PoolIndex == PoolIndex;
	public override string ToString() => $"{{ ObjectVerificationType #{PoolIndex} }}";

	public override void Write(Stream Stream) {
		Stream.Write((byte) Tag);
		Stream.Write(PoolIndex);
	}
}