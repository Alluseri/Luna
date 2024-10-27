using Alluseri.Luna.Internals;
using Alluseri.Luna.Utils;

namespace Alluseri.Luna;

public class MethodHandleBootstrapArgument : BootstrapArgument {
	public MethodHandle Handle;

	public MethodHandleBootstrapArgument(MethodHandle Handle) {
		this.Handle = Handle;
	}

	protected override ushort Checkout(ConstantPool Pool) => Handle.Checkout(Pool);

	public override string ToString() => $"{{ BArg::MethodHandle {Handle} }}";
}