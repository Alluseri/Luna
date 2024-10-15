using Alluseri.Luna.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Alluseri.Luna.Internals;

public class ExceptionsAttribute : AttributeInfo {
	public IList<ushort> ExceptionIndexes;

	public override int Size => 2 + (2 * ExceptionIndexes.Count);

	public ExceptionsAttribute(IList<ushort> ExceptionIndexes) : base("Exceptions") {
		this.ExceptionIndexes = ExceptionIndexes;
	}
	public ExceptionsAttribute(ushort[] ExceptionIndexes) : this(GU.AsList(ExceptionIndexes)) { }

	public override int GetHashCode() => HashCode.Combine(Name, ExceptionIndexes);
	public override bool Equals(object? Object) => Object is ExceptionsAttribute Attr && Attr.ExceptionIndexes.SequenceEqual(ExceptionIndexes);
	public override string ToString() => $"{{ Exceptions [ {GU.ToString(ExceptionIndexes)} ] }}";

	public static AttributeInfo Parse(Stream Stream) {
		byte[] Store = new byte[Stream.ReadUInt()];
		using MemoryStream Substream = new(Store, 0, Stream.Read(Store));

		if (!Substream.ReadUShort(out ushort NumberOfExceptions))
			return new MalformedAttribute("Exceptions", Store);

		List<ushort> ExceptionIndexes = new(NumberOfExceptions);
		for (ushort i = 0; i < NumberOfExceptions; i++) {
			if (!Substream.ReadUShort(out ushort Index))
				return new MalformedAttribute("Exceptions", Store);
			ExceptionIndexes.Add(Index);
		}

		return new ExceptionsAttribute(ExceptionIndexes);
	}

	protected override void Write(Stream Stream) {
		Stream.Write((ushort) ExceptionIndexes.Count);
		foreach (ushort Ex in ExceptionIndexes)
			Stream.Write(Ex);
	}
}