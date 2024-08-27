using Alluseri.Luna.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Alluseri.Luna.Internals;

public class StackMapTableAttribute : AttributeInfo {
	public readonly LinkableList<StackMapFrame> Frames;

	public StackMapTableAttribute(IEnumerable<StackMapFrame> Frames) : base("StackMapTable", 2 + GU.GetSize(Frames)) {
		this.Frames = new();
		this.Frames.AddAll(Frames);
		this.Frames.Lock();
	}

	public StackMapTableAttribute(LinkableList<StackMapFrame> Frames) : base("StackMapTable", 2 + GU.GetSize(Frames)) {
		Frames.Lock();
		this.Frames = Frames;
	}

	public override int GetHashCode() => HashCode.Combine(Name, Frames);
	public override bool Equals(object? Object) => Object is StackMapTableAttribute Attr && Attr.Frames.SequenceEqual(Frames);
	public override string ToString() => $"{{ StackMapTable [ {GU.ToString(Frames)} ] }}";

	public static AttributeInfo Parse(Stream Stream) {
		byte[] Store = new byte[Stream.ReadUInt()];
		using MemoryStream Substream = new(Store, 0, Stream.Read(Store));

		if (!Substream.ReadUShort(out ushort FrameCount))
			return new MalformedAttribute("StackMapTable", Store);

		LinkableList<StackMapFrame> Frames = new(FrameCount);

		for (ushort i = 0; i < FrameCount; i++) {
			if (StackMapFrame.Parse(Substream, Frames) == null) // Will automatically assign itself to the root Frames
				return new MalformedAttribute("StackMapTable", Store);
		}

		return new StackMapTableAttribute(Frames);
	}

	protected override void Write(Stream Stream) {
		Stream.Write((ushort) Frames.Count);
		foreach (StackMapFrame Smf in Frames) {
			Smf.Write(Stream);
		}
	}
}