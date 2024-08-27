using Alluseri.Luna.Utils;
using System;
using System.IO;

namespace Alluseri.Luna.Internals;

public class ConstantString : ConstantInfo {
	public readonly ushort PoolIndex;

	public ConstantString(Stream Stream) : this(Stream.ReadUShort()) { }
	public ConstantString(ushort PoolIndex) : base(ConstantInfoTag.String) {
		this.PoolIndex = PoolIndex;
	}

	public string GetString(ConstantPool Pool) => ((ConstantUtf8) Pool[PoolIndex]).Value;

	public override int GetHashCode() => HashCode.Combine(Tag, PoolIndex);
	public override bool Equals(object? Object) => Object is ConstantString Constant && Constant.PoolIndex == PoolIndex;
	public override string ToString() => $"{{ String #{PoolIndex} }}";

	public override void Write(Stream Stream) {
		Stream.Write((byte) ConstantInfoTag.String);
		Stream.Write(PoolIndex);
	}
}