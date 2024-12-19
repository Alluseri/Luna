using System;
using System.IO;
using System.Linq;

namespace Alluseri.Luna.Internals;

public class MalformedAttribute : AttributeInfo {
	public byte[] Data;
	const int TruncateBytes = 8;

	public override int Size => Data.Length;

	public MalformedAttribute(string Name, byte[] Data) : base(Name) {
		this.Data = Data;
	}

	public override int GetHashCode() => HashCode.Combine(Name, Data);
	public override bool Equals(object? Object) => Object is MalformedAttribute Attr && Attr.Name == Name && Attr.Data.SequenceEqual(Data);
	public override string ToString() => Data.Length > TruncateBytes ? $"{{ Luna:Malformed {Name} [ {Convert.ToHexString(Data[..TruncateBytes])}... ({Data.Length} bytes total) ] }}" : $"{{ Luna:Malformed {Name} [ {Convert.ToHexString(Data)} ({Data.Length} bytes total) ] }}";

	protected override void Write(Stream Stream) => Stream.Write(Data);
}