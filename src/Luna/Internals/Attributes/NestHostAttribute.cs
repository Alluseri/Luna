using Alluseri.Luna.Utils;
using System;
using System.IO;

namespace Alluseri.Luna.Internals;

public class NestHostAttribute : AttributeInfo {
	public ushort HostClassIndex;

	public override int Size => 2;

	public NestHostAttribute(ushort HostClassIndex) : base("NestHost") {
		this.HostClassIndex = HostClassIndex;
	}

	public ConstantClass GetHostClass(ConstantPool Pool) => (ConstantClass) Pool[HostClassIndex];

	public override int GetHashCode() => HashCode.Combine(Name, HostClassIndex);
	public override bool Equals(object? Object) => Object is NestHostAttribute Attr && Attr.HostClassIndex == HostClassIndex;
	public override string ToString() => $"{{ NestHost #{HostClassIndex} }}";

	public static AttributeInfo? Parse(Stream Stream) {
		MemoryStream? Substream = Stream.ReadSafeStream(Stream.ReadUInt(), out byte[] Store);
		if (Substream == null)
			return null;

		return Substream.ReadUShort(out ushort Index) ? new NestHostAttribute(Index) : new MalformedAttribute("NestHost", Store);
	}

	protected override void Write(Stream Stream) => Stream.Write(HostClassIndex);
}