using Alluseri.Luna.Internals;
using Alluseri.Luna.Utils;
using System;
using System.IO;

namespace Alluseri.Luna.Abstract.Bytecode;

public class InsnStoreLong : Instruction {
	public ushort Slot;

	public InsnStoreLong(int Slot) : this((ushort) Slot) { }
	public InsnStoreLong(uint Slot) : this((ushort) Slot) { }
	public InsnStoreLong(short Slot) : this((ushort) Slot) { }
	public InsnStoreLong(ushort Slot) {
		this.Slot = Slot;
	}

	public override void Write(Stream Stream, InternalClass Class) {
		if (Slot <= 3) {
			Stream.Write(Opcode.LStore_0, Slot);
		} else if (Slot > byte.MaxValue) {
			Stream.Write(Opcode.Wide);
			Stream.Write(Opcode.LStore);
			Stream.Write(Slot);
		} else {
			Stream.Write(Opcode.LStore);
			Stream.Write((byte) Slot);
		}
	}

	public override string ToString() => $"store.l {Slot}";
	public override int GetHashCode() => HashCode.Combine(nameof(InsnStoreLong));
	public override bool Equals(object? Other) => Other is InsnStoreLong I && I.Slot == Slot;
}