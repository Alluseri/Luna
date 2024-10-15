using Alluseri.Luna.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Alluseri.Luna.Internals;

public class StackMapTableAttribute : AttributeInfo {
	public IList<StackMapFrame> Frames;

	public override int Size => 2 + GU.GetSize(Frames);

	public StackMapTableAttribute(IList<StackMapFrame> Frames) : base("StackMapTable") {
		this.Frames = Frames;
	}
	public StackMapTableAttribute(StackMapFrame[] Frames) : this(GU.AsList(Frames)) { }

	public override int GetHashCode() => HashCode.Combine(Name, Frames);
	public override bool Equals(object? Object) => Object is StackMapTableAttribute Attr && Attr.Frames.SequenceEqual(Frames);
	public override string ToString() => $"{{ StackMapTable [ {GU.ToString(Frames)} ] }}";

	public static AttributeInfo Parse(Stream Stream) {
		byte[] Store = new byte[Stream.ReadUInt()];
		using MemoryStream Substream = new(Store, 0, Stream.Read(Store));

		if (!Substream.ReadUShort(out ushort FrameCount))
			return new MalformedAttribute("StackMapTable", Store);

		List<StackMapFrame> Frames = new(FrameCount);

		for (ushort i = 0; i < FrameCount; i++) {
			StackMapFrame? Frame = StackMapFrame.Parse(Substream);
			if (Frame == null)
				return new MalformedAttribute("StackMapTable", Store);
			Frames.Add(Frame);
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