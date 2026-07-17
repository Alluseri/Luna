using Alluseri.Luna.Internals;

namespace Alluseri.Luna;

public class MethodHandle(MethodHandleKind Kind, ClassMemberReference Reference) {
	public MethodHandleKind Kind = Kind;
	public ClassMemberReference Reference = Reference;

	public ushort Checkout(ConstantPool Pool) => Pool.Checkout(
		new ConstantMethodHandle(
			Kind,
			Reference.Checkout(Pool)
		)
	);

	public override string ToString() => $"{{ MethodHandle of kind {Kind} to {Reference} }}";
}