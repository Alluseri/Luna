using Alluseri.Luna.Utils;
using System;
using System.IO;

namespace Alluseri.Luna.Internals;

public class ConstantMethodRef : ConstantInfo {
	public readonly ushort ClassIndex;
	public readonly ushort NameAndTypeIndex;

	public ConstantMethodRef(Stream Stream) : this(Stream.ReadUShort(), Stream.ReadUShort()) { }
	public ConstantMethodRef(ushort ClassIndex, ushort NameAndTypeIndex) : base(ConstantInfoTag.MethodRef) {
		this.ClassIndex = ClassIndex;
		this.NameAndTypeIndex = NameAndTypeIndex;
	}

	public ConstantClass GetClass(ConstantPool Pool) => (ConstantClass) Pool[ClassIndex];
	public string GetClassName(ConstantPool Pool) => Pool.Value<ConstantUtf8>(((ConstantClass) Pool[ClassIndex]).NameIndex).Value;
	public ConstantNameAndType GetNameAndType(ConstantPool Pool) => (ConstantNameAndType) Pool[NameAndTypeIndex];

	public override int GetHashCode() => HashCode.Combine(Tag, ClassIndex, NameAndTypeIndex);
	public override bool Equals(object? Object) => Object is ConstantMethodRef Constant && Constant.ClassIndex == ClassIndex && Constant.NameAndTypeIndex == NameAndTypeIndex;
	public override string ToString() => $"{{ MethodRef to #{NameAndTypeIndex} in Class #{ClassIndex} }}";

	public override void Write(Stream Stream) {
		Stream.Write((byte) ConstantInfoTag.MethodRef);
		Stream.Write(ClassIndex);
		Stream.Write(NameAndTypeIndex);
	}
}