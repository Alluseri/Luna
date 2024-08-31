namespace Alluseri.Luna.Abstract;

public abstract class ConstantDescriptor : Descriptor { // TODO: Possibly consider a better name
	private readonly string _Term;
	public override string Term => _Term;

	public ConstantDescriptor(string Term) {
		_Term = Term;
	}
}