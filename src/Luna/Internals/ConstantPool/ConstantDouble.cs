using Alluseri.Luna.Utils;
using System;
using System.IO;

namespace Alluseri.Luna.Internals;

public class ConstantDouble : ConstantInfo {
	public readonly double Value;
	public override bool IsWide => true;

	public ConstantDouble(Stream Stream) : this(Stream.ReadDouble()) { }
	public ConstantDouble(double Value) : base(ConstantInfoTag.Double) {
		this.Value = Value;
	}

	public override int GetHashCode() => HashCode.Combine(Tag, Value);
	public override bool Equals(object? Object) => Object is ConstantDouble Constant && Constant.Value == Value;
	public override string ToString() => $"{{ Double {Value} }}";

	public override void Write(Stream Stream) {
		Stream.Write((byte) ConstantInfoTag.Double);
		Stream.Write(Value);
	}
}