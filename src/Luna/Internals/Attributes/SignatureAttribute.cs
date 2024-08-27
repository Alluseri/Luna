using Alluseri.Luna.Utils;
using System;
using System.IO;

namespace Alluseri.Luna.Internals;

public class SignatureAttribute : AttributeInfo {
	public readonly ushort PoolIndex;

	public SignatureAttribute(ushort PoolIndex) : base("Signature", 2) {
		this.PoolIndex = PoolIndex;
	}

	public string GetSignature(ConstantPool Pool) => ((ConstantUtf8) Pool[PoolIndex]).Value;

	public override int GetHashCode() => HashCode.Combine(Name, PoolIndex);
	public override bool Equals(object? Object) => Object is SignatureAttribute Attr && Attr.PoolIndex == PoolIndex;
	public override string ToString() => $"{{ Signature #{PoolIndex} }}";

	public static AttributeInfo Parse(Stream Stream) {
		byte[] Store = new byte[Stream.ReadUInt()];
		using MemoryStream Substream = new(Store, 0, Stream.Read(Store));

		return Substream.ReadUShort(out ushort Index) ? new SignatureAttribute(Index) : new MalformedAttribute("Signature", Store);
	}

	protected override void Write(Stream Stream) => Stream.Write(PoolIndex);
}