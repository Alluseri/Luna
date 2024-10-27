using Alluseri.Luna.Internals;

namespace Alluseri.Luna;

public class IntegerBootstrapArgument : BootstrapArgument {
	public int Value;

	public IntegerBootstrapArgument(int Value) {
		this.Value = Value;
	}

	protected override ushort Checkout(ConstantPool Pool) => Pool.Checkout(new ConstantInteger(Value));

	public override string ToString() => $"{{ BArg::Integer {Value} }}";
}