namespace Alluseri.Luna.Abstract.Bytecode;

public class Label(string Name) : PseudoInstruction(true) {
	public string Name = Name;

	public override string ToString() => $"{Name}:";
}