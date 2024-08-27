using Alluseri.Luna.Utils;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace Alluseri.Luna.Internals;

public class RecordAttribute : AttributeInfo {
	public readonly RecordComponentInfo[] Components;

	public RecordAttribute(RecordComponentInfo[] Components)
		: base("Record", 2 + Components.Sum(Cmpt => 6 + GU.GetSize(Cmpt.Attributes))) {
		this.Components = Components;
	}

	public override int GetHashCode() => HashCode.Combine(Name, Components);
	public override bool Equals(object? Object) => Object is RecordAttribute Attr && Attr.Components.SequenceEqual(Components);
	public override string ToString() => $"{{ Record [ {GU.ToString(Components)} ] }}";

	public static AttributeInfo ParseRecord(Stream Stream, ConstantPool Pool) {
		byte[] Store = new byte[Stream.ReadUInt()];
		using MemoryStream Substream = new(Store, 0, Stream.Read(Store));

		if (!Substream.ReadUShort(out ushort ComponentsCount))
			return new MalformedAttribute("Record", Store);

		RecordComponentInfo[] Components = new RecordComponentInfo[ComponentsCount];
		for (ushort i = 0; i < ComponentsCount; i++) {
			if (
				!Substream.ReadUShort(out ushort NameIndex) ||
				!Substream.ReadUShort(out ushort DescriptorIndex) ||
				!Substream.ReadUShort(out ushort AttributesCount)
			)
				return new MalformedAttribute("Record", Store);

			AttributeInfo[] Attributes = new AttributeInfo[AttributesCount];
			for (ushort j = 0; j < AttributesCount; j++) {
				if ((Attributes[j] = AttributeInfo.Parse(Substream, Pool)!) == null)
					return new MalformedAttribute("Record", Store);
			}

			Components[i] = new RecordComponentInfo(NameIndex, DescriptorIndex, Attributes);
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
		Stream.Write(Size - 6);
		Stream.Write((ushort) Components.Length);
		foreach (RecordComponentInfo Rci in Components)
			Rci.Write(Stream, Pool);
	}
}