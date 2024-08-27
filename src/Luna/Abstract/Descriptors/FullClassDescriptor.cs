namespace Alluseri.Luna.Abstract;

// DESIGN: ClassDescriptor, ClassNameDescriptor might be a better name.

public class FullClassNameDescriptor : UnqualifiedDescriptor {
	public FullClassNameDescriptor(string NormalName) : base(NormalName.Replace('.', '/')) { }
}