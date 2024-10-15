using Alluseri.Luna.Exceptions;
using Alluseri.Luna.Utils;
using System;
using System.IO;

namespace Alluseri.Luna.Internals;

public abstract class ElementValue : ISizeable {
	public readonly char Tag;

	internal ElementValue(char Tag) {
		this.Tag = Tag;
	}

	public abstract int Size { get; }

	public abstract override bool Equals(object? obj);
	public abstract override int GetHashCode();
	public abstract override string ToString();

	public static ElementValue? Parse(Stream Stream) {
		char Tag = unchecked((char) Stream.ReadByte()); // No need to check for length - 0xFFFF is not a valid element value
		return Tag.IsValidConstantTag() ? ConstantEV.ParseEV(Tag, Stream) : Tag switch {
			'e' => EnumEV.ParseEV(Stream),
			'c' => ClassEV.ParseEV(Stream),
			'@' => AnnotationEV.ParseEV(Stream),
			'[' => ArrayEV.ParseEV(Stream),
			_ => null
		};
	}

	protected abstract void WriteEV(Stream Stream);
	public void Write(Stream Stream) {
		Stream.WriteByte((byte) Tag); // Will never exceed 0xFF because read from a byte
		WriteEV(Stream);
	}
}