using Alluseri.Luna.Utils;
using System.IO;
using System.Reflection.Emit;

namespace Alluseri.Luna.Bytecode;

public class InsnIncrementInteger : Instruction {
	private ushort _Slot;
	public ushort Slot {
		get => _Slot;
		set => Size = (_Slot = value) <= byte.MaxValue ? 3 : 5;
	}
	public short Addend;

	public InsnIncrementInteger(ushort Slot, sbyte Addend) {
		this.Slot = Slot;
		this.Addend = Addend;
	}
	public InsnIncrementInteger(ushort Slot, short Addend) {
		this.Slot = Slot;
		this.Addend = Addend;
	}

	internal override void Write(Stream Stream, CodeBuilder Builder) {
		switch (Size) {
			case 3:
			Stream.Write(Opcode.IInc);
			Stream.Write((byte) Slot);
			Stream.Write(Addend);
			break;
			case 5:
			default:
			Stream.Write(Opcode.Wide);
			Stream.Write(Opcode.IInc);
			Stream.Write(Slot);
			Stream.Write(Addend);
			break;
		}
	}

	public override string ToString() => $"inc.i {Slot} {Addend:+#;-#;0}";
}