using Alluseri.Luna.Utils;
using System;
using System.IO;

namespace Alluseri.Luna.Internals;

internal class ConstantPadding : ConstantInfo {
	public override bool IsWide => true; // It is not possible to generate a valid class file in which skipping over a wide instruction would yield another padding. No behavioral changes.

	public ConstantPadding() : base(ConstantInfoTag.LunaPadding) { }

	public override int GetHashCode() => HashCode.Combine(Tag);
	public override bool Equals(object? Object) => Object is ConstantPadding Constant;
	public override string ToString() => $"{{ Padding }}";

	public override void Write(Stream Stream) {
		// Mhm.. Fuck you, JVM engineers
	}
}