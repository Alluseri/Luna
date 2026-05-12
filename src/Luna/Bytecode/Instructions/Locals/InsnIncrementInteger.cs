using Alluseri.Luna.Utils;
using System.IO;
using System.Reflection.Emit;

namespace Alluseri.Luna.Bytecode;

public class InsnIncrementInteger : Instruction {
	private ushort _Slot;
	private short _Addend;

	public ushort Slot {
		get => _Slot;
		set => Size = GetSize(_Slot = value, Addend);
	}
	public short Addend {
		get => _Addend;
		set => Size = GetSize(Slot, _Addend = value);
	}

	private static int GetSize(ushort Slot, short Addend) {
		return (Slot > byte.MaxValue || Addend > byte.MaxValue) ? 6 : 3;
	}

	public InsnIncrementInteger(ushort Slot, sbyte Addend) {
		this.Slot = Slot;
		this.Addend = Addend;
	}
	public InsnIncrementInteger(ushort Slot, short Addend) {
		this.Slot = Slot;
		this.Addend = Addend;
	}

	internal override void Write(Stream Stream, CodeBuilder Builder, int Address) {
		switch (Size) {
			case 3:
			Stream.Write(Opcode.IInc);
			Stream.Write((byte) Slot);
			Stream.Write((byte) Addend);
			break;
			case 6:
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