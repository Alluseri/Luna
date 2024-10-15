namespace Alluseri.Luna.Bytecode;

public class TryBlockEnd(string Name) : PseudoInstruction {
	public string Name = Name;

	public override string ToString() => $"}} end try {Name}";
}