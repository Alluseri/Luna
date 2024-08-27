namespace Alluseri.Luna.Abstract;

// DESIGN: MultiTypeDescriptor, ArgumentsDescriptor are better names, maybe?

public class CompoundTypeDescriptor : Descriptor {
	public TypeDescriptor[] Descriptors;

	public CompoundTypeDescriptor(params TypeDescriptor[] Descriptors) {
		this.Descriptors = Descriptors;
	}

	public override string Term => string.Join("", (object[]) Descriptors);
}