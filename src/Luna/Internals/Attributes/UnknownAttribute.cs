using Alluseri.Luna.Utils;
using System;
using System.IO;
using System.Linq;

namespace Alluseri.Luna.Internals;

public class UnknownAttribute : AttributeInfo {
	public byte[] Data;
	const int TruncateBytes = 16;

	public override int Size => Data.Length;

	public UnknownAttribute(string Name, byte[] Data) : base(Name) {
		this.Data = Data;
	}

	public override int GetHashCode() => HashCode.Combine(Name, Data);
	public override bool Equals(object? Object) => Object is UnknownAttribute Attr && Attr.Name == Name && Attr.Data.SequenceEqual(Data);
	public override string ToString() => Data.Length > TruncateBytes ? $"{{ Luna:Unknown {Name} [ {Convert.ToHexString(Data[..TruncateBytes])}... ({Data.Length} bytes total) ] }}" : $"{{ Luna:Unknown {Name} [ {Convert.ToHexString(Data)} ({Data.Length} bytes total) ] }}";

	public static AttributeInfo Parse(Stream Stream, string Name) => new UnknownAttribute(Name, Stream.ReadSegment(Stream.ReadUInt()));

	protected override void Write(Stream Stream) => Stream.Write(Data);
}