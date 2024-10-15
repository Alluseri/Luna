using Alluseri.Luna.Exceptions;
using Alluseri.Luna.Utils;
using System.IO;
using System.Runtime.CompilerServices;

namespace Alluseri.Luna.Internals;

public abstract class StackMapFrame : ISizeable {
	public readonly ushort OffsetDelta; // DESIGN: I think it is unsafe to unreadonly this. Use Replace() instead.

	public StackMapFrame(ushort OffsetDelta) {
		this.OffsetDelta = OffsetDelta;
	}

	public abstract int Size { get; }

	public abstract override int GetHashCode();
	public abstract override bool Equals(object? obj);
	public abstract override string ToString();

	public static StackMapFrame? Parse(Stream Stream) {
		byte Tag = (byte) Stream.ReadByte();
		if (Tag <= 63 || Tag == 251)
			return SameStackFrame.Parse(Stream, Tag);
		else if ((Tag >= 64 && Tag <= 127) || Tag == 247)
			return SameLocalsOneStackItemFrame.Parse(Stream, Tag);
		else if (Tag >= 248 && Tag <= 250)
			return ChopFrame.Parse(Stream, Tag);
		else if (Tag >= 252 && Tag <= 254)
			return AppendFrame.Parse(Stream, Tag);
		else if (Tag == 255)
			return FullFrame.Parse(Stream, Tag);
		else
			return null;
	}

	public abstract void Write(Stream Stream);
}