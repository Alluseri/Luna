using Alluseri.Luna.Internals;
using Alluseri.Luna.Utils;
using System;
using System.IO;

namespace Alluseri.Luna.Abstract.Bytecode;

public class InsnStoreFloat : AbstractLocalsInstruction {
	public InsnStoreFloat(short Slot) : this((ushort) Slot) { }
	public InsnStoreFloat(ushort Slot) : base(Slot) { }

	protected override Opcode SmallOpcode => Opcode.FLoad_0;
	protected override Opcode LargeOpcode => Opcode.FLoad;

	public override string ToString() => $"store.f {Slot}";
}