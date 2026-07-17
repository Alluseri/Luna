using Alluseri.Luna.Internals;
using Alluseri.Luna.Utils;
using System.IO;

namespace Alluseri.Luna.Bytecode;

// TODO: How do we resolve this to wide or not wide if its own size decides the size of all future instructions in CodeWriter?
public class InsnGoto : AbstractSingleBranchInstruction {
	public override bool Conditional => false;

	internal InsnGoto(int Address, int Offset) : base(null!) {
		TargetLocation = Address + Offset;
	}
	public InsnGoto(Label Lab) : base(Lab) { }

	internal override void Checkout(CodeBuilder Builder, int Address) {
		Size = 5;
	}

	internal override void Write(Stream Stream, CodeBuilder Class, int Address) {
		Stream.Write(Opcode.Goto_W);
		Stream.Write(Target.Location - Address); // am sowy
	}

	public override string ToString() => $"goto_w {Target.Name}";
}