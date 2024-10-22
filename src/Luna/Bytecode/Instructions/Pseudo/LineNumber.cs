namespace Alluseri.Luna.Bytecode;

public class LineNumber(ushort Line) : PseudoInstruction {
	public ushort Line = Line;

	public override string ToString() => $"[ L{Line} ]";
}