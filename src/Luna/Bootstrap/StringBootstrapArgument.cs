using Alluseri.Luna.Internals;
using Alluseri.Luna.Utils;
using System;

namespace Alluseri.Luna;

public class StringBootstrapArgument : BootstrapArgument {
	public string Value;

	public StringBootstrapArgument(string Value) {
		this.Value = Value;
	}

	protected override ushort Checkout(ConstantPool Pool) => Pool.Checkout(new ConstantString(Pool.CheckoutUTF8(Value)));

	public override string ToString() => $"{{ BArg::String \"{Value}\" }}"; // TODO: Escape (make a utility method god damn it)
}