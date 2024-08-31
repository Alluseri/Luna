using Alluseri.Luna.Utils;
using System;
using System.IO;

namespace Alluseri.Luna.Internals;

public class ConstantClass : ConstantInfo {
	public readonly ushort NameIndex;

	public ConstantClass(Stream Stream) : this(Stream.ReadUShort()) { }
	public ConstantClass(ushort NameIndex) : base(ConstantInfoTag.Class) {
		this.NameIndex = NameIndex;
	}

	public string GetName(ConstantPool Pool) => ((ConstantUtf8) Pool[NameIndex]).Value;

	public override int GetHashCode() => HashCode.Combine(Tag, NameIndex);
	public override bool Equals(object? Object) => Object is ConstantClass Constant && Constant.NameIndex == NameIndex;
	public override string ToString() => $"{{ Class #{NameIndex} }}";

	public override void Write(Stream Stream) {
		Stream.Write((byte) ConstantInfoTag.Class);
		Stream.Write(NameIndex);
	}
}