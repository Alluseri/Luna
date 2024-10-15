using Alluseri.Luna.Internals;
using Alluseri.Luna.Utils;
using System;

namespace Alluseri.Luna;

public class StringBootstrapArgument : BootstrapArgument {
	public string Value;

	public StringBootstrapArgument(string Value) {
		this.Value = Value;
	}

	public override ushort Checkout(ConstantPool Pool) => Pool.Checkout(new ConstantString(Pool.CheckoutUtf8(Value)));
}