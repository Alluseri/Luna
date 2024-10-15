using Alluseri.Luna.Internals;
using Alluseri.Luna.Utils;
using System;
using System.IO;

namespace Alluseri.Luna.Bytecode;

public class InsnStoreRef : AbstractLocalsInstruction {
	public InsnStoreRef(short Slot) : this((ushort) Slot) { }
	public InsnStoreRef(ushort Slot) : base(Slot) { }

	protected override Opcode SmallOpcode => Opcode.AStore_0;
	protected override Opcode LargeOpcode => Opcode.AStore;

	public override string ToString() => $"store.a {Slot}";
}