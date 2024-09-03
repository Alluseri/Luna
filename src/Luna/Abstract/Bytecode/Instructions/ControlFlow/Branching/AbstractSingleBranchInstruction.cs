using Alluseri.Luna.Internals;

namespace Alluseri.Luna.Abstract.Bytecode;

public abstract class AbstractSingleBranchInstruction : Instruction {
	public abstract bool Conditional { get; } // Will be useful for the SMT builder
	internal uint TargetLocation;
	public Label Target;

	public AbstractSingleBranchInstruction(Label Target) {
		this.Target = Target;
	}

	internal override void Checkout(ConstantPool Pool) => base.Checkout(Pool);
}