using Alluseri.Luna.Internals;

namespace Alluseri.Luna;

public class FloatBootstrapArgument : BootstrapArgument {
	public float Value;

	public FloatBootstrapArgument(float Value) {
		this.Value = Value;
	}

	public override ushort Checkout(ConstantPool Pool) => Pool.Checkout(new ConstantFloat(Value));
}