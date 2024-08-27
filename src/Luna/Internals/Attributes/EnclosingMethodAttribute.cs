using Alluseri.Luna.Utils;
using System;
using System.IO;

namespace Alluseri.Luna.Internals;

public class EnclosingMethodAttribute : AttributeInfo {
	public readonly ushort ClassIndex;
	public readonly ushort MethodIndex;

	public EnclosingMethodAttribute(ushort ClassIndex, ushort MethodIndex)
		: base("EnclosingMethod", 4) {
		this.ClassIndex = ClassIndex;
		this.MethodIndex = MethodIndex;
	}

	public ConstantClass GetEnclosingClass(ConstantPool Pool) => (ConstantClass) Pool[ClassIndex];
	public ConstantNameAndType? GetEnclosingMethod(ConstantPool Pool) => ClassIndex == 0 ? null : (ConstantNameAndType) Pool[MethodIndex];

	public override int GetHashCode() => HashCode.Combine(Name, ClassIndex, MethodIndex);
	public override bool Equals(object? Object) => Object is EnclosingMethodAttribute Attr && Attr.ClassIndex == ClassIndex && Attr.MethodIndex == MethodIndex;
	public override string ToString() => $"{{ EnclosingMethod #{MethodIndex} in #{ClassIndex} }}";

	public static AttributeInfo Parse(Stream Stream) {
		byte[] Store = new byte[Stream.ReadUInt()];
		using MemoryStream Substream = new(Store, 0, Stream.Read(Store));

		if (!Substream.ReadUShort(out ushort ClassIndex) || !Substream.ReadUShort(out ushort MethodIndex))
			return new MalformedAttribute("EnclosingMethod", Store);

		return new EnclosingMethodAttribute(ClassIndex, MethodIndex);
	}

	protected override void Write(Stream Stream) {
		Stream.Write(ClassIndex);
		Stream.Write(MethodIndex);
	}
}