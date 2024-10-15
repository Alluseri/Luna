using Alluseri.Luna.Internals;

namespace Alluseri.Luna;

public class IntegerBootstrapArgument : BootstrapArgument {
	public int Value;

	public IntegerBootstrapArgument(int Value) {
		this.Value = Value;
	}

	public override ushort Checkout(ConstantPool Pool) => Pool.Checkout(new ConstantInteger(Value));
}