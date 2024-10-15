using Alluseri.Luna.Utils;
using System;
using System.IO;
using System.Linq;

namespace Alluseri.Luna.Internals;

public class CustomMalformedAttribute : AttributeInfo {
	public byte[] Data;
	public uint FakeSize;
	const int TruncateBytes = 8;

	public override int Size => Data.Length;

	public CustomMalformedAttribute(string Name, uint FakeSize, byte[] Data) : base(Name) {
		this.Data = Data;
		this.FakeSize = FakeSize;
	}

	public override int GetHashCode() => HashCode.Combine(Name, Data);
	public override bool Equals(object? Object) => Object is CustomMalformedAttribute Attr && Attr.Name == Name && Attr.FakeSize == FakeSize && Attr.Data.SequenceEqual(Data);
	public override string ToString() => Data.Length > TruncateBytes ? $"{{ Luna:Malformed:{FakeSize} {Name} [ {Convert.ToHexString(Data[..TruncateBytes])}... ({Data.Length} bytes total) ] }}" : $"{{ Luna:Malformed {Name} [ {Convert.ToHexString(Data)} ({Data.Length} bytes total) ] }}";

	protected override void Write(Stream Stream) => throw new NotSupportedException($"IntentionallyMalformedAttribute has to be written using the Write(Stream, InternalConstantPool) method.");
	public override void Write(Stream Stream, ConstantPool Pool) {
		Stream.Write(Pool.IndexOf(new ConstantUtf8(Name)));
		Stream.Write(FakeSize);
		Stream.Write(Data);
	}
}