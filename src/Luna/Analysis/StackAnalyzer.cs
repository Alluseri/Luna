using Alluseri.Luna.Bytecode;
using Alluseri.Luna.Internals;
using System.Collections.Generic;

namespace Alluseri.Luna.Analysis;

public class StackAnalyzer {
	private readonly AppContext Context;

	public StackAnalyzer(AppContext Context) {
		this.Context = Context;
	}

	List<Edge> Edges = new();

	public void Analyze(IList<Instruction> Instructions) {
		List<VerificationType> Stack = new();
		List<VerificationType> Locals = new();

		// First pass: Just put labels in place
		for (int i = 0, z = 0; i < Instructions.Count; i++) {
			if (Instructions[i] is PseudoInstruction Pi) {
				// IsLabel check exists, but it's not really relevant for us since every pseudo is a stack frame
				Edges[z] =
			} else
				z++;
		}
	}

	public Edge GetEdge(PseudoInstruction Label) {
		return Edges.Find(E => E.LabelsAtEdge.Contains(Label)) ?? throw new MalformedBytecodeException($"Label {Label} doesn't exist in provided bytecode!");
	}

	class Edge {
		public HashSet<PseudoInstruction> LabelsAtEdge; // This can be both labels and try/catch blocks
		public bool Visited; // If already visited and trying to merge into with bs data, might want to crash
		public List<VerificationType> Stack; // TODO: Can override getter with a copier
		public List<VerificationType> Locals;

		public Edge() {
			LabelsAtEdge = new();
			Visited = false;
		}
	}
}