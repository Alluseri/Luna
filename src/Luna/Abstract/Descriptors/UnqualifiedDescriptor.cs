namespace Alluseri.Luna.Abstract;

public class UnqualifiedDescriptor : ConstantDescriptor { // DESIGN: The name is misleading because it doesn't make any checks for true unqualifiedness, and it's very fucky to do it anyway
	public UnqualifiedDescriptor(string QualifiedName) : base(QualifiedName) { }
}