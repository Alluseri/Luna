using Alluseri.Luna.Utils;
using System;
using System.IO;

namespace Alluseri.Luna.Internals;

public class ConstantPackage : ConstantInfo {
	public readonly ushort NameIndex;

	public ConstantPackage(Stream Stream) : this(Stream.ReadUShort()) { }
	public ConstantPackage(ushort NameIndex) : base(ConstantInfoTag.Package) {
		this.NameIndex = NameIndex;
	}

	public string GetName(ConstantPool Pool) => ((ConstantUtf8) Pool[NameIndex]).Value;

	public override int GetHashCode() => HashCode.Combine(Tag, NameIndex);
	public override bool Equals(object? Object) => Object is ConstantPackage Constant && Constant.NameIndex == NameIndex;
	public override string ToString() => $"{{ Package #{NameIndex} }}";

	public override void Write(Stream Stream) {
		Stream.Write((byte) ConstantInfoTag.Package);
		Stream.Write(NameIndex);
	}
}