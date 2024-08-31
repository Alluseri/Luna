using Alluseri.Luna.Utils;
using System;
using System.IO;

namespace Alluseri.Luna.Internals;

public readonly record struct ExceptionHandler(ushort Start, ushort End, ushort Handler, ushort CatchTypeIndex) {
	public ConstantClass GetCatchType(ConstantPool Pool) => (ConstantClass) Pool[CatchTypeIndex];

	public override int GetHashCode() => HashCode.Combine(Start, End, Handler, CatchTypeIndex);
	public override string ToString() => $"{{ ExceptionTable [{Start}, {End}):{Handler} catches {CatchTypeIndex} }}";

	public void Write(Stream Stream) {
		Stream.Write(Start);
		Stream.Write(End);
		Stream.Write(Handler);
		Stream.Write(CatchTypeIndex);
	}
}