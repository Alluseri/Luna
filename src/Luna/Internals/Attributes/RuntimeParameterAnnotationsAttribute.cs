using Alluseri.Luna.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Alluseri.Luna.Internals;

public class RuntimeParameterAnnotationsAttribute : AttributeInfo {
	public IList<IList<AnnotationInfo>> Parameters;
	public bool Visible;

	public override int Size => 1 + Parameters.Sum(Param => 2 + GU.GetSize(Param));

	private static string GetName(bool Visible) => Visible ? "RuntimeVisibleParameterAnnotations" : "RuntimeInvisibleParameterAnnotations";

	public RuntimeParameterAnnotationsAttribute(IList<IList<AnnotationInfo>> Parameters, bool Visible) : base(GetName(Visible)) {
		this.Parameters = Parameters;
	}
	public RuntimeParameterAnnotationsAttribute(AnnotationInfo[][] Parameters, bool Visible) : this((IList<IList<AnnotationInfo>>) Parameters.Select(Param => new List<AnnotationInfo>(Param)).ToList(), Visible) { }

	public override int GetHashCode() => HashCode.Combine(Name, Parameters);
	public override bool Equals(object? Object) => Object is RuntimeParameterAnnotationsAttribute Attr && Attr.Visible == Visible && Attr.Parameters.SequenceEqual(Parameters);
	public override string ToString() => $"{{ {Name} [ {GU.ToString(Parameters)} ] }}";

	public static AttributeInfo ParseRPA(Stream Stream, bool Visible) {
		byte[] Store = new byte[Stream.ReadUInt()];
		using MemoryStream Substream = new(Store, 0, Stream.Read(Store));

		int ParameterCount = Substream.ReadByte();
		if (ParameterCount == -1)
			return new MalformedAttribute(GetName(Visible), Store);

		List<IList<AnnotationInfo>> Parameters = new(ParameterCount);
		for (byte i = 0; i < ParameterCount; i++) {
			if (!Substream.ReadUShort(out ushort NumAnnotations))
				return new MalformedAttribute(GetName(Visible), Store);

			List<AnnotationInfo> ParameterAnnotations = new(NumAnnotations);
			for (ushort j = 0; j < NumAnnotations; j++) {
				AnnotationInfo? Annotation = AnnotationInfo.Parse(Substream);
				if (Annotation == null)
					return new MalformedAttribute(GetName(Visible), Store);
				ParameterAnnotations.Add(Annotation);
			}
			Parameters.Add(ParameterAnnotations);
		}

		return new RuntimeParameterAnnotationsAttribute(Parameters, Visible);
	}

	protected override void Write(Stream Stream) {
		Stream.Write((byte) Parameters.Count);
		foreach (AnnotationInfo[] Param in Parameters) {
			Stream.Write((ushort) Param.Length);
			foreach (AnnotationInfo Ai in Param)
				Ai.Write(Stream);
		}
	}
}