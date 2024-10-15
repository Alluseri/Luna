using Alluseri.Luna.Exceptions;
using Alluseri.Luna.Utils;
using System;
using System.IO;

namespace Alluseri.Luna.Internals;

public class SameStackFrame : StackMapFrame {
	public readonly bool Extended;

	public SameStackFrame(ushort OffsetDelta) : base(OffsetDelta) { Extended = OffsetDelta > 63; }

	/*public override (VerificationType[] Locals, VerificationType[] Stack) Emulate()
	=> (Previous ?? throw new EmulationException("Cannot emulate a SameStackFrame due to there being no previous frame.")).Emulate();*/

	public override int Size => Extended ? 3 : 1;

	public override int GetHashCode() => HashCode.Combine(OffsetDelta);
	public override bool Equals(object? Object) => Object is SameStackFrame SF && SF.OffsetDelta == OffsetDelta;
	public override string ToString() => $"{{ SameStackFrame +{((uint) OffsetDelta) + 1} }}";

	public static SameStackFrame? Parse(Stream Stream, byte Tag) {
		if (Tag == 251)
			return Stream.ReadUShort(out ushort OffsetDelta) ? new SameStackFrame(OffsetDelta) : null;
		else
			return new SameStackFrame(Tag);
	}

	public override void Write(Stream Stream) {
		if (Extended) {
			Stream.WriteByte(251);
			Stream.Write(OffsetDelta);
		} else {
			Stream.WriteByte((byte) OffsetDelta);
		}
	}
}