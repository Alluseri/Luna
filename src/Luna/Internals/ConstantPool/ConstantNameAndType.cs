using Alluseri.Luna.Utils;
using System;
using System.IO;

namespace Alluseri.Luna.Internals;

public class ConstantNameAndType : ConstantInfo {
	public readonly ushort NameIndex;
	public readonly ushort DescriptorIndex;

	public ConstantNameAndType(Stream Stream) : this(Stream.ReadUShort(), Stream.ReadUShort()) { }
	public ConstantNameAndType(ushort NameIndex, ushort DescriptorIndex) : base(ConstantInfoTag.NameAndType) {
		this.NameIndex = NameIndex;
		this.DescriptorIndex = DescriptorIndex;
	}

	public string GetName(ConstantPool Pool) => ((ConstantUtf8) Pool[NameIndex]).Value;
	public string GetDescriptor(ConstantPool Pool) => ((ConstantUtf8) Pool[DescriptorIndex]).Value;

	public override int GetHashCode() => HashCode.Combine(Tag, NameIndex, DescriptorIndex);
	public override bool Equals(object? Object) => Object is ConstantNameAndType Constant && Constant.NameIndex == NameIndex && Constant.DescriptorIndex == DescriptorIndex;
	public override string ToString() => $"{{ NameAndType #{NameIndex}, #{DescriptorIndex} }}";

	public override void Write(Stream Stream) {
		Stream.Write((byte) ConstantInfoTag.NameAndType);
		Stream.Write(NameIndex);
		Stream.Write(DescriptorIndex);
	}
}