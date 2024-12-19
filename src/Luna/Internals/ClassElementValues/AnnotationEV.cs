using Alluseri.Luna.Utils;
using System;
using System.IO;

namespace Alluseri.Luna.Internals;

public class AnnotationEV : ElementValue {
	public readonly AnnotationInfo Value;

	public AnnotationEV(AnnotationInfo Nested) : base('@') {
		Value = Nested;
	}

	public override int GetHashCode() => HashCode.Combine(nameof(AnnotationEV), Value);
	public override bool Equals(object? Object) => Object is AnnotationEV IV && IV.Value.Equals(Value);
	public override string ToString() => $"{{ AnnotationEV {Value} }}";
	public override int Size => 1 + Value.Size;

	public static AnnotationEV? ParseEV(Stream Stream) {
		AnnotationInfo? K = AnnotationInfo.Parse(Stream);
		return K == null ? null : new(K);
	}

	protected override void WriteEV(Stream Stream) {
		Value.Write(Stream);
	}
}