using System;
using System.Collections.Generic;

namespace Alluseri.Luna;

public class OverflowedBootstrapMethod : BootstrapMethodBase {
	public override IList<StackConstant> Arguments => throw new NotSupportedException($"{nameof(OverflowedBootstrapMethod)} doesn't support arguments!");
	public override bool SupportsArguments => false;

	public OverflowedBootstrapMethod(MethodHandle Handle) : base(Handle) { }

	public override string ToString() => $"{{ OverflowedBootstrapMethod {Handle} }}";
}