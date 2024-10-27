using Alluseri.Luna.Internals;
using Alluseri.Luna.Utils;
using System.Collections.Generic;
using System.Linq;

namespace Alluseri.Luna;

public class BootstrapMethod {
	public MethodHandle Handle;
	public IList<BootstrapArgument> Arguments;

	public BootstrapMethod(MethodHandle Handle, IList<BootstrapArgument> Arguments) {
		this.Handle = Handle;
		this.Arguments = Arguments;
	}
	public BootstrapMethod(MethodHandle Handle, IEnumerable<BootstrapArgument> Arguments) {
		this.Handle = Handle;
		this.Arguments = new List<BootstrapArgument>(Arguments);
	}
	public BootstrapMethod(MethodHandle Handle, params BootstrapArgument[] Arguments) {
		this.Handle = Handle;
		this.Arguments = new List<BootstrapArgument>(Arguments);
	}

	public static BootstrapMethod FromInternal(InternalClass Class, Internals.BootstrapMethod Method) {
		ConstantMethodHandle BootstrapHandle = Method.GetHandle(Class.ConstantPool);
		return new(
			new MethodHandle(BootstrapHandle.Kind, ClassMemberReference.FromConstant(Class.ConstantPool, BootstrapHandle.GetInfo(Class.ConstantPool))),
			Method.ArgumentIndexes.Select(Ai => BootstrapArgument.FromConstant(Class, Class.ConstantPool[Ai]))
		);
	}

	public override string ToString() => $"{{ BootstrapMethod call to {Handle} with args [ {GU.ToString(Arguments)} ] }}";
}