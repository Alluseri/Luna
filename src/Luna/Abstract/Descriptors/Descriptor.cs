namespace Alluseri.Luna.Abstract;

public abstract class Descriptor {
	public abstract string Term { get; } // TODO: Definitely needs a better name

	public override string ToString() => Term;
}