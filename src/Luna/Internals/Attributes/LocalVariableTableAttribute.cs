using Alluseri.Luna.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Alluseri.Luna.Internals;

public class LocalVariableTableAttribute : AttributeInfo {
	public IList<LocalVariableEntry> LocalVariables;

	public override int Size => 2 + (LocalVariables.Count * 10);

	public LocalVariableTableAttribute(IList<LocalVariableEntry> LocalVariables) : base("LocalVariableTable") {
		this.LocalVariables = LocalVariables;
	}
	public LocalVariableTableAttribute(LocalVariableEntry[] LocalVariables) : this(GU.AsList(LocalVariables)) { }

	public override int GetHashCode() => HashCode.Combine(Name, LocalVariables);
	public override bool Equals(object? Object) => Object is LocalVariableTableAttribute Attr && Attr.LocalVariables.SequenceEqual(LocalVariables);
	public override string ToString() => $"{{ LocalVariableTable [ {GU.ToString(LocalVariables)} ] }}";

	public static AttributeInfo Parse(Stream Stream) {
		byte[] Store = new byte[Stream.ReadUInt()];
		using MemoryStream Substream = new(Store, 0, Stream.Read(Store));

		if (!Substream.ReadUShort(out ushort LocalVariableTableLength))
			return new MalformedAttribute("LocalVariableTable", Store);

		List<LocalVariableEntry> LocalVariables = new(LocalVariableTableLength);
		for (ushort i = 0; i < LocalVariableTableLength; i++) {
			if (
				!Substream.ReadUShort(out ushort StartPc) ||
				!Substream.ReadUShort(out ushort Length) ||
				!Substream.ReadUShort(out ushort NameIndex) ||
				!Substream.ReadUShort(out ushort DescriptorIndex) ||
				!Substream.ReadUShort(out ushort Index)
			)
				return new MalformedAttribute("LocalVariableTable", Store);

			LocalVariables.Add(new LocalVariableEntry(StartPc, Length, NameIndex, DescriptorIndex, Index));
		}

		return new LocalVariableTableAttribute(LocalVariables);
	}

	protected override void Write(Stream Stream) {
		Stream.Write((ushort) LocalVariables.Count);
		foreach (LocalVariableEntry Lv in LocalVariables)
			Lv.Write(Stream);
	}
}