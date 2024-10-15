using Alluseri.Luna.Internals;
using Alluseri.Luna.Utils;
using System;
using System.IO;

namespace Alluseri.Luna.Bytecode;

public class InsnStoreFloat : AbstractLocalsInstruction {
	public InsnStoreFloat(short Slot) : this((ushort) Slot) { }
	public InsnStoreFloat(ushort Slot) : base(Slot) { }

	protected override Opcode SmallOpcode => Opcode.FStore_0;
	protected override Opcode LargeOpcode => Opcode.FStore;

	public override string ToString() => $"store.f {Slot}";
}