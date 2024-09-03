namespace Alluseri.Luna.Abstract.Bytecode;

public class TryBlockEnd(string Name) : PseudoInstruction {
	public string Name = Name;

	public override string ToString() => $"}} end try {Name}";
}