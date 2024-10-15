using Alluseri.Luna.Internals;
using Alluseri.Luna.Utils;
using System;
using System.IO;

namespace Alluseri.Luna.Bytecode;

public class InsnLoadInteger : AbstractLocalsInstruction {
	public InsnLoadInteger(short Slot) : this((ushort) Slot) { }
	public InsnLoadInteger(ushort Slot) : base(Slot) { }

	protected override Opcode SmallOpcode => Opcode.ILoad_0;
	protected override Opcode LargeOpcode => Opcode.ILoad;

	public override string ToString() => $"load.i {Slot}";
}