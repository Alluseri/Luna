using Alluseri.Luna.Internals;
using Alluseri.Luna.Utils;
using System;
using System.IO;

namespace Alluseri.Luna.Abstract.Bytecode;

public class InsnStoreInteger : Instruction {
	public ushort Slot;

	public InsnStoreInteger(int Slot) : this((ushort) Slot) { }
	public InsnStoreInteger(uint Slot) : this((ushort) Slot) { }
	public InsnStoreInteger(short Slot) : this((ushort) Slot) { }
	public InsnStoreInteger(ushort Slot) {
		this.Slot = Slot;
	}

	public override void Write(Stream Stream, InternalClass Class) {
		if (Slot <= 3) {
			Stream.Write(Opcode.IStore_0, Slot);
		} else if (Slot > byte.MaxValue) {
			Stream.Write(Opcode.Wide);
			Stream.Write(Opcode.IStore);
			Stream.Write(Slot);
		} else {
			Stream.Write(Opcode.IStore);
			Stream.Write((byte) Slot);
		}
	}

	public override string ToString() => $"store.i {Slot}";
	public override int GetHashCode() => HashCode.Combine(nameof(InsnStoreInteger));
	public override bool Equals(object? Other) => Other is InsnStoreInteger I && I.Slot == Slot;
}