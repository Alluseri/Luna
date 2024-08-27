using Alluseri.Luna.Utils;
using System;
using System.IO;

namespace Alluseri.Luna.Internals;

public class ConstantMethodHandle : ConstantInfo {
	public readonly ReferenceKind Kind;
	public readonly ushort PoolIndex;

	public ConstantMethodHandle(Stream Stream) : this((ReferenceKind) (byte) Stream.ReadByte(), Stream.ReadUShort()) { }
	public ConstantMethodHandle(ReferenceKind Kind, ushort PoolIndex) : base(ConstantInfoTag.MethodHandle) {
		this.Kind = Kind;
		this.PoolIndex = PoolIndex;
	}

	public ConstantInfo GetInfo(ConstantPool Pool) => Pool[PoolIndex];

	public override int GetHashCode() => HashCode.Combine(Tag, Kind, PoolIndex);
	public override bool Equals(object? Object) => Object is ConstantMethodHandle Constant && Constant.Kind == Kind && Constant.PoolIndex == PoolIndex;
	public override string ToString() => $"{{ MethodHandle #{PoolIndex} of kind {Kind} }}";

	public override void Write(Stream Stream) {
		Stream.Write((byte) ConstantInfoTag.MethodHandle);
		Stream.Write((byte) Kind);
		Stream.Write(PoolIndex);
	}
}

public enum ReferenceKind : byte {
	GetField = 1, GetStatic, PutField, PutStatic, InvokeVirtual, InvokeStatic, InvokeSpecial, NewInvokeSpecial, InvokeInterface
}