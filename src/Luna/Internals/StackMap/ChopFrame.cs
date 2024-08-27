using Alluseri.Luna.Exceptions;
using Alluseri.Luna.Utils;
using System;
using System.IO;

namespace Alluseri.Luna.Internals;

public class ChopFrame : StackMapFrame {
	public readonly int ChopLocals;

	public ChopFrame(ushort OffsetDelta, int ChopLocals, LinkableList<StackMapFrame>? Root = null) : base(OffsetDelta, Root) {
		this.ChopLocals = Assert.RangeII(nameof(ChopLocals), ChopLocals, 1, 3);
	}

	public override (VerificationType[] Locals, VerificationType[] Stack) Emulate()
	=> (
		(Previous ?? throw new EmulationException("Cannot emulate a ChopFrame due to there being no previous frame.")).Emulate().Locals[0..^ChopLocals],
		Array.Empty<VerificationType>()
	);

	public override int Size => 3;

	public override int GetHashCode() => HashCode.Combine(OffsetDelta, ChopLocals);
	public override bool Equals(object? Object) => Object is ChopFrame CF && CF.OffsetDelta == OffsetDelta && CF.ChopLocals == ChopLocals;
	public override string ToString() => $"{{ ChopFrame +{((uint) OffsetDelta) + 1}: Locals [0..^{ChopLocals}], Stack [] }}";

	public static ChopFrame? Parse(Stream Stream, byte Tag, LinkableList<StackMapFrame>? Root = null)
	=> Stream.ReadUShort(out ushort OffsetDelta) ? new ChopFrame(OffsetDelta, Tag - 247, Root) : null;

	public override void Write(Stream Stream) {
		Stream.WriteByte((byte) (247 + ChopLocals));
		Stream.Write(OffsetDelta);
	}
}