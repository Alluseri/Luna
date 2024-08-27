using Alluseri.Luna.Internals;
using Alluseri.Luna.Utils;
using System;
using System.IO;

namespace Alluseri.Luna.Abstract.Bytecode;

public class InsnStoreDouble : Instruction {
	public ushort Slot;

	public InsnStoreDouble(int Slot) : this((ushort) Slot) { }
	public InsnStoreDouble(uint Slot) : this((ushort) Slot) { }
	public InsnStoreDouble(short Slot) : this((ushort) Slot) { }
	public InsnStoreDouble(ushort Slot) {
		this.Slot = Slot;
	}

	public override void Write(Stream Stream, InternalClass Class) {
		if (Slot <= 3) {
			Stream.Write(Opcode.DStore_0, Slot);
		} else if (Slot > byte.MaxValue) {
			Stream.Write(Opcode.Wide);
			Stream.Write(Opcode.DStore);
			Stream.Write(Slot);
		} else {
			Stream.Write(Opcode.DStore);
			Stream.Write((byte) Slot);
		}
	}

	public override string ToString() => $"store.d {Slot}";
	public override int GetHashCode() => HashCode.Combine(nameof(InsnStoreDouble));
	public override bool Equals(object? Other) => Other is InsnStoreDouble I && I.Slot == Slot;
}