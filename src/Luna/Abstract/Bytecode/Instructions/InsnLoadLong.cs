using Alluseri.Luna.Internals;
using Alluseri.Luna.Utils;
using System;
using System.IO;

namespace Alluseri.Luna.Abstract.Bytecode;

public class InsnLoadLong : Instruction {
	public ushort Slot;

	public InsnLoadLong(int Slot) : this((ushort) Slot) { }
	public InsnLoadLong(uint Slot) : this((ushort) Slot) { }
	public InsnLoadLong(short Slot) : this((ushort) Slot) { }
	public InsnLoadLong(ushort Slot) {
		this.Slot = Slot;
	}

	public override void Write(Stream Stream, InternalClass Class) {
		if (Slot <= 3) {
			Stream.Write(Opcode.LLoad_0, Slot);
		} else if (Slot > byte.MaxValue) {
			Stream.Write(Opcode.Wide);
			Stream.Write(Opcode.LLoad);
			Stream.Write(Slot);
		} else {
			Stream.Write(Opcode.LLoad);
			Stream.Write((byte) Slot);
		}
	}

	public override string ToString() => $"load.l {Slot}";
	public override int GetHashCode() => HashCode.Combine(nameof(InsnLoadLong));
	public override bool Equals(object? Other) => Other is InsnLoadLong I && I.Slot == Slot;
}