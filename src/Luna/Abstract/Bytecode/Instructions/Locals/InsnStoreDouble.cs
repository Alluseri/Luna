using Alluseri.Luna.Internals;
using Alluseri.Luna.Utils;
using System;
using System.IO;

namespace Alluseri.Luna.Abstract.Bytecode;

public class InsnStoreDouble : AbstractLocalsInstruction {
	public InsnStoreDouble(short Slot) : this((ushort) Slot) { }
	public InsnStoreDouble(ushort Slot) : base(Slot) { }

	protected override Opcode SmallOpcode => Opcode.DLoad_0;
	protected override Opcode LargeOpcode => Opcode.DLoad;

	public override string ToString() => $"store.d {Slot}";
}