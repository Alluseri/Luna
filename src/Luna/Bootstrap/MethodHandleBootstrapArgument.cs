using Alluseri.Luna.Internals;
using Alluseri.Luna.Utils;

namespace Alluseri.Luna;

public class MethodHandleBootstrapArgument : BootstrapArgument {
	public MethodHandle Handle;

	public MethodHandleBootstrapArgument(MethodHandle Handle) {
		this.Handle = Handle;
	}

	public override ushort Checkout(ConstantPool Pool) => Handle.Checkout(Pool);
}