using Alluseri.Luna.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Alluseri.Luna.Internals;

public class InnerClassesAttribute : AttributeInfo {
	public IList<InnerClass> InnerClasses;

	public override int Size => 2 + (8 * InnerClasses.Count);

	public InnerClassesAttribute(IList<InnerClass> InnerClasses) : base("InnerClasses") {
		this.InnerClasses = InnerClasses;
	}
	public InnerClassesAttribute(InnerClass[] Classes) : this(GU.AsList(Classes)) { }

	public override int GetHashCode() => HashCode.Combine(Name, InnerClasses);
	public override bool Equals(object? Object) => Object is InnerClassesAttribute Attr && Attr.InnerClasses.SequenceEqual(InnerClasses);
	public override string ToString() => $"{{ InnerClasses [ {GU.ToString(InnerClasses)} ] }}";

	public static AttributeInfo Parse(Stream Stream) {
		byte[] Store = new byte[Stream.ReadUInt()];
		using MemoryStream Substream = new(Store, 0, Stream.Read(Store));

		if (!Substream.ReadUShort(out ushort Icl))
			return new MalformedAttribute("InnerClasses", Store);

		List<InnerClass> InnerClasses = new(Icl);

		for (ushort i = 0; i < Icl; i++) {
			if (
				!Substream.ReadUShort(out ushort InfoIndex) ||
				!Substream.ReadUShort(out ushort OuterInfoIndex) ||
				!Substream.ReadUShort(out ushort NameIndex) ||
				!Substream.ReadUShort(out ushort AccessFlags)
			)
				return new MalformedAttribute("InnerClasses", Store);
			InnerClasses.Add(new(InfoIndex, OuterInfoIndex, NameIndex, AccessFlags));
		}

		return new InnerClassesAttribute(InnerClasses);
	}

	protected override void Write(Stream Stream) {
		Stream.Write((ushort) InnerClasses.Count);
		foreach (InnerClass Ic in InnerClasses) {
			Ic.Write(Stream);
		}
	}
}