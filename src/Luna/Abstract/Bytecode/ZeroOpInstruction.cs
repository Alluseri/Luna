using Alluseri.Luna.Abstract.Bytecode;
using Alluseri.Luna.Internals;
using Alluseri.Luna.Utils;
using System.IO;

public abstract class ZeroOpInstruction : Instruction {
	private readonly Opcode Opcode;
	private readonly string Mnemonic;

	public ZeroOpInstruction(Opcode Opcode, string Mnemonic) : base(1) {
		this.Opcode = Opcode;
		this.Mnemonic = Mnemonic;
	}

	internal override void Write(Stream Stream, InternalClass Class) => Stream.Write(Opcode);

	public override string ToString() => Mnemonic;
}