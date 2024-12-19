using Alluseri.Luna.Utils;
using System;
using System.IO;

namespace Alluseri.Luna.Internals;

public class DeprecatedAttribute : AttributeInfo {
	public override int Size => 0;

	public DeprecatedAttribute() : base("Deprecated") { }

	public override int GetHashCode() => HashCode.Combine(Name);
	public override bool Equals(object? Object) => Object is DeprecatedAttribute Attr;
	public override string ToString() => $"{{ Deprecated }}";

	public static AttributeInfo? Parse(Stream Stream) => Stream.SkipSafe(Stream.ReadUInt()) ? new DeprecatedAttribute() : null;

	protected override void Write(Stream Stream) { }
}