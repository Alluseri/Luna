using Alluseri.Luna.Utils;
using System;
using System.IO;
using System.Linq;

namespace Alluseri.Luna.Internals;

public class FullFrame : StackMapFrame {
	public readonly VerificationType[] Locals;
	public readonly VerificationType[] Stack;

	public FullFrame(ushort OffsetDelta, VerificationType[] Locals, VerificationType[] Stack, LinkableList<StackMapFrame>? Root = null) : base(OffsetDelta, Root) {
		this.Locals = Locals;
		this.Stack = Stack;
	}

	public override (VerificationType[] Locals, VerificationType[] Stack) Emulate()
	=> (Locals, Stack);

	public override int Size => 7 + GU.GetSize(Locals) + GU.GetSize(Stack);

	public override int GetHashCode() => HashCode.Combine(OffsetDelta, Locals, Stack);
	public override bool Equals(object? Object) => Object is FullFrame FF && FF.OffsetDelta == OffsetDelta && FF.Stack.SequenceEqual(Stack) && FF.Locals.SequenceEqual(Locals);
	public override string ToString() => $"{{ FullFrame +{((uint) OffsetDelta) + 1}: Locals [ {GU.ToString(Locals)} ], Stack [ {GU.ToString(Stack)} ] }}";

	public static FullFrame? Parse(Stream Stream, byte Tag, LinkableList<StackMapFrame>? Root = null) { // DESIGN: Keep the unused Tag or override VerificationType with new? Alternative idea: ParseFrame
		if (!Stream.ReadUShort(out ushort OffsetDelta) || !Stream.ReadUShort(out ushort NumberOfLocals))
			return null;

		VerificationType[] Locals = new VerificationType[NumberOfLocals];
		for (ushort i = 0; i < Locals.Length; i++) {
			if ((Locals[i] = VerificationType.Parse(Stream)!) == null)
				return null;
		}

		if (!Stream.ReadUShort(out ushort NumberOfStackItems))
			return null;

		VerificationType[] Stack = new VerificationType[NumberOfStackItems];
		for (ushort i = 0; i < Stack.Length; i++) {
			if ((Stack[i] = VerificationType.Parse(Stream)!) == null)
				return null;
		}

		return new FullFrame(OffsetDelta, Locals, Stack, Root);
	}

	public override void Write(Stream Stream) {
		Stream.WriteByte(255);
		Stream.Write(OffsetDelta);
		Stream.Write((ushort) Locals.Length);
		foreach (VerificationType Vt in Locals)
			Vt.Write(Stream);
		Stream.Write((ushort) Stack.Length);
		foreach (VerificationType Vt in Stack)
			Vt.Write(Stream);
	}
}