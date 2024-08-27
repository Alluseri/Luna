using Alluseri.Luna.Internals;
using Alluseri.Luna.Utils;
using System;
using System.IO;

namespace Alluseri.Luna.Abstract.Bytecode;

public class InsnLoadInteger : Instruction {
	public ushort Slot;

	public InsnLoadInteger(int Slot) : this((ushort) Slot) { }
	public InsnLoadInteger(uint Slot) : this((ushort) Slot) { }
	public InsnLoadInteger(short Slot) : this((ushort) Slot) { }
	public InsnLoadInteger(ushort Slot) {
		this.Slot = Slot;
	}

	public override void Write(Stream Stream, InternalClass Class) {
		if (Slot <= 3) {
			Stream.Write(Opcode.ILoad_0, Slot);
		} else if (Slot > byte.MaxValue) {
			Stream.Write(Opcode.Wide);
			Stream.Write(Opcode.ILoad);
			Stream.Write(Slot);
		} else {
			Stream.Write(Opcode.ILoad);
			Stream.Write((byte) Slot);
		}
	}

	public override string ToString() => $"load.i {Slot}";
	public override int GetHashCode() => HashCode.Combine(nameof(InsnLoadInteger));
	public override bool Equals(object? Other) => Other is InsnLoadInteger I && I.Slot == Slot;
}