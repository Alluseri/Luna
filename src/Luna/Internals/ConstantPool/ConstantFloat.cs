using Alluseri.Luna.Utils;
using System;
using System.IO;

namespace Alluseri.Luna.Internals;

public class ConstantFloat : ConstantInfo {
	public readonly float Value;

	public ConstantFloat(Stream Stream) : this(Stream.ReadFloat()) { }
	public ConstantFloat(float Value) : base(ConstantInfoTag.Float) {
		this.Value = Value;
	}

	public override int GetHashCode() => HashCode.Combine(Tag, Value);
	public override bool Equals(object? Object) => Object is ConstantFloat Constant && Constant.Value == Value;
	public override string ToString() => $"{{ Float {Value} }}";

	public override void Write(Stream Stream) {
		Stream.Write((byte) ConstantInfoTag.Float);
		Stream.Write(Value);
	}
}