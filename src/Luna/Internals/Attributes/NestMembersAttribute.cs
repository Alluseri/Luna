using Alluseri.Luna.Utils;
using System;
using System.IO;
using System.Linq;

namespace Alluseri.Luna.Internals;

public class NestMembersAttribute : AttributeInfo {
	public readonly ushort[] Classes;

	public NestMembersAttribute(ushort[] Classes)
		: base("NestMembers", 2 + (2 * Classes.Length)) {
		this.Classes = Classes;
	}

	public override int GetHashCode() => HashCode.Combine(Name, Classes);
	public override bool Equals(object? Object) => Object is NestMembersAttribute Attr && Attr.Classes.SequenceEqual(Classes);
	public override string ToString() => $"{{ NestMembers [ {GU.ToString(Classes.Select(Cl => $"#{Cl}"))} ] }}";

	public static AttributeInfo Parse(Stream Stream) {
		byte[] Store = new byte[Stream.ReadUInt()];
		using MemoryStream Substream = new(Store, 0, Stream.Read(Store));

		if (!Substream.ReadUShort(out ushort NumberOfClasses))
			return new MalformedAttribute("NestMembers", Store);

		ushort[] Classes = new ushort[NumberOfClasses];
		for (ushort i = 0; i < NumberOfClasses; i++) {
			if (!Substream.ReadUShort(out Classes[i]))
				return new MalformedAttribute("NestMembers", Store);
		}

		return new NestMembersAttribute(Classes);
	}

	protected override void Write(Stream Stream) {
		Stream.Write((ushort) Classes.Length);
		foreach (ushort Class in Classes)
			Stream.Write(Class);
	}
}