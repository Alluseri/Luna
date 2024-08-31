using Alluseri.Luna.Exceptions;
using Alluseri.Luna.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace Alluseri.Luna.Internals;

public class AppendFrame : StackMapFrame {
	public readonly VerificationType[] Locals;

	public AppendFrame(ushort OffsetDelta, VerificationType[] Locals, LinkableList<StackMapFrame>? Root = null) : base(OffsetDelta, Root) {
		this.Locals = Locals;
	}

	public override (VerificationType[] Locals, VerificationType[] Stack) Emulate()
	=> (
		(Previous ?? throw new EmulationException("Cannot emulate an AppendFrame due to there being no previous frame.")).Emulate().Locals.Append(Locals),
		Array.Empty<VerificationType>()
	);

	public override int Size => 3 + GU.GetSize(Locals);

	public override int GetHashCode() => HashCode.Combine(OffsetDelta, Locals);
	public override bool Equals(object? Object) => Object is AppendFrame AF && AF.OffsetDelta == OffsetDelta && AF.Locals.SequenceEqual(Locals);
	public override string ToString() => $"{{ AppendFrame +{((uint) OffsetDelta) + 1}: Locals +[ {GU.ToString(Locals)} ], Stack [] }}";

	public static AppendFrame? Parse(Stream Stream, byte Tag, LinkableList<StackMapFrame>? Root = null) {
		if (!Stream.ReadUShort(out ushort OffsetDelta))
			return null;
		VerificationType[] Locals = new VerificationType[Tag - 251];
		for (ushort i = 0; i < Locals.Length; i++) {
			if ((Locals[i] = VerificationType.Parse(Stream)!) == null)
				return null;
		}
		return new AppendFrame(OffsetDelta, Locals, Root);
	}

	public override void Write(Stream Stream) {
		Stream.WriteByte((byte) (251 + Locals.Length)); // I WILL LET YOU FIRE RIGHT THROUGH YOUR KNEE JUST IN CASE
		Stream.Write(OffsetDelta);
		foreach (VerificationType Vt in Locals)
			Vt.Write(Stream);
	}
}