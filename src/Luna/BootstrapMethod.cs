using Alluseri.Luna.Utils;
using System.Collections.Generic;

namespace Alluseri.Luna;

public class BootstrapMethod : BootstrapMethodBase {
	private IList<StackConstant> _Arguments;
	public override IList<StackConstant> Arguments => _Arguments;
	public override bool SupportsArguments => true;

	public BootstrapMethod(MethodHandle Handle, IList<StackConstant> Arguments) : base(Handle) {
		_Arguments = Arguments;
	}
	public BootstrapMethod(MethodHandle Handle, IEnumerable<StackConstant> Arguments) : base(Handle) {
		_Arguments = new List<StackConstant>(Arguments);
	}
	public BootstrapMethod(MethodHandle Handle, params StackConstant[] Arguments) : base(Handle) {
		_Arguments = new List<StackConstant>(Arguments);
	}

	public override string ToString() => $"{{ BootstrapMethod {Handle} [ {GU.ToString(Arguments)} ] }}";
}