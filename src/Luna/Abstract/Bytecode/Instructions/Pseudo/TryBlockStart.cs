namespace Alluseri.Luna.Abstract.Bytecode;

public class TryBlockStart(string Name) : PseudoInstruction {
	public string Name = Name;

	public override string ToString() => $"try {Name} {{";
}