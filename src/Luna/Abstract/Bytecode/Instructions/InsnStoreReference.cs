using Alluseri.Luna.Internals;
using Alluseri.Luna.Utils;
using System;
using System.IO;

namespace Alluseri.Luna.Abstract.Bytecode;

public class InsnStoreReference : Instruction {
	public ushort Slot;

	public InsnStoreReference(int Slot) : this((ushort) Slot) { }
	public InsnStoreReference(uint Slot) : this((ushort) Slot) { }
	public InsnStoreReference(short Slot) : this((ushort) Slot) { }
	public InsnStoreReference(ushort Slot) {
		this.Slot = Slot;
	}

	public override void Write(Stream Stream, InternalClass Class) {
		if (Slot <= 3) {
			Stream.Write(Opcode.AStore_0, Slot);
		} else if (Slot > byte.MaxValue) {
			Stream.Write(Opcode.Wide);
			Stream.Write(Opcode.AStore);
			Stream.Write(Slot);
		} else {
			Stream.Write(Opcode.AStore);
			Stream.Write((byte) Slot);
		}
	}

	public override string ToString() => $"store.a {Slot}";
	public override int GetHashCode() => HashCode.Combine(nameof(InsnStoreReference));
	public override bool Equals(object? Other) => Other is InsnStoreReference I && I.Slot == Slot;
}