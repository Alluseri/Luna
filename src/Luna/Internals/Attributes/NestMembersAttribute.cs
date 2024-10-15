using Alluseri.Luna.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Alluseri.Luna.Internals;

public class NestMembersAttribute : AttributeInfo {
	public IList<ushort> Classes;

	public override int Size => 2 + (2 * Classes.Count);

	public NestMembersAttribute(IList<ushort> Classes) : base("NestMembers") {
		this.Classes = Classes;
	}
	public NestMembersAttribute(ushort[] Classes) : this(GU.AsList(Classes)) { }

	public override int GetHashCode() => HashCode.Combine(Name, Classes);
	public override bool Equals(object? Object) => Object is NestMembersAttribute Attr && Attr.Classes.SequenceEqual(Classes);
	public override string ToString() => $"{{ NestMembers [ {GU.ToString(Classes.Select(Cl => $"#{Cl}"))} ] }}";

	public static AttributeInfo Parse(Stream Stream) {
		byte[] Store = new byte[Stream.ReadUInt()];
		using MemoryStream Substream = new(Store, 0, Stream.Read(Store));

		if (!Substream.ReadUShort(out ushort NumberOfClasses))
			return new MalformedAttribute("NestMembers", Store);

		List<ushort> Classes = new(NumberOfClasses);
		for (ushort i = 0; i < NumberOfClasses; i++) {
			if (!Substream.ReadUShort(out ushort Class))
				return new MalformedAttribute("NestMembers", Store);
			Classes.Add(Class);
		}

		return new NestMembersAttribute(Classes);
	}

	protected override void Write(Stream Stream) {
		Stream.Write((ushort) Classes.Count);
		foreach (ushort Class in Classes)
			Stream.Write(Class);
	}
}