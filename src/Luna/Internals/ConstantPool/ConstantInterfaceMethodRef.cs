using Alluseri.Luna.Utils;
using System;
using System.IO;

namespace Alluseri.Luna.Internals;

public class ConstantInterfaceMethodRef : ConstantInfo {
	public readonly ushort ClassIndex;
	public readonly ushort NameAndTypeIndex;

	public ConstantInterfaceMethodRef(Stream Stream) : this(Stream.ReadUShort(), Stream.ReadUShort()) { }
	public ConstantInterfaceMethodRef(ushort ClassIndex, ushort NameAndTypeIndex) : base(ConstantInfoTag.InterfaceMethodRef) {
		this.ClassIndex = ClassIndex;
		this.NameAndTypeIndex = NameAndTypeIndex;
	}

	public ConstantClass GetClass(ConstantPool Pool) => (ConstantClass) Pool[ClassIndex];
	public string GetClassName(ConstantPool Pool) => Pool.Value<ConstantUtf8>(((ConstantClass) Pool[ClassIndex]).NameIndex).Value;
	public ConstantNameAndType GetNameAndType(ConstantPool Pool) => (ConstantNameAndType) Pool[NameAndTypeIndex];

	public override int GetHashCode() => HashCode.Combine(Tag, ClassIndex, NameAndTypeIndex);
	public override bool Equals(object? Object) => Object is ConstantInterfaceMethodRef Constant && Constant.ClassIndex == ClassIndex && Constant.NameAndTypeIndex == NameAndTypeIndex;
	public override string ToString() => $"{{ InterfaceMethodRef to #{NameAndTypeIndex} in Class #{ClassIndex} }}";

	public override void Write(Stream Stream) {
		Stream.Write((byte) ConstantInfoTag.InterfaceMethodRef);
		Stream.Write(ClassIndex);
		Stream.Write(NameAndTypeIndex);
	}
}