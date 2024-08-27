using Alluseri.Luna.Utils;
using System;
using System.IO;
using System.Linq;

namespace Alluseri.Luna.Internals;

public class InnerClassesAttribute : AttributeInfo {
	public readonly InnerClass[] InnerClasses;

	public InnerClassesAttribute(InnerClass[] Classes) : base("InnerClasses", 2 + (8 * Classes.Length)) {
		InnerClasses = Classes;
	}

	public override int GetHashCode() => HashCode.Combine(Name, InnerClasses);
	public override bool Equals(object? Object) => Object is InnerClassesAttribute Attr && Attr.InnerClasses.SequenceEqual(InnerClasses);
	public override string ToString() => $"{{ InnerClasses [ {GU.ToString(InnerClasses)} ] }}";

	public static AttributeInfo Parse(Stream Stream) {
		byte[] Store = new byte[Stream.ReadUInt()];
		using MemoryStream Substream = new(Store, 0, Stream.Read(Store));

		if (!Substream.ReadUShort(out ushort Icl))
			return new MalformedAttribute("InnerClasses", Store);

		InnerClass[] Ic = new InnerClass[Icl];

		for (ushort i = 0; i < Icl; i++) {
			if (
				!Substream.ReadUShort(out ushort InfoIndex) ||
				!Substream.ReadUShort(out ushort OuterInfoIndex) ||
				!Substream.ReadUShort(out ushort NameIndex) ||
				!Substream.ReadUShort(out ushort AccessFlags)
			)
				return new MalformedAttribute("InnerClasses", Store);
			Ic[i] = new(InfoIndex, OuterInfoIndex, NameIndex, AccessFlags);
		}

		return new InnerClassesAttribute(Ic);
	}

	protected override void Write(Stream Stream) {
		Stream.Write((ushort) InnerClasses.Length);
		foreach (InnerClass Ic in InnerClasses) {
			Ic.Write(Stream);
		}
	}
}