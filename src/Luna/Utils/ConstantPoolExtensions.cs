using Alluseri.Luna.Internals;

namespace Alluseri.Luna.Utils;

public static class ConstantPoolExtensions {
	public static ushort CheckoutUtf8(this ConstantPool Self, string Value) => Self.Checkout(new ConstantUtf8(Value));
}