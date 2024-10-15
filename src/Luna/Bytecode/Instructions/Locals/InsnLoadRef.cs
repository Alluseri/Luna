using Alluseri.Luna.Internals;
using Alluseri.Luna.Utils;
using System;
using System.IO;

namespace Alluseri.Luna.Bytecode;

public class InsnLoadRef : AbstractLocalsInstruction {
	public InsnLoadRef(short Slot) : this((ushort) Slot) { }
	public InsnLoadRef(ushort Slot) : base(Slot) { }

	protected override Opcode SmallOpcode => Opcode.ALoad_0;
	protected override Opcode LargeOpcode => Opcode.ALoad;

	public override string ToString() => $"load.a {Slot}";
}