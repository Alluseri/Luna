using Alluseri.Luna.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace Alluseri.Luna.Internals;

public class RecordAttribute : AttributeInfo {
	public IList<RecordComponentInfo> Components;

	public override int Size => 2 + Components.Sum(Cmpt => 6 + GU.GetSize(Cmpt.Attributes));

	public RecordAttribute(IList<RecordComponentInfo> Components) : base("Record") {
		this.Components = Components;
	}
	public RecordAttribute(RecordComponentInfo[] Components) : this(GU.AsList(Components)) { }

	public override int GetHashCode() => HashCode.Combine(Name, Components);
	public override bool Equals(object? Object) => Object is RecordAttribute Attr && Attr.Components.SequenceEqual(Components);
	public override string ToString() => $"{{ Record [ {GU.ToString(Components)} ] }}";

	public static AttributeInfo ParseRecord(Stream Stream, ConstantPool Pool) {
		byte[] Store = new byte[Stream.ReadUInt()];
		using MemoryStream Substream = new(Store, 0, Stream.Read(Store));

		if (!Substream.ReadUShort(out ushort ComponentsCount))
			return new MalformedAttribute("Record", Store);

		List<RecordComponentInfo> Components = new(ComponentsCount);
		for (ushort i = 0; i < ComponentsCount; i++) {
			if (
				!Substream.ReadUShort(out ushort NameIndex) ||
				!Substream.ReadUShort(out ushort DescriptorIndex) ||
				!Substream.ReadUShort(out ushort AttributesCount)
			)
				return new MalformedAttribute("Record", Store);

			List<AttributeInfo> Attributes = new(AttributesCount);
			for (ushort j = 0; j < AttributesCount; j++) {
				AttributeInfo? Attr = AttributeInfo.Parse(Substream, Pool);
				if (Attr == null)
					return new MalformedAttribute("Record", Store);
				Attributes.Add(Attr);
			}

			Components.Add(new(NameIndex, DescriptorIndex, Attributes));
		}

		return new RecordAttribute(Components);
	}

	public override void Checkout(ConstantPool Pool) {
		base.Checkout(Pool);
		foreach (RecordComponentInfo Rci in Components)
			foreach (AttributeInfo Ai in Rci.Attributes)
				Ai.Checkout(Pool);
	}

	protected override void Write(Stream Stream) => throw new NotSupportedException($"{Name} has to be written using the Write(Stream, InternalConstantPool) method.");
	public override void Write(Stream Stream, ConstantPool Pool) {
		Stream.Write(Pool.IndexOf(new ConstantUtf8(Name)));
		Stream.Write(Size);
		Stream.Write((ushort) Components.Count);
		foreach (RecordComponentInfo Rci in Components)
			Rci.Write(Stream, Pool);
	}
}