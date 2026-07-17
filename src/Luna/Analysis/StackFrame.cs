using Alluseri.Luna.Bytecode;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Alluseri.Luna.Analysis;

public class StackFrame {
	public AnalysisType[] Locals;
	public Stack<AnalysisType> Stack;

	// TODO: "Node[] Origins;" field for debugging?

	public bool HasLocal(ushort Slot) => Locals.Length > Slot;

	public StackFrame Copy() {
		return new() {
			Locals = (AnalysisType[]) Locals.Clone(),
			Stack = new(Stack.Reverse())
		};
	}

	public StackFrame CopyWithStack(AnalysisType[] Stack) {
		return new() {
			Locals = (AnalysisType[]) Locals.Clone(),
			Stack = new(Stack.Reverse())
		};
	}
}