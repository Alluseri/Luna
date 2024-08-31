using Alluseri.Luna.Utils;
using System;
using System.IO;

namespace Alluseri.Luna.Internals;

public class SourceFileAttribute : AttributeInfo {
	public readonly ushort NameIndex;

	public SourceFileAttribute(ushort NameIndex) : base("SourceFile", 2) {
		this.NameIndex = NameIndex;
	}

	public string GetName(ConstantPool Pool) => ((ConstantUtf8) Pool[NameIndex]).Value;

	public override int GetHashCode() => HashCode.Combine(Name, NameIndex);
	public override bool Equals(object? Object) => Object is SourceFileAttribute Attr && Attr.NameIndex == NameIndex;
	public override string ToString() => $"{{ SourceFile #{NameIndex} }}";

	public static AttributeInfo Parse(Stream Stream) {
		byte[] Store = new byte[Stream.ReadUInt()];
		using MemoryStream Substream = new(Store, 0, Stream.Read(Store));

		return Substream.ReadUShort(out ushort Index) ? new SourceFileAttribute(Index) : new MalformedAttribute("SourceFile", Store);
	}

	protected override void Write(Stream Stream) => Stream.Write(NameIndex);
}