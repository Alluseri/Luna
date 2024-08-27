using Alluseri.Luna.Exceptions;
using Alluseri.Luna.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Alluseri.Luna.Internals;

public class MethodInfo {
	public readonly MethodAccessFlags AccessFlags;
	public readonly ushort NameIndex;
	public readonly ushort DescriptorIndex;
	public readonly AttributeInfo[] Attributes;

	public MethodInfo(Stream Stream, ConstantPool Pool) {
		AccessFlags = (MethodAccessFlags) Stream.ReadUShort();
		NameIndex = Stream.ReadUShort();
		DescriptorIndex = Stream.ReadUShort();
		GU.New(out Attributes, Stream.ReadUShort());
		for (ushort i = 0; i < Attributes.Length; i++) {
			Attributes[i] = AttributeInfo.Parse(Stream, Pool) ?? throw new ClassFileException($"Cannot parse an incomplete or otherwise malformed class file: ran out of bytes while reading the cpool index of attribute #{i} (owned by a MethodInfo).");
		}
	}

	public MethodInfo(MethodAccessFlags AccessFlags, ushort NameIndex, ushort DescriptorIndex, AttributeInfo[] Attributes) {
		this.AccessFlags = AccessFlags;
		this.NameIndex = NameIndex;
		this.DescriptorIndex = DescriptorIndex;
		this.Attributes = Attributes;
	}

	public string GetName(ConstantPool Pool) => ((ConstantUtf8) Pool[NameIndex]).Value;
	public string GetDescriptor(ConstantPool Pool) => ((ConstantUtf8) Pool[DescriptorIndex]).Value;

	public override int GetHashCode() => HashCode.Combine(AccessFlags, NameIndex, DescriptorIndex, Attributes);
	public override bool Equals(object? Object) => Object is MethodInfo CFI && CFI.AccessFlags == AccessFlags && CFI.DescriptorIndex == DescriptorIndex && CFI.NameIndex == NameIndex && CFI.Attributes.SequenceEqual(Attributes);
	public override string ToString() => $"{{ Method #{NameIndex}, #{DescriptorIndex} w/ Access 0x{AccessFlags:X}, Attributes [ {GU.ToString(Attributes)} ] }}";

	public void Write(Stream Stream, ConstantPool Pool) {
		Stream.Write((ushort) AccessFlags);
		Stream.Write(NameIndex);
		Stream.Write(DescriptorIndex);
		Stream.Write((ushort) Attributes.Length);
		foreach (AttributeInfo Ai in Attributes) {
			Ai.Write(Stream, Pool);
		}
	}
}