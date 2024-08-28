using System.Collections.Generic;

namespace Alluseri.Luna.Abstract;

// DESIGN: MultiTypeDescriptor, ArgumentsDescriptor are better names, maybe?
// DESIGN: Do I even need this shit? It's only used in one place.

public class CompoundTypeDescriptor : Descriptor {
	public IList<TypeDescriptor> Descriptors;

	public CompoundTypeDescriptor(params TypeDescriptor[] Descriptors) {
		this.Descriptors = new List<TypeDescriptor>(Descriptors);
	}
	public CompoundTypeDescriptor(IList<TypeDescriptor> Descriptors) {
		this.Descriptors = Descriptors;
	}

	public override string Term => string.Join("", Descriptors);
}