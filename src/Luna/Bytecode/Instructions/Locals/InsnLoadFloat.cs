using Alluseri.Luna.Internals;
using Alluseri.Luna.Utils;
using System;
using System.IO;

namespace Alluseri.Luna.Bytecode;

public class InsnLoadFloat : AbstractLocalsInstruction {
	public InsnLoadFloat(short Slot) : this((ushort) Slot) { }
	public InsnLoadFloat(ushort Slot) : base(Slot) { }

	protected override Opcode SmallOpcode => Opcode.FLoad_0;
	protected override Opcode LargeOpcode => Opcode.FLoad;

	public override string ToString() => $"load.f {Slot}";
}