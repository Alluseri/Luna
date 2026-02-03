using Alluseri.Luna.Bytecode;
using Alluseri.Luna.Internals;
using Alluseri.Luna.Utils;
using System;
using System.Collections.Generic;

namespace Alluseri.Luna.Context;

public class MethodAnalysisContext {
	// TODO: To make this actually work, you need an internal verification type for padding longs and doubles.
	// THEN we can work on the analyzer.

	// The analyzer doesn't need to do any verification by itself.
	// Most of the magic happens in the merging step.
	public void Analyze(Method Method) {
		ArgumentNullException.ThrowIfNull(Method.Code);

		Dictionary<int, List<StackState>> Map = new();
		Stack<VerificationType> Stack = new();
		for (int i = 0; i < Method.Code.Count; i++) {
			Instruction Insn = Method.Code[i];

			// The entrance stack
			Map.GetOrNew(i).Add(new() {
				HasEntry = i == 0, // first instruction is always entered into
				Stack = Stack
			});

			// Evaluate the instruction for the next instruction's entrance stack
			switch (Insn) {
				case InsnZeroBranch Izb:
				Stack.Pop();

				Map.GetOrNew(Izb.Target.Location).Add(new() {
					HasEntry = true,
					Stack = Stack
				});
				break;
			}
		}
	}
}

public class StackState {
	public bool HasEntry { get; init; }
	public Stack<VerificationType> Stack { get; init; }
}