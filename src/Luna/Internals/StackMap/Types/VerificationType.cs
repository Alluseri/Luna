using Alluseri.Luna.Utils;
using System;
using System.IO;

namespace Alluseri.Luna.Internals;

public abstract class VerificationType : ISizeable {
	public VerificationTag Tag { get; init; }

	internal VerificationType(VerificationTag Tag) { // No thanks, I don't want you to make a non-existent vtag.
		this.Tag = Tag;
	}

	public virtual int Size => 1;
	public virtual bool Category2 => false; // TODO: Better name please

	public override int GetHashCode() => HashCode.Combine(Tag);
	public override bool Equals(object? Object) => Object is VerificationType VT && VT.Tag == Tag;
	public override string ToString() => $"{{ VerificationType {Tag} }}";

	public static VerificationType? Parse(Stream Stream) {
		VerificationTag Tag = (VerificationTag) Stream.ReadByte();
		return Tag switch {
			VerificationTag.Top => new TopVerificationType(),
			VerificationTag.Integer => new IntegerVerificationType(),
			VerificationTag.Float => new FloatVerificationType(),
			VerificationTag.Double => new DoubleVerificationType(),
			VerificationTag.Long => new LongVerificationType(),
			VerificationTag.Null => new NullVerificationType(),
			VerificationTag.UninitializedThis => new UninitializedThisVerificationType(),
			VerificationTag.Object => Stream.ReadUShort(out ushort PoolIndex) ? new ObjectVerificationType(PoolIndex) : null,
			VerificationTag.Uninitialized => Stream.ReadUShort(out ushort Delta) ? new UninitializedVerificationType(Delta) : null,
			_ => null
		};
	}

	public virtual void Write(Stream Stream) {
		Stream.Write((byte) Tag);
	}
}

public enum VerificationTag : byte {
	Top, Integer, Float, Double, Long, Null, UninitializedThis, Object, Uninitialized
}