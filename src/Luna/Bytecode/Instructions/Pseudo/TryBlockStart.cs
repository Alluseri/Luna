namespace Alluseri.Luna.Bytecode;

public class TryBlockStart(string Name, int Priority = 0) : PseudoInstruction {
	public string Name = Name;
	public int Priority = Priority; // the higher this value - the closer to index 0 the handler of this try block is

	public override string ToString() => $"try {Name}{(Priority != 0 ? $" #{Priority}" : "")} {{";
}