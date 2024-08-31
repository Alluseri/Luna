using Alluseri.Luna;
using Alluseri.Luna.Utils;
using System;
using System.IO;
using System.Linq;

namespace Alluseri.Luna.Internals;

public class ConstantInvokeDynamic : ConstantInfo {
	public readonly ushort BootstrapMethodIndex;
	public readonly ushort NameAndTypeIndex;

	public ConstantInvokeDynamic(Stream Stream) : this(Stream.ReadUShort(), Stream.ReadUShort()) { }
	public ConstantInvokeDynamic(ushort BootstrapMethodIndex, ushort NameAndTypeIndex) : base(ConstantInfoTag.InvokeDynamic) {
		this.BootstrapMethodIndex = BootstrapMethodIndex;
		this.NameAndTypeIndex = NameAndTypeIndex;
	}

	public BootstrapMethod GetBootstrapMethod(InternalClass CF) => ((BootstrapMethodsAttribute) CF.Attributes.First(Attribute => Attribute is BootstrapMethodsAttribute)).BootstrapMethods[BootstrapMethodIndex];
	public BootstrapMethod GetBootstrapMethod(BootstrapMethodsAttribute Attribute) => Attribute.BootstrapMethods[BootstrapMethodIndex];
	public ConstantNameAndType GetNameAndType(ConstantPool Pool) => (ConstantNameAndType) Pool[NameAndTypeIndex];

	public override int GetHashCode() => HashCode.Combine(Tag, BootstrapMethodIndex, NameAndTypeIndex);
	public override bool Equals(object? Object) => Object is ConstantInvokeDynamic Constant && Constant.BootstrapMethodIndex == BootstrapMethodIndex && Constant.NameAndTypeIndex == NameAndTypeIndex;
	public override string ToString() => $"{{ InvokeDynamic #{NameAndTypeIndex}, Bootstrap #{BootstrapMethodIndex} }}";

	public override void Write(Stream Stream) {
		Stream.Write((byte) ConstantInfoTag.InvokeDynamic);
		Stream.Write(BootstrapMethodIndex);
		Stream.Write(NameAndTypeIndex);
	}
}