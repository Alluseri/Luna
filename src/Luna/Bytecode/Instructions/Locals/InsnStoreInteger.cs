using Alluseri.Luna.Internals;
using Alluseri.Luna.Utils;
using System;
using System.IO;

namespace Alluseri.Luna.Bytecode;

public class InsnStoreInteger : AbstractLocalsInstruction {
	public InsnStoreInteger(short Slot) : this((ushort) Slot) { }
	public InsnStoreInteger(ushort Slot) : base(Slot) { }

	protected override Opcode SmallOpcode => Opcode.IStore_0;
	protected override Opcode LargeOpcode => Opcode.IStore;

	public override string ToString() => $"store.i {Slot}";
}