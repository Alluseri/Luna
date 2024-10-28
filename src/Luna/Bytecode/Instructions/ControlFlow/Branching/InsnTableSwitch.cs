using Alluseri.Luna.Internals;
using Alluseri.Luna.Utils;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Alluseri.Luna.Bytecode;

public class InsnTableSwitch : Instruction {
	public Label DefaultCase;
	public int MinMatch;
	public IList<Label> Cases;

	public int MaxMatch => MinMatch + Cases.Count - 1;

	internal int DefaultTargetLocation;
	internal List<int> TargetLocations = null!;

	internal InsnTableSwitch(int DefaultTargetLocation, List<int> TargetLocations) {
		this.DefaultTargetLocation = DefaultTargetLocation;
		this.TargetLocations = TargetLocations;

		DefaultCase = null!;
		Cases = null!;
	}
	public InsnTableSwitch(Label DefaultTarget, IList<Label> Targets) {
		this.DefaultCase = DefaultTarget;
		this.Cases = Targets;
	}

	internal override void Checkout(CodeBuilder Builder, int Address) {
		Size = 1 + ((4 - ((Address + 1) % 4)) % 4) + 4 + 4 + 4 + Cases.Count * 4;
	}

	internal override void Write(Stream Stream, CodeBuilder Class, int Address) {
		Stream.Write(Opcode.TableSwitch);
		Stream.Write(stackalloc byte[(4 - ((Address + 1) % 4)) % 4]);
		Stream.Write(DefaultCase.Location - Address);
		Stream.Write(MinMatch);
		Stream.Write(MaxMatch);
		foreach (Label Lb in Cases) {
			Stream.Write(Lb.Location - Address);
		}
	}

	public override string ToString() => $"tableswitch [default:{DefaultCase}] {string.Join(' ', Cases.Select((Case, Index) => $"[{MinMatch + Index}:{Case}]"))}";
}