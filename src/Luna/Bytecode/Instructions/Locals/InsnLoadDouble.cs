using Alluseri.Luna.Internals;
using Alluseri.Luna.Utils;
using System;
using System.IO;

namespace Alluseri.Luna.Bytecode;

public class InsnLoadDouble : AbstractLocalsInstruction {
	public InsnLoadDouble(short Slot) : this((ushort) Slot) { }
	public InsnLoadDouble(ushort Slot) : base(Slot) { }

	protected override Opcode SmallOpcode => Opcode.DLoad_0;
	protected override Opcode LargeOpcode => Opcode.DLoad;

	public override string ToString() => $"load.d {Slot}";
}