using Alluseri.Luna.Exceptions;
using Alluseri.Luna.Utils;
using System;
using System.IO;

namespace Alluseri.Luna.Internals;

public class SameLocalsOneStackItemFrame : StackMapFrame {
	public readonly bool Extended = false;
	public readonly VerificationType StackItem;

	public SameLocalsOneStackItemFrame(ushort OffsetDelta, VerificationType StackItem) : base(OffsetDelta) {
		Extended = OffsetDelta > 63;
		this.StackItem = StackItem;
	}

	/*public override (VerificationType[] Locals, VerificationType[] Stack) Emulate()
	=>
	(
		(Previous ?? throw new EmulationException("Cannot emulate a SameLocalsOneStackItemFrame due to there being no previous frame.")).Emulate().Locals,
		new[] { StackItem }
	);*/

	public override int Size => (Extended ? 3 : 1) + StackItem.Size;

	public override int GetHashCode() => HashCode.Combine(OffsetDelta, StackItem);
	public override bool Equals(object? Object) => Object is SameLocalsOneStackItemFrame SF && SF.OffsetDelta == OffsetDelta && SF.StackItem.Equals(StackItem);
	public override string ToString() => $"{{ SameLocalsOneStackItemFrame +{((uint) OffsetDelta) + 1}: Stack +[ {StackItem} ] }}";

	// DESIGN: Arghhh I can't get over the thought of onelining this with out parameters

	public static SameLocalsOneStackItemFrame? Parse(Stream Stream, byte Tag) {
		if (Tag == 247) {
			if (!Stream.ReadUShort(out ushort OffsetDelta))
				return null;
			VerificationType? Vt = VerificationType.Parse(Stream);
			return Vt == null ? null : new SameLocalsOneStackItemFrame(OffsetDelta, Vt);
		} else {
			VerificationType? Vt = VerificationType.Parse(Stream);
			return Vt == null ? null : new SameLocalsOneStackItemFrame((ushort) (Tag - 64), Vt);
		}
	}

	public override void Write(Stream Stream) {
		if (Extended) {
			Stream.WriteByte(247);
			Stream.Write(OffsetDelta);
		} else {
			Stream.WriteByte((byte) (OffsetDelta + 64));
		}
		StackItem.Write(Stream);
	}
}