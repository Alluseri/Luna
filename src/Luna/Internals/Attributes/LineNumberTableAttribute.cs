using Alluseri.Luna.Utils;
using System;
using System.IO;
using System.Linq;

namespace Alluseri.Luna.Internals;

public class LineNumberTableAttribute : AttributeInfo {
	public readonly LineEntry[] Lines;

	public LineNumberTableAttribute(LineEntry[] Lines)
		: base("LineNumberTable", 2 + (4 * Lines.Length)) {
		this.Lines = Lines;
	}

	public override int GetHashCode() => HashCode.Combine(Name, Lines);
	public override bool Equals(object? Object) => Object is LineNumberTableAttribute Attr && Attr.Lines.SequenceEqual(Lines);
	public override string ToString() => $"{{ LineNumberTable [{GU.ToString(Lines.Select(Line => $"#{Line.InstructionIndex}:L{Line.LineNumber}"))}] }}";

	public static AttributeInfo Parse(Stream Stream) {
		byte[] Store = new byte[Stream.ReadUInt()];
		using MemoryStream Substream = new(Store, 0, Stream.Read(Store));

		if (!Substream.ReadUShort(out ushort LineNumberTableLength))
			return new MalformedAttribute("LineNumberTable", Store);

		LineEntry[] LineNumbers = new LineEntry[LineNumberTableLength];
		for (ushort i = 0; i < LineNumbers.Length; i++) {
			if (
				!Substream.ReadUShort(out ushort StartPc) ||
				!Substream.ReadUShort(out ushort LineNumber)
			)
				return new MalformedAttribute("LineNumberTable", Store);

			LineNumbers[i] = new LineEntry(StartPc, LineNumber);
		}

		return new LineNumberTableAttribute(LineNumbers);
	}

	protected override void Write(Stream Stream) {
		Stream.Write((ushort) Lines.Length);
		foreach (LineEntry Le in Lines) {
			Stream.Write(Le.InstructionIndex);
			Stream.Write(Le.LineNumber);
		}
	}
}

public readonly record struct LineEntry(ushort InstructionIndex, ushort LineNumber);