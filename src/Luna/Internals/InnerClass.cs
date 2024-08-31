using Alluseri.Luna.Utils;
using System;
using System.IO;

namespace Alluseri.Luna.Internals;

public readonly record struct InnerClass(ushort InfoIndex, ushort OuterInfoIndex, ushort NameIndex, ushort AccessFlags) {
	public override int GetHashCode() => HashCode.Combine(InfoIndex, OuterInfoIndex, NameIndex, AccessFlags);
	public override string ToString() => $"{{ InnerClass named {NameIndex}, Info: {InfoIndex}, OuterInfo: {OuterInfoIndex}, Access Flags: 0x{AccessFlags:X4} }}"; // TODO: Proper ToString here, I might need to make a standard to follow here.

	public void Write(Stream Stream) {
		Stream.Write(InfoIndex);
		Stream.Write(OuterInfoIndex);
		Stream.Write(NameIndex);
		Stream.Write(AccessFlags);
	}
}