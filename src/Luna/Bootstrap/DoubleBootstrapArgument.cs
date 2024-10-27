using Alluseri.Luna.Internals;

namespace Alluseri.Luna;

public class DoubleBootstrapArgument : BootstrapArgument {
	public double Value;

	public DoubleBootstrapArgument(double Value) {
		this.Value = Value;
	}

	protected override ushort Checkout(ConstantPool Pool) => Pool.Checkout(new ConstantDouble(Value));

	public override string ToString() => $"{{ BArg::Double {Value}D }}";
}