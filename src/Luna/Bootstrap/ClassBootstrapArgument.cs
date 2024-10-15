using Alluseri.Luna.Internals;
using Alluseri.Luna.Utils;
using System;

namespace Alluseri.Luna;

#pragma warning disable CS8618
public class ClassBootstrapArgument : BootstrapArgument {
	private string _ClassName;
	public string ClassName {
		get => _ClassName;
		set => _ClassName = value.Replace('.', '/'); // DESIGN: We NEED consistency for these replacements. Either we do it everywhere or we don't anywhere. ffs.
	}

	public ClassBootstrapArgument(string ClassName) {
		this.ClassName = ClassName;
	}

	public override ushort Checkout(ConstantPool Pool) => Pool.Checkout(new ConstantClass(Pool.CheckoutUtf8(_ClassName)));
}