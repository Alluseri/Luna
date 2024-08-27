using Alluseri.Luna;
using Alluseri.Luna.Utils;
using System;
using System.IO;
using System.Linq;

namespace Alluseri.Luna.Internals;

public class ConstantDynamic : ConstantInfo {
	public readonly ushort BootstrapMethodIndex;
	public readonly ushort NameAndTypeIndex;

	public ConstantDynamic(Stream Stream) : this(Stream.ReadUShort(), Stream.ReadUShort()) { }
	public ConstantDynamic(ushort BootstrapMethodIndex, ushort NameAndTypeIndex) : base(ConstantInfoTag.Dynamic) {
		this.BootstrapMethodIndex = BootstrapMethodIndex;
		this.NameAndTypeIndex = NameAndTypeIndex;
	}

	public BootstrapMethod GetBootstrapMethod(InternalClass CF) => ((BootstrapMethodsAttribute) CF.Attributes.First(Attribute => Attribute is BootstrapMethodsAttribute)).BootstrapMethods[BootstrapMethodIndex];
	public ConstantNameAndType GetNameAndType(ConstantPool Pool) => (ConstantNameAndType) Pool[NameAndTypeIndex];

	public override int GetHashCode() => HashCode.Combine(Tag, BootstrapMethodIndex, NameAndTypeIndex);
	public override bool Equals(object? Object) => Object is ConstantDynamic Constant && Constant.BootstrapMethodIndex == BootstrapMethodIndex && Constant.NameAndTypeIndex == NameAndTypeIndex;
	public override string ToString() => $"{{ Dynamic #{NameAndTypeIndex}, Bootstrap #{BootstrapMethodIndex} }}";

	public override void Write(Stream Stream) {
		Stream.Write((byte) ConstantInfoTag.Dynamic);
		Stream.Write(BootstrapMethodIndex);
		Stream.Write(NameAndTypeIndex);
	}
}