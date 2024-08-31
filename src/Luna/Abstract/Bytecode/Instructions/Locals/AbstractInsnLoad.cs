using Alluseri.Luna.Internals;
using Alluseri.Luna.Utils;
using System.IO;

namespace Alluseri.Luna.Abstract.Bytecode;

public abstract class AbstractLocalsInstruction : Instruction {
	private ushort _Slot;
	public ushort Slot {
		get => _Slot;
		set => _Size = (_Slot = value) switch {
			<= 3 => 1,
			<= byte.MaxValue => 2,
			_ => 3
		};
	}

	private int _Size;
	public override int Size => _Size;

	protected abstract Opcode SmallOpcode { get; }
	protected abstract Opcode LargeOpcode { get; }

	public AbstractLocalsInstruction(ushort Slot) {
		this.Slot = Slot;
	}

	public override void Write(Stream Stream, InternalClass Class) {
		switch (_Size) {
			case 1:
			Stream.Write(SmallOpcode, Slot);
			break;
			case 2:
			Stream.Write(LargeOpcode);
			Stream.Write((byte) Slot);
			break;
			case 3:
			default:
			Stream.Write(Opcode.Wide);
			Stream.Write(LargeOpcode);
			Stream.Write(Slot);
			break;
		}
	}
}