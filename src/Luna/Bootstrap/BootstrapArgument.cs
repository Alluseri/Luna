using Alluseri.Luna.Internals;
using System;
using System.IO;

namespace Alluseri.Luna;

/*
Can be:
+ Class
+ Dynamic (bruh 💀)
+ String
+ MethodHandle (managed representation: a computed/linked java.lang.invoke.MethodHandle)
+ MethodType (managed representation: a computed java.lang.invoke.MethodType)
+ Integer
+ Float
+ Long
+ Double
Everything else is fatal during resolution
*/

public abstract class BootstrapArgument {
	public abstract ushort Checkout(ConstantPool Pool);

	public static BootstrapArgument FromConstant(InternalClass Class, ConstantInfo Info) => Info switch {
		ConstantClass CClass => new ClassBootstrapArgument(CClass.GetName(Class.ConstantPool)),
		ConstantDynamic CDyn => new DynamicBootstrapArgument(
			BootstrapMethod.FromInternal(Class, CDyn.GetBootstrapMethod(Class) ?? throw new InvalidDataException($"Recovery from a malformed ConDy is not yet implemented.")),
			FieldDescriptor.FromSignature(Class.ConstantPool, CDyn.GetNameAndType(Class.ConstantPool))),
		ConstantString CString => new StringBootstrapArgument(CString.GetString(Class.ConstantPool)),
		ConstantMethodHandle CMeh => new MethodHandleBootstrapArgument(new(CMeh.Kind, ClassMemberReference.FromConstant(Class.ConstantPool, CMeh.GetInfo(Class.ConstantPool)))),
		ConstantMethodType CMet => new MethodTypeBootstrapArgument(MethodTypeDescriptor.FromSignature(Class.ConstantPool, CMet)),
		ConstantInteger CInt => new IntegerBootstrapArgument(CInt.Value),
		ConstantFloat CFloat => new FloatBootstrapArgument(CFloat.Value),
		ConstantLong CLong => new LongBootstrapArgument(CLong.Value),
		ConstantDouble CDouble => new DoubleBootstrapArgument(CDouble.Value),
		_ => throw new InvalidDataException($"Cannot represent {Info} as a managed bootstrap argument.")
	};

	public override string ToString() => $"{}";
}