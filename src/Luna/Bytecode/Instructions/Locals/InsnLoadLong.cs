using Alluseri.Luna.Internals;
using Alluseri.Luna.Utils;
using System;
using System.IO;

namespace Alluseri.Luna.Bytecode;

public class InsnLoadLong : AbstractLocalsInstruction {
	public InsnLoadLong(short Slot) : this((ushort) Slot) { }
	public InsnLoadLong(ushort Slot) : base(Slot) { }

	protected override Opcode SmallOpcode => Opcode.LLoad_0;
	protected override Opcode LargeOpcode => Opcode.LLoad;

	public override string ToString() => $"load.l {Slot}";
}