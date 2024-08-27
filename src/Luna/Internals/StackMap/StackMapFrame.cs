using Alluseri.Luna.Exceptions;
using Alluseri.Luna.Utils;
using System.IO;
using System.Runtime.CompilerServices;

namespace Alluseri.Luna.Internals;

// TODO: Unlink, since SMT doesn't work like I thought it does. E.g. ChopFrame will reference the previous frame in the stack, not in the table.
public abstract class StackMapFrame : Linkable<StackMapFrame>, ISizeable {
	public readonly ushort OffsetDelta;

	public StackMapFrame(ushort OffsetDelta, LinkableList<StackMapFrame>? Root = null) : base(Root) {
		this.OffsetDelta = OffsetDelta;
	}

	public abstract int Size { get; }

	public abstract (VerificationType[] Locals, VerificationType[] Stack) Emulate(); // DESIGN: Turn this to a record?

	public FullFrame AsFull() {
		(VerificationType[] Locals, VerificationType[] Stack) = Emulate();
		return new(OffsetDelta, Locals, Stack);
	}

	public abstract override int GetHashCode();
	public abstract override bool Equals(object? obj);
	public abstract override string ToString();

	public static StackMapFrame? Parse(Stream Stream, LinkableList<StackMapFrame>? Root = null) {
		byte Tag = (byte) Stream.ReadByte();
		if (Tag <= 63 || Tag == 251)
			return SameStackFrame.Parse(Stream, Tag, Root);
		else if ((Tag >= 64 && Tag <= 127) || Tag == 247)
			return SameLocalsOneStackItemFrame.Parse(Stream, Tag, Root);
		else if (Tag >= 248 && Tag <= 250)
			return ChopFrame.Parse(Stream, Tag, Root);
		else if (Tag >= 252 && Tag <= 254)
			return AppendFrame.Parse(Stream, Tag, Root);
		else if (Tag == 255)
			return FullFrame.Parse(Stream, Tag, Root);
		else
			return null;
	}

	public abstract void Write(Stream Stream);
}