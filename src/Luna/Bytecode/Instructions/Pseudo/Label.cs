namespace Alluseri.Luna.Bytecode;

public class Label(string Name) : PseudoInstruction(true) {
	public string Name = Name;

	// TODO: Anonymous labels yes or no?
	public override int GetHashCode() => Name.GetHashCode();
	public override bool Equals(object? Other) => Other is Label Lab && Lab.Name == Name;
	public override string ToString() => $"{Name}:";
}