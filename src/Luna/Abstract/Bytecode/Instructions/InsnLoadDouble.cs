using Alluseri.Luna.Internals;
using Alluseri.Luna.Utils;
using System;
using System.IO;

namespace Alluseri.Luna.Abstract.Bytecode;

public class InsnLoadDouble : Instruction {
	public ushort Slot;

	public InsnLoadDouble(int Slot) : this((ushort) Slot) { }
	public InsnLoadDouble(uint Slot) : this((ushort) Slot) { }
	public InsnLoadDouble(short Slot) : this((ushort) Slot) { }
	public InsnLoadDouble(ushort Slot) {
		this.Slot = Slot;
	}

	public override void Write(Stream Stream, InternalClass Class) {
		if (Slot <= 3) {
			Stream.Write(Opcode.DLoad_0, Slot);
		} else if (Slot > byte.MaxValue) {
			Stream.Write(Opcode.Wide);
			Stream.Write(Opcode.DLoad);
			Stream.Write(Slot);
		} else {
			Stream.Write(Opcode.DLoad);
			Stream.Write((byte) Slot);
		}
	}

	public override string ToString() => $"load.d {Slot}";
	public override int GetHashCode() => HashCode.Combine(nameof(InsnLoadDouble));
	public override bool Equals(object? Other) => Other is InsnLoadDouble I && I.Slot == Slot;
}