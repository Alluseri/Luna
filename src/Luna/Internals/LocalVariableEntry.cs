using Alluseri.Luna.Utils;
using System;
using System.IO;

namespace Alluseri.Luna.Internals;

public class LocalVariableEntry {
	public readonly ushort Start;
	public readonly ushort Length;
	public readonly ushort NameIndex;
	public readonly ushort DescriptorIndex;
	public readonly ushort FrameIndex;


	public LocalVariableEntry(ushort Start, ushort Length, ushort NameIndex, ushort DescriptorIndex, ushort FrameIndex) {
		this.Start = Start;
		this.Length = Length;
		this.NameIndex = NameIndex;
		this.DescriptorIndex = DescriptorIndex;
		this.FrameIndex = FrameIndex;
	}

	public string GetName(ConstantPool Pool) => (string) (ConstantUtf8) Pool[NameIndex];
	public string GetDescriptor(ConstantPool Pool) => (string) (ConstantUtf8) Pool[DescriptorIndex];

	public override int GetHashCode() => HashCode.Combine(Start, Length, NameIndex, DescriptorIndex, FrameIndex);
	public override bool Equals(object? Object) => Object is LocalVariableEntry LVE && LVE.Start == Start && LVE.Length == Length && LVE.NameIndex == NameIndex && LVE.DescriptorIndex == DescriptorIndex && LVE.FrameIndex == FrameIndex;
	public override string ToString() => $"{{ LocalVariableEntry #{NameIndex}, #{DescriptorIndex} at [{Start}, {Start + Length}):Slot{FrameIndex} }}";

	public void Write(Stream Stream) {
		Stream.Write(Start);
		Stream.Write(Length);
		Stream.Write(NameIndex);
		Stream.Write(DescriptorIndex);
		Stream.Write(FrameIndex);
	}
}