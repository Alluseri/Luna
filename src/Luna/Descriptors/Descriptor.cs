namespace Alluseri.Luna;

public abstract class Descriptor {
	public abstract string Term { get; } // TODO: Definitely needs a better name

	public override string ToString() => Term;
	public override int GetHashCode() => Term.GetHashCode();
	public override bool Equals(object? Other) => Other is Descriptor Desc && Desc.Term == Term;

	public static explicit operator string(Descriptor Desc) => Desc.Term; // While this can't technically fail, I'll still hesitate to mark this as implicit due to extreme ambiguity.
}