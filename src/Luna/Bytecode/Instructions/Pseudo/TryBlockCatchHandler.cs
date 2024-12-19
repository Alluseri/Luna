namespace Alluseri.Luna.Bytecode;

// This is not a case of symbolic referencing since you cannot catch an array of exceptions. Using "CatchClassName" relies the purpose pretty well.
public class TryBlockCatchHandler(string Name, string? CatchClassName) : PseudoInstruction {
	public TryBlockCatchHandler(string Name) : this(Name, null) { }

	public string Name = Name;
	public string? CatchClassName = CatchClassName;

	public override string ToString() => $"catch {CatchClassName ?? "*"} from {Name}:";
}