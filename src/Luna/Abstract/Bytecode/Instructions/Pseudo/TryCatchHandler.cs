namespace Alluseri.Luna.Abstract.Bytecode;

public class TryCatchHandler(string Name, string? CatchClassName) : PseudoInstruction {
	public string Name = Name;
	public string? CatchClassName = CatchClassName;

	public override string ToString() => $"catch {CatchClassName ?? "*"} from {Name}:";
}