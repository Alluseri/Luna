using Alluseri.Luna.Utils;
using System;
using System.IO;

namespace Alluseri.Luna.Internals;

public class ConstantInteger : ConstantInfo {
	public readonly int Value;

	public ConstantInteger(Stream Stream) : this(Stream.ReadInt()) { }
	public ConstantInteger(int Value) : base(ConstantInfoTag.Integer) {
		this.Value = Value;
	}

	public override int GetHashCode() => HashCode.Combine(Tag, Value);
	public override bool Equals(object? Object) => Object is ConstantInteger Constant && Constant.Value == Value;
	public override string ToString() => $"{{ Integer {Value} }}";

	public override void Write(Stream Stream) {
		Stream.Write((byte) ConstantInfoTag.Integer);
		Stream.Write(Value);
	}
}