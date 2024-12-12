using Alluseri.Luna.Internals;
using System.Collections.Generic;
using System.Linq;

namespace Alluseri.Luna;

public abstract class BootstrapMethodBase {
	public MethodHandle Handle;
	public abstract bool SupportsArguments { get; }
	public abstract IList<StackConstant> Arguments { get; }

	public BootstrapMethodBase(MethodHandle Handle) {
		this.Handle = Handle;
	}

	public static BootstrapMethodBase FromInternal(InternalClass Class, Internals.BootstrapMethod Method, HashSet<ushort>? Fork = null) {
		ConstantMethodHandle BootstrapHandle = Method.GetHandle(Class.ConstantPool);

		MethodHandle ManagedHandle = new(BootstrapHandle.Kind, ClassMemberReference.FromConstant(Class.ConstantPool, BootstrapHandle.GetInfo(Class.ConstantPool)));

		// For each argument, we build an *identity* set of bootstrap methods (e.g. through indices into the BootstrapMethods attribute)
		// We fork the set for every resolution which results in a ConstantDynamic

		// Additional problem to solve: may still stack overflow.

		List<StackConstant> Constants = new();
		foreach (ushort ArgumentIndex in Method.ArgumentIndexes) {
			ConstantInfo Ci = Class.ConstantPool[ArgumentIndex];
			if (Ci is ConstantDynamic Condy) {
				HashSet<ushort> IdxCopy;
				if (Fork == null) {
					IdxCopy = new() { ArgumentIndex };
				} else {
					if (Fork.Contains(ArgumentIndex))
						return new CyclicBootstrapMethod(ManagedHandle); // DESIGN: Do we return CyclicBootstrapMethod (i.e. abort on level 1) or add a new ConstantDynamic pointing to a CyclicBM to arguments (i.e. abort on level n)?
					else if (Fork.Count > 256) // Over 256 seems unrealistic and really kills the performance // TODO: Add a way to specify the length
						return new OverflowedBootstrapMethod(ManagedHandle);
					else {
						IdxCopy = new(Fork) {
							ArgumentIndex
						};
					}
				}
				Constants.Add(StackConstantDynamic.FromConstantDynamic(Class, Condy, IdxCopy));
			} else
				// Constants.Add(StackConstant.FromConstant(Class, Ci));
				Constants.Add(StackConstant.FromConstant(Class, ArgumentIndex));
		}

		return new BootstrapMethod(ManagedHandle, Constants);
	}
}