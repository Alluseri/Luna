using Alluseri.Luna.Internals;
using Alluseri.Luna.Utils;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Alluseri.Luna.Bytecode;

public class InsnLookupSwitch : Instruction {
	public Label DefaultCase;
	public SortedList<int, Label> Cases;

	internal int DefaultCaseLocation;
	internal Dictionary<int, int> CaseLocations = null!;

	internal InsnLookupSwitch(int DefaultCaseLocation, Dictionary<int, int> CaseLocations) {
		this.DefaultCaseLocation = DefaultCaseLocation;
		this.CaseLocations = CaseLocations;

		DefaultCase = null!;
		Cases = null!;
	}
	public InsnLookupSwitch(Label DefaultCase, IDictionary<int, Label> Cases) {
		this.DefaultCase = DefaultCase;
		this.Cases = new(Cases);
	}
	public InsnLookupSwitch(Label DefaultCase, SortedList<int, Label> Cases) {
		this.DefaultCase = DefaultCase;
		this.Cases = Cases;
	}

	private static int GetPadding(int Address) => (4 - ((Address + 1) % 4)) % 4; // idk how the fuck I cooked this but I did

	internal override void Checkout(CodeBuilder Builder, int Address) {
		Size = 1 + GetPadding(Address) + 4 + 4 + Cases.Count * (4 + 4);
	}

	internal override void Write(Stream Stream, CodeBuilder Class, int Address) {
		Stream.Write(Opcode.LookupSwitch);
		Stream.Write(stackalloc byte[GetPadding(Address)]);
		Stream.Write(DefaultCase.Location - Address);
		Stream.Write(Cases.Count);
		foreach (KeyValuePair<int, Label> Case in Cases) {
			Stream.Write(Case.Key);
			Stream.Write(Case.Value.Location - Address);
		}
	}

	public override string ToString() => $"lookupswitch [default:{DefaultCase}] {string.Join(' ', Cases.Select(Case => $"[{Case.Key}:{Case.Value}]"))}";
}