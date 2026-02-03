using System;

namespace Alluseri.Luna;

// tbh this thing might as well die and everyone would love it
// TOTALLY

public abstract class NamedDescriptor : Descriptor {
	public string Name;

	public abstract string FullDescriptor { get; } // TODO: Reconsider the name after Term is renamed to something more proper

	public NamedDescriptor(string Name) {
		this.Name = Name;
	}

	public override int GetHashCode() => FullDescriptor.GetHashCode();
	public override string ToString() => FullDescriptor;
	public override bool Equals(object? Other) => Other is NamedDescriptor ND && ND.FullDescriptor == FullDescriptor;
}