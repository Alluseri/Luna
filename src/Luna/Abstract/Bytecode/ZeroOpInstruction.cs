using Alluseri.Luna.Abstract.Bytecode;
using Alluseri.Luna.Internals;
using Alluseri.Luna.Utils;
using System.IO;

public abstract class ZeroOpInstruction : Instruction {
	public override int Size => 1;

	private readonly Opcode Opcode;
	private readonly string Mnemonic;

	public ZeroOpInstruction(Opcode Opcode, string Mnemonic) {
		this.Opcode = Opcode;
		this.Mnemonic = Mnemonic;
	}

	public override void Write(Stream Stream, InternalClass Class) {
		Stream.Write(Opcode);
	}

	public override string ToString() => Mnemonic;
}