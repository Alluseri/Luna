using Alluseri.Luna.Internals;

namespace Alluseri.Luna;

public class MethodHandle(MethodHandleReferenceKind Kind, ClassMemberReference Reference) {
	MethodHandleReferenceKind Kind = Kind;
	ClassMemberReference Reference = Reference;

	public ushort Checkout(ConstantPool Pool) => Pool.Checkout(
		new ConstantMethodHandle(
			Kind,
			Reference.Checkout(Pool)
		)
	);

	public override string ToString() => $"{{ MethodHandle of kind {Kind} to {Reference} }}";
}