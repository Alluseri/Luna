using Alluseri.Luna.Utils;
using System;
using System.IO;
using System.Linq;

namespace Alluseri.Luna.Internals;

public class InvalidAttribute : AttributeInfo { // Specification: All bytes will be attempted to read, the remaining length will be filled with zeroes, not truncated.
	public readonly ushort PoolIndex;
	public readonly byte[] Data;
	const int TruncateBytes = 8;

	public InvalidAttribute(ushort PoolIndex, byte[] Data) : base($"Luna:Invalid ({PoolIndex})", (uint) Data.Length) {
		this.PoolIndex = PoolIndex;
		this.Data = Data;
	}

	public override int GetHashCode() => HashCode.Combine(PoolIndex, Data);
	public override bool Equals(object? Object) => Object is InvalidAttribute Attr && Attr.PoolIndex == PoolIndex && Attr.Data.SequenceEqual(Data);
	public override string ToString() => Data.Length > TruncateBytes ? $"{{ Luna:Invalid ({PoolIndex}) [ {Convert.ToHexString(Data[..TruncateBytes])}... ({Data.Length} bytes total) ] }}" : $"{{ Luna:Invalid ({PoolIndex}) [ {Convert.ToHexString(Data)} ({Data.Length} bytes total) ] }}";

	public static AttributeInfo Parse(Stream Stream, ushort PoolIndex) {
		byte[] T = new byte[Stream.ReadUInt()];
		Stream.Read(T);
		return new InvalidAttribute(PoolIndex, T);
	}

	protected override void Write(Stream Stream) => throw new NotSupportedException($"InvalidAttribute has to be written using the Write(Stream, InternalConstantPool) method.");
	public override void Write(Stream Stream, ConstantPool Pool) {
		Stream.Write(PoolIndex);
		Stream.Write(Size);
		Stream.Write(Data);
	}
}