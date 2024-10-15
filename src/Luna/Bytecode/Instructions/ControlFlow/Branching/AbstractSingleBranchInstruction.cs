using Alluseri.Luna.Internals;
using System;
using System.Diagnostics;
using System.IO;

namespace Alluseri.Luna.Bytecode;

public abstract class AbstractSingleBranchInstruction : Instruction {
	public abstract bool Conditional { get; } // Will be useful for the SMT builder
	internal int TargetLocation; // This is only used by the code reader, don't worry about its existence
	public Label Target;

	public AbstractSingleBranchInstruction(Label Target) {
		this.Target = Target;
	}

	internal override void Checkout(ConstantPool Pool) => base.Checkout(Pool);
	internal override void Write(Stream Stream, CodeBuilder Class) => throw new UnreachableException("Write(,,) must be used to write ASBI. What?");

	internal abstract void Write(Stream Stream, CodeBuilder Class, int Address);
}