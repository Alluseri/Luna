using Alluseri.Luna.Internals;
using Alluseri.Luna.Utils;
using System;
using System.IO;

namespace Alluseri.Luna.Abstract.Bytecode;

public class InsnLoadFloat : Instruction {
	public ushort Slot;

	public InsnLoadFloat(int Slot) : this((ushort) Slot) { }
	public InsnLoadFloat(uint Slot) : this((ushort) Slot) { }
	public InsnLoadFloat(short Slot) : this((ushort) Slot) { }
	public InsnLoadFloat(ushort Slot) {
		this.Slot = Slot;
	}

	public override void Write(Stream Stream, InternalClass Class) {
		if (Slot <= 3) {
			Stream.Write(Opcode.FLoad_0, Slot);
		} else if (Slot > byte.MaxValue) {
			Stream.Write(Opcode.Wide);
			Stream.Write(Opcode.FLoad);
			Stream.Write(Slot);
		} else {
			Stream.Write(Opcode.FLoad);
			Stream.Write((byte) Slot);
		}
	}

	public override string ToString() => $"load.f {Slot}";
	public override int GetHashCode() => HashCode.Combine(nameof(InsnLoadFloat));
	public override bool Equals(object? Other) => Other is InsnLoadFloat I && I.Slot == Slot;
}