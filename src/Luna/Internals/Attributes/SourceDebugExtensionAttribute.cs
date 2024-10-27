using Alluseri.Luna.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Alluseri.Luna.Internals;

public class SourceDebugExtensionAttribute : AttributeInfo {
	public byte[] DebugExtension;
	const int TruncateBytes = 8;

	public override int Size => DebugExtension.Length;

	public SourceDebugExtensionAttribute(byte[] DebugExtension) : base("SourceDebugExtension") {
		this.DebugExtension = DebugExtension;
	}

	public override int GetHashCode() => HashCode.Combine(Name, DebugExtension);
	public override bool Equals(object? Object) => Object is SourceDebugExtensionAttribute Attr && Attr.DebugExtension.SequenceEqual(DebugExtension);
	public override string ToString() => DebugExtension.Length > TruncateBytes ? $"{{ SourceDebugExtension [ {Convert.ToHexString(DebugExtension[..TruncateBytes])}... ({DebugExtension.Length} bytes total) ] }}" : $"{{ SourceDebugExtension [ {Convert.ToHexString(DebugExtension)} ({DebugExtension.Length} bytes total) ] }}";

	public static AttributeInfo Parse(Stream Stream) {
		return Stream.ReadSafe(Stream.ReadUInt(), out byte[] Store) ? new SourceDebugExtensionAttribute(Store) : new MalformedAttribute("SourceDebugExtension", Store);
	}

	protected override void Write(Stream Stream) => Stream.Write(DebugExtension);
}