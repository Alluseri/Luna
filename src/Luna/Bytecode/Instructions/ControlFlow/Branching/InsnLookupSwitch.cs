using Alluseri.Luna.Internals;
using Alluseri.Luna.Utils;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Alluseri.Luna.Bytecode;

public class InsnLookupSwitch : Instruction {
	public Label DefaultCase;
	public IDictionary<int, Label> Cases;

	internal int DefaultTargetLocation;
	internal Dictionary<int, int> TargetLocations = null!;

	internal InsnLookupSwitch(int DefaultTargetLocation, Dictionary<int, int> TargetLocations) {
		this.DefaultTargetLocation = DefaultTargetLocation;
		this.TargetLocations = TargetLocations;

		DefaultCase = null!;
		Cases = null!;
	}
	public InsnLookupSwitch(Label DefaultTarget, Dictionary<int, Label> Targets) {
		this.DefaultCase = DefaultTarget;
		this.Cases = Targets;
	}

	internal override void Checkout(ConstantPool Pool) => base.Checkout(Pool);
	internal override void Write(Stream Stream, CodeBuilder Class) => throw new UnreachableException("Write(,,) must be used to write ILS. What?");

	internal void Write(Stream Stream, CodeBuilder Class, int Address) {
		Stream.Write(Opcode.LookupSwitch);
		Stream.Write(stackalloc byte[Address % 4]);
		Stream.Write(DefaultCase.Location - Address);
		Stream.Write(Cases.Count);
		foreach (KeyValuePair<int, Label> Case in Cases) {
			Stream.Write(Case.Key);
			Stream.Write(Case.Value.Location - Address);
		}
	}

	public override string ToString() => $"lookupswitch [default:{DefaultCase}] {string.Join(' ', Cases.Select(Case => $"[{Case.Key}:{Case.Value}]"))}";
}