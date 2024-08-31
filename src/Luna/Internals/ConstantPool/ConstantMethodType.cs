using Alluseri.Luna.Utils;
using System;
using System.IO;

namespace Alluseri.Luna.Internals;

public class ConstantMethodType : ConstantInfo {
	public readonly ushort DescriptorIndex;

	public ConstantMethodType(Stream Stream) : this(Stream.ReadUShort()) { }
	public ConstantMethodType(ushort DescriptorIndex) : base(ConstantInfoTag.MethodType) {
		this.DescriptorIndex = DescriptorIndex;
	}

	public string GetDescriptor(ConstantPool Pool) => ((ConstantUtf8) Pool[DescriptorIndex]).Value;

	public override int GetHashCode() => HashCode.Combine(Tag, DescriptorIndex);
	public override bool Equals(object? Object) => Object is ConstantMethodType Constant && Constant.DescriptorIndex == DescriptorIndex;
	public override string ToString() => $"{{ MethodType #{DescriptorIndex} }}";

	public override void Write(Stream Stream) {
		Stream.Write((byte) ConstantInfoTag.MethodType);
		Stream.Write(DescriptorIndex);
	}
}