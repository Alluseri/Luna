using Alluseri.Luna.Internals;
using System.Diagnostics;
using System.IO;

namespace Alluseri.Luna.Bytecode;

// TODO: Every branch instruction must introduce self-integrity to ensure all its labels are present in the output code
// This might've been introduced in RepairLabelIdentities in CodeBuilder (but it's not mandatory to enable so...)

public abstract class AbstractSingleBranchInstruction : Instruction {
	public abstract bool Conditional { get; } // Will be useful for the SMT builder
	internal int TargetLocation; // This is only used by the code reader, don't worry about its existence; DESIGN: Is this a good idea? Looks fucked up to me
	public Label Target;

	public AbstractSingleBranchInstruction(Label Target) {
		this.Target = Target;
	}

	internal abstract override void Write(Stream Stream, CodeBuilder Class, int Address);
}