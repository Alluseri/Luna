using Alluseri.Luna.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Alluseri.Luna.Internals;

public class RuntimeAnnotationsAttribute : AttributeInfo {
	public IList<AnnotationInfo> Annotations;
	public bool Visible;

	public override int Size => 2 + GU.GetSize(Annotations);

	private static string GetName(bool Visible) => Visible ? "RuntimeVisibleAnnotations" : "RuntimeInvisibleAnnotations";

	public RuntimeAnnotationsAttribute(IList<AnnotationInfo> Annotations, bool Visible) : base(GetName(Visible)) {
		this.Annotations = Annotations;
	}
	public RuntimeAnnotationsAttribute(AnnotationInfo[] Annotations, bool Visible) : this(new List<AnnotationInfo>(Annotations), Visible) { }

	public override int GetHashCode() => HashCode.Combine(Name, Annotations);
	public override bool Equals(object? Object) => Object is RuntimeAnnotationsAttribute Attr && Attr.Visible == Visible && Attr.Annotations.SequenceEqual(Annotations);
	public override string ToString() => $"{{ {Name} [ {GU.ToString(Annotations)} ] }}";

	public static AttributeInfo ParseRA(Stream Stream, bool Visible) {
		byte[] Store = new byte[Stream.ReadUInt()];
		using MemoryStream Substream = new(Store, 0, Stream.Read(Store));

		if (!Substream.ReadUShort(out ushort AnnotationCount))
			return new MalformedAttribute(GetName(Visible), Store);

		List<AnnotationInfo> Annotations = new(AnnotationCount);
		for (ushort i = 0; i < AnnotationCount; i++) {
			AnnotationInfo? Annotation = AnnotationInfo.Parse(Substream);
			if (Annotation == null)
				return new MalformedAttribute(GetName(Visible), Store);
			Annotations.Add(Annotation);
		}

		return new RuntimeAnnotationsAttribute(Annotations, Visible);
	}

	protected override void Write(Stream Stream) {
		Stream.Write((ushort) Annotations.Count);
		foreach (AnnotationInfo Ann in Annotations) {
			Ann.Write(Stream);
		}
	}
}