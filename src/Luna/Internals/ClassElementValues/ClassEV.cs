using Alluseri.Luna.Utils;
using System;
using System.IO;

namespace Alluseri.Luna.Internals;

public class ClassEV : ElementValue {
	public readonly ushort PoolIndex;

	public ClassEV(ushort PoolIndex) : base('c') {
		this.PoolIndex = PoolIndex;
	}

	public override int Size => 3;

	public string GetDescriptor(ConstantPool Pool) => ((ConstantUtf8) Pool[PoolIndex]).Value;

	public override int GetHashCode() => HashCode.Combine(nameof(ClassEV), PoolIndex);
	public override bool Equals(object? Object) => Object is ClassEV IV && IV.PoolIndex == PoolIndex;
	public override string ToString() => $"{{ ClassEV #{PoolIndex} }}";

	public static ClassEV? ParseEV(Stream Stream) => Stream.ReadUShort(out ushort PoolIndex) ? new(PoolIndex) : null;

	protected override void WriteEV(Stream Stream) {
		Stream.Write(PoolIndex);
	}
}