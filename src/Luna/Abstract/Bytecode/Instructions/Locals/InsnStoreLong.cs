using Alluseri.Luna.Internals;
using Alluseri.Luna.Utils;
using System;
using System.IO;

namespace Alluseri.Luna.Abstract.Bytecode;

public class InsnStoreLong : AbstractLocalsInstruction {
	public InsnStoreLong(short Slot) : this((ushort) Slot) { }
	public InsnStoreLong(ushort Slot) : base(Slot) { }

	protected override Opcode SmallOpcode => Opcode.LLoad_0;
	protected override Opcode LargeOpcode => Opcode.LLoad;

	public override string ToString() => $"store.l {Slot}";
}