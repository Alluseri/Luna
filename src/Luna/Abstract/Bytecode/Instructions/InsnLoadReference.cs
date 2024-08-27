using Alluseri.Luna.Internals;
using Alluseri.Luna.Utils;
using System;
using System.IO;

namespace Alluseri.Luna.Abstract.Bytecode;

public class InsnLoadReference : Instruction {
	public ushort Slot;

	public InsnLoadReference(int Slot) : this((ushort) Slot) { }
	public InsnLoadReference(uint Slot) : this((ushort) Slot) { }
	public InsnLoadReference(short Slot) : this((ushort) Slot) { }
	public InsnLoadReference(ushort Slot) {
		this.Slot = Slot;
	}

	public override void Write(Stream Stream, InternalClass Class) {
		if (Slot <= 3) {
			Stream.Write(Opcode.ALoad_0, Slot);
		} else if (Slot > byte.MaxValue) {
			Stream.Write(Opcode.Wide);
			Stream.Write(Opcode.ALoad);
			Stream.Write(Slot);
		} else {
			Stream.Write(Opcode.ALoad);
			Stream.Write((byte) Slot);
		}
	}

	public override string ToString() => $"load.a {Slot}";
	public override int GetHashCode() => HashCode.Combine(nameof(InsnLoadReference));
	public override bool Equals(object? Other) => Other is InsnLoadReference I && I.Slot == Slot;
}