using Alluseri.Luna.Internals;

namespace Alluseri.Luna;

public class LongBootstrapArgument : BootstrapArgument {
	public long Value;

	public LongBootstrapArgument(long Value) {
		this.Value = Value;
	}

	public override ushort Checkout(ConstantPool Pool) => Pool.Checkout(new ConstantLong(Value));
}