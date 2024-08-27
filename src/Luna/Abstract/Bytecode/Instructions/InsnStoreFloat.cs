using Alluseri.Luna.Internals;
using Alluseri.Luna.Utils;
using System;
using System.IO;

namespace Alluseri.Luna.Abstract.Bytecode;

public class InsnStoreFloat : Instruction {
	public ushort Slot;

	public InsnStoreFloat(int Slot) : this((ushort) Slot) { }
	public InsnStoreFloat(uint Slot) : this((ushort) Slot) { }
	public InsnStoreFloat(short Slot) : this((ushort) Slot) { }
	public InsnStoreFloat(ushort Slot) {
		this.Slot = Slot;
	}

	public override void Write(Stream Stream, InternalClass Class) {
		if (Slot <= 3) {
			Stream.Write(Opcode.FStore_0, Slot);
		} else if (Slot > byte.MaxValue) {
			Stream.Write(Opcode.Wide);
			Stream.Write(Opcode.FStore);
			Stream.Write(Slot);
		} else {
			Stream.Write(Opcode.FStore);
			Stream.Write((byte) Slot);
		}
	}

	public override string ToString() => $"store.f {Slot}";
	public override int GetHashCode() => HashCode.Combine(nameof(InsnStoreFloat));
	public override bool Equals(object? Other) => Other is InsnStoreFloat I && I.Slot == Slot;
}