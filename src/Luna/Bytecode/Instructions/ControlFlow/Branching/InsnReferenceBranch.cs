using Alluseri.Luna.Internals;
using Alluseri.Luna.Utils;
using System.IO;

namespace Alluseri.Luna.Bytecode;

public class InsnReferenceBranch : AbstractSingleBranchInstruction {
	public override bool Conditional => true;

	public ReferenceBranchCondition Condition;

	internal InsnReferenceBranch(ReferenceBranchCondition Condition, int Address, int Offset) : base(null!) {
		this.Condition = Condition;
		TargetLocation = Address + Offset;
	}
	public InsnReferenceBranch(ReferenceBranchCondition Condition, Label Lab) : base(Lab) {
		this.Condition = Condition;
	}

	internal override void Checkout(ConstantPool Pool) {
		Size = 3;
	}

	internal override void Write(Stream Stream, CodeBuilder Class, int Address) {
		checked {
			Stream.Write(Opcode.If_ACmpEqual, (uint) Condition);
			Stream.Write((short) (Target.Location - Address)); // yay woo
		}
	}

	public override string ToString() => $"if{Condition.GetInstructionSign()}.i {Target.Name}";
}
