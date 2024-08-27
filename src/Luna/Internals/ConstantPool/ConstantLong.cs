using Alluseri.Luna.Utils;
using System;
using System.IO;

namespace Alluseri.Luna.Internals;

public class ConstantLong : ConstantInfo {
	public readonly long Value;
	public override bool IsWide => true;

	public ConstantLong(Stream Stream) : this(Stream.ReadLong()) { }
	public ConstantLong(long Value) : base(ConstantInfoTag.Long) {
		this.Value = Value;
	}

	public override int GetHashCode() => HashCode.Combine(Tag, Value);
	public override bool Equals(object? Object) => Object is ConstantLong Constant && Constant.Value == Value;
	public override string ToString() => $"{{ Long {Value} }}";

	public override void Write(Stream Stream) {
		Stream.Write((byte) ConstantInfoTag.Long);
		Stream.Write(Value);
	}
}