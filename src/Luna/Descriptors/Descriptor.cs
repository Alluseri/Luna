namespace Alluseri.Luna;

// TODO: hashcode equals IMPORTANT!

public abstract class Descriptor {
	public abstract string Term { get; } // TODO: Definitely needs a better name

	public override string ToString() => Term;

	public static explicit operator string(Descriptor Desc) => Desc.Term; // While this can't technically fail, I'll still hesitate to mark this as implicit due to extreme ambiguity.
}