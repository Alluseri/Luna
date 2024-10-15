using Alluseri.Luna.Utils;
using System;
using System.IO;

namespace Alluseri.Luna.Internals;

public class ConstantModule : ConstantInfo {
	public readonly ushort PoolIndex;

	public ConstantModule(Stream Stream) : this(Stream.ReadUShort()) { }
	public ConstantModule(ushort NameIndex) : base(ConstantInfoTag.Module) {
		PoolIndex = NameIndex;
	}

	public string GetName(ConstantPool Pool) => ((ConstantUtf8) Pool[PoolIndex]).Value;

	public override int GetHashCode() => HashCode.Combine(Tag, PoolIndex);
	public override bool Equals(object? Object) => Object is ConstantModule Constant && Constant.PoolIndex == PoolIndex;
	public override string ToString() => $"{{ Module #{PoolIndex} }}";

	public override void Write(Stream Stream) {
		Stream.Write((byte) ConstantInfoTag.Module);
		Stream.Write(PoolIndex);
	}
}