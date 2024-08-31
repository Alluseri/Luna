using Alluseri.Luna.Utils;
using System;
using System.IO;
using System.Linq;

namespace Alluseri.Luna.Internals;

public class RuntimeParameterAnnotationsAttribute : AttributeInfo {
	public readonly AnnotationInfo[][] Parameters;
	public readonly bool Visible;

	public RuntimeParameterAnnotationsAttribute(AnnotationInfo[][] Parameters, bool Visible)
		: base(Visible ? "RuntimeVisibleParameterAnnotations" : "RuntimeInvisibleParameterAnnotations", 1 + Parameters.Sum(pa => 2 + GU.GetSize(pa))) {
		this.Parameters = Parameters;
	}

	public override int GetHashCode() => HashCode.Combine(Name, Parameters);
	public override bool Equals(object? Object) => Object is RuntimeParameterAnnotationsAttribute Attr && Attr.Visible == Visible && Attr.Parameters.SequenceEqual(Parameters);
	public override string ToString() => $"{{ {Name} [ {GU.ToString(Parameters)} ] }}";

	public static AttributeInfo ParseRPA(Stream Stream, bool Visible) {
		byte[] Store = new byte[Stream.ReadUInt()];
		using MemoryStream Substream = new(Store, 0, Stream.Read(Store));

		if (Store.Length == 0)
			return new MalformedAttribute(Visible ? "RuntimeVisibleParameterAnnotations" : "RuntimeInvisibleParameterAnnotations", Store);

		AnnotationInfo[][] ParameterAnnotations = new AnnotationInfo[Substream.ReadByte()][];
		for (byte i = 0; i < ParameterAnnotations.Length; i++) {
			if (!Substream.ReadUShort(out ushort NumAnnotations))
				return new MalformedAttribute(Visible ? "RuntimeVisibleParameterAnnotations" : "RuntimeInvisibleParameterAnnotations", Store);

			ParameterAnnotations[i] = new AnnotationInfo[NumAnnotations];
			for (ushort j = 0; j < NumAnnotations; j++) {
				if ((ParameterAnnotations[i][j] = AnnotationInfo.Parse(Substream)!) == null)
					return new MalformedAttribute(Visible ? "RuntimeVisibleParameterAnnotations" : "RuntimeInvisibleParameterAnnotations", Store);
			}
		}

		return new RuntimeParameterAnnotationsAttribute(ParameterAnnotations, Visible);
	}

	protected override void Write(Stream Stream) {
		Stream.Write((byte) Parameters.Length);
		foreach (AnnotationInfo[] Param in Parameters) {
			Stream.Write((ushort) Param.Length);
			foreach (AnnotationInfo Ai in Param)
				Ai.Write(Stream);
		}
	}
}