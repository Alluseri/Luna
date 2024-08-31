using Alluseri.Luna.Utils;
using System;
using System.IO;

namespace Alluseri.Luna.Internals;

public class DeprecatedAttribute : AttributeInfo {
	public DeprecatedAttribute() : base("Deprecated", 0) { }

	public override int GetHashCode() => HashCode.Combine(Name);
	public override bool Equals(object? Object) => Object is DeprecatedAttribute Attr;
	public override string ToString() => $"{{ Deprecated }}";

	public static AttributeInfo Parse(Stream Stream) {
		// This attribute cannot physically be malformed, but I'm legally required to fast forward every residual byte.

		if (Stream.CanSeek)
			Stream.Seek(Stream.ReadUInt(), SeekOrigin.Current);
		else
			Stream.Read(new byte[Stream.ReadUInt()]); // TODO: I swear, is there actually no better way to do this? I hate it wholeheartedly.

		return new DeprecatedAttribute();
	}

	protected override void Write(Stream Stream) { }
}