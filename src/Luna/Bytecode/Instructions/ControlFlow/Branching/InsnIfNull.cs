using Alluseri.Luna.Internals;
using Alluseri.Luna.Utils;
using System.IO;

namespace Alluseri.Luna.Bytecode;

public class InsnIfNullBranch : AbstractSingleBranchInstruction {
	public override bool Conditional => true;

	internal InsnIfNullBranch(int Address, int Offset) : base(null!) {
		TargetLocation = Address + Offset;
	}
	public InsnIfNullBranch(Label Lab) : base(Lab) { }

	internal override void Checkout(CodeBuilder Builder, int Address) {
		Size = 3;
	}

	internal override void Write(Stream Stream, CodeBuilder Class, int Address) {
		checked {
			Stream.Write(Opcode.IfNull);
			Stream.Write((short) (Target.Location - Address)); // yay woo
		}
	}

	public override string ToString() => $"ifnull {Target.Name}";
}
