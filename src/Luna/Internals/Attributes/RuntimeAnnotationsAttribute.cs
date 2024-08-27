using Alluseri.Luna.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Alluseri.Luna.Internals;

public class RuntimeAnnotationsAttribute : AttributeInfo {
	public readonly AnnotationInfo[] Annotations;
	public readonly bool Visible;

	public RuntimeAnnotationsAttribute(AnnotationInfo[] Annotations, bool Visible) : base(Visible ? "RuntimeVisibleAnnotations" : "RuntimeInvisibleAnnotations", 2 + GU.GetSize(Annotations)) {
		this.Annotations = Annotations;
	}

	public override int GetHashCode() => HashCode.Combine(Name, Annotations);
	public override bool Equals(object? Object) => Object is RuntimeAnnotationsAttribute Attr && Attr.Visible == Visible && Attr.Annotations.SequenceEqual(Annotations);
	public override string ToString() => $"{{ {Name} [ {GU.ToString(Annotations)} ] }}";

	public static AttributeInfo ParseRA(Stream Stream, bool Visible) {
		byte[] Store = new byte[Stream.ReadUInt()];
		using MemoryStream Substream = new(Store, 0, Stream.Read(Store));

		if (!Substream.ReadUShort(out ushort AnnotationCount))
			return new MalformedAttribute(Visible ? "RuntimeVisibleAnnotations" : "RuntimeInvisibleAnnotations", Store);

		AnnotationInfo[] Annotations = new AnnotationInfo[AnnotationCount];
		for (ushort i = 0; i < AnnotationCount; i++) {
			if ((Annotations[i] = AnnotationInfo.Parse(Substream)!) == null)
				return new MalformedAttribute(Visible ? "RuntimeVisibleAnnotations" : "RuntimeInvisibleAnnotations", Store);
		}

		return new RuntimeAnnotationsAttribute(Annotations, Visible);
	}

	protected override void Write(Stream Stream) {
		Stream.Write((ushort) Annotations.Length);
		foreach (AnnotationInfo Ann in Annotations) {
			Ann.Write(Stream);
		}
	}
}