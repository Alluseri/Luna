using Alluseri.Luna.Utils;
using System;
using System.IO;
using System.Linq;

namespace Alluseri.Luna.Internals;

public class ExceptionsAttribute : AttributeInfo {
	public readonly ushort[] ExceptionIndexes;

	public ExceptionsAttribute(ushort[] ExceptionIndexes)
		: base("Exceptions", 2 + (2 * ExceptionIndexes.Length)) {
		this.ExceptionIndexes = ExceptionIndexes;
	}

	public override int GetHashCode() => HashCode.Combine(Name, ExceptionIndexes);
	public override bool Equals(object? Object) => Object is ExceptionsAttribute Attr && Attr.ExceptionIndexes.SequenceEqual(ExceptionIndexes);
	public override string ToString() => $"{{ Exceptions [ {GU.ToString(ExceptionIndexes)} ] }}";

	public static AttributeInfo Parse(Stream Stream) {
		byte[] Store = new byte[Stream.ReadUInt()];
		using MemoryStream Substream = new(Store, 0, Stream.Read(Store));

		if (!Substream.ReadUShort(out ushort NumberOfExceptions))
			return new MalformedAttribute("Exceptions", Store);

		ushort[] ExceptionIndexes = new ushort[NumberOfExceptions];
		for (ushort i = 0; i < ExceptionIndexes.Length; i++) {
			if (!Substream.ReadUShort(out ExceptionIndexes[i]))
				return new MalformedAttribute("Exceptions", Store);
		}

		return new ExceptionsAttribute(ExceptionIndexes);
	}

	protected override void Write(Stream Stream) {
		Stream.Write((ushort) ExceptionIndexes.Length);
		foreach (ushort Ex in ExceptionIndexes)
			Stream.Write(Ex);
	}
}