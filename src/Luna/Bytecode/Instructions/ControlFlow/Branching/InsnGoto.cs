using Alluseri.Luna.Internals;
using Alluseri.Luna.Utils;
using System.IO;

namespace Alluseri.Luna.Bytecode;

public class InsnGoto : AbstractSingleBranchInstruction {
	public override bool Conditional => false;

	internal InsnGoto(int Address, int Offset) : base(null!) {
		TargetLocation = Address + Offset;
	}
	public InsnGoto(Label Lab) : base(Lab) { }

	internal override void Checkout(ConstantPool Pool) {
		Size = 5;
	}

	internal override void Write(Stream Stream, CodeBuilder Class, int Address) {
		Stream.Write(Opcode.Goto_W);
		Stream.Write(Target.Location - Address); // am sowy
	}

	public override string ToString() => $"goto_w {Target.Name}";
}