using Alluseri.Luna.Utils;
using System;
using System.IO;

namespace Alluseri.Luna.Internals;

public class LocalVariableTypeEntry {
	public readonly ushort Start;
	public readonly ushort Length;
	public readonly ushort NameIndex;
	public readonly ushort SignatureIndex;
	public readonly ushort FrameIndex;

	public LocalVariableTypeEntry(Stream Stream) {
		Start = Stream.ReadUShort();
		Length = Stream.ReadUShort();
		NameIndex = Stream.ReadUShort();
		SignatureIndex = Stream.ReadUShort();
		FrameIndex = Stream.ReadUShort();
	}
	public LocalVariableTypeEntry(ushort Start, ushort Length, ushort NameIndex, ushort SignatureIndex, ushort FrameIndex) {
		this.Start = Start;
		this.Length = Length;
		this.NameIndex = NameIndex;
		this.SignatureIndex = SignatureIndex;
		this.FrameIndex = FrameIndex;
	}

	public string GetName(ConstantPool Pool) => (string) (ConstantUtf8) Pool[NameIndex];
	public string GetSignature(ConstantPool Pool) => (string) (ConstantUtf8) Pool[SignatureIndex];

	public override int GetHashCode() => HashCode.Combine(Start, Length, NameIndex, SignatureIndex, FrameIndex);
	public override bool Equals(object? Object) => Object is LocalVariableTypeEntry LVTE && LVTE.Start == Start && LVTE.Length == Length && LVTE.NameIndex == NameIndex && LVTE.SignatureIndex == SignatureIndex && LVTE.FrameIndex == FrameIndex;
	public override string ToString() => $"{{ LocalVariableTypeEntry #{NameIndex}, #{SignatureIndex} at [{Start}, {Start + Length}):Slot{FrameIndex} }}";

}