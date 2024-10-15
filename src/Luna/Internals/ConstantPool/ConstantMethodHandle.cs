using Alluseri.Luna.Utils;
using System;
using System.IO;

namespace Alluseri.Luna.Internals;

public class ConstantMethodHandle : ConstantInfo {
	public readonly MethodHandleReferenceKind Kind;
	public readonly ushort PoolIndex;

	public ConstantMethodHandle(Stream Stream) : this((MethodHandleReferenceKind) (byte) Stream.ReadByte(), Stream.ReadUShort()) { }
	public ConstantMethodHandle(MethodHandleReferenceKind Kind, ushort PoolIndex) : base(ConstantInfoTag.MethodHandle) {
		this.Kind = Kind;
		this.PoolIndex = PoolIndex;
	}

	public ConstantInfo GetInfo(ConstantPool Pool) => Pool[PoolIndex];
	public T GetInfo<T>(ConstantPool Pool) where T : ConstantInfo => (T) Pool[PoolIndex];

	public override int GetHashCode() => HashCode.Combine(Tag, Kind, PoolIndex);
	public override bool Equals(object? Object) => Object is ConstantMethodHandle Constant && Constant.Kind == Kind && Constant.PoolIndex == PoolIndex;
	public override string ToString() => $"{{ MethodHandle #{PoolIndex} of kind {Kind} }}";

	public override void Write(Stream Stream) {
		Stream.Write((byte) ConstantInfoTag.MethodHandle);
		Stream.Write((byte) Kind);
		Stream.Write(PoolIndex);
	}
}