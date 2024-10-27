using Alluseri.Luna.Internals;

namespace Alluseri.Luna;

public class LongBootstrapArgument : BootstrapArgument {
	public long Value;

	public LongBootstrapArgument(long Value) {
		this.Value = Value;
	}

	protected override ushort Checkout(ConstantPool Pool) => Pool.Checkout(new ConstantLong(Value));

	public override string ToString() => $"{{ BArg::Long {Value}L }}";
}