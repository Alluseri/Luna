using System;
using System.Collections.Generic;

namespace Alluseri.Luna;

public class CyclicBootstrapMethod : BootstrapMethodBase {
	public override IList<StackConstant> Arguments => throw new NotSupportedException($"{nameof(CyclicBootstrapMethod)} doesn't support arguments!");
	public override bool SupportsArguments => false;

	public CyclicBootstrapMethod(MethodHandle Handle) : base(Handle) { }

	public override string ToString() => $"{{ CyclicBootstrapMethod {Handle} }}";
}