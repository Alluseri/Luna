using Alluseri.Luna.Internals;
using Alluseri.Luna.Utils;
using System;
using System.IO;

namespace Alluseri.Luna.Abstract.Bytecode;

public class InsnStoreRef : AbstractLocalsInstruction {
	public InsnStoreRef(short Slot) : this((ushort) Slot) { }
	public InsnStoreRef(ushort Slot) : base(Slot) { }

	protected override Opcode SmallOpcode => Opcode.ALoad_0;
	protected override Opcode LargeOpcode => Opcode.ALoad;

	public override string ToString() => $"store.a {Slot}";
}