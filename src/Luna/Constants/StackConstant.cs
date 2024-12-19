using Alluseri.Luna.Bytecode;
using Alluseri.Luna.Internals;
using Alluseri.Luna.Utils;
using System.IO;

namespace Alluseri.Luna;

public abstract class StackConstant {
	protected static int GetLdcSize(ushort PoolIndex) => PoolIndex > byte.MaxValue ? 3 : 2; // You are never expected to call this if you're already a WIDE opcode.

	protected static Stream Ldc(Stream Stream, ushort PoolIndex, bool Wide = false) {
		if (Wide || PoolIndex > byte.MaxValue) {
			Stream.WriteByte((byte) (Wide ? Opcode.Ldc2_W : Opcode.Ldc_W));
			Stream.Write(PoolIndex);
		} else {
			Stream.Write(Opcode.Ldc);
			Stream.Write((byte) PoolIndex);
		}
		return Stream;
	}

	internal abstract ushort CheckoutPool(CodeBuilder Builder);
	internal abstract void CheckoutLdc(CodeBuilder Builder, out int Size);
	internal abstract void WriteLdc(Stream Stream);

	public abstract override string ToString();
	public abstract string ToLdcString();

	public static StackConstant FromConstant(InternalClass Class, ushort InfoIndex) => Class.ConstantPool[InfoIndex] switch {
		ConstantClass CClass => new StackConstantClass(CClass.GetName(Class.ConstantPool)),
		ConstantDynamic CDyn => StackConstantDynamic.FromConstantDynamic(Class, CDyn, new() { InfoIndex }),
		ConstantString CString => new StackConstantString(CString.GetString(Class.ConstantPool)),
		ConstantMethodHandle CMeh => new StackConstantMethodHandle(new(CMeh.Kind, ClassMemberReference.FromConstant(Class.ConstantPool, CMeh.GetInfo(Class.ConstantPool)))),
		ConstantMethodType CMet => new StackConstantMethodType(MethodTypeDescriptor.FromSignature(Class.ConstantPool, CMet)),
		ConstantInteger CInt => new StackConstantInteger(CInt.Value),
		ConstantFloat CFloat => new StackConstantFloat(CFloat.Value),
		ConstantLong CLong => new StackConstantLong(CLong.Value),
		ConstantDouble CDouble => new StackConstantDouble(CDouble.Value),
		_ => throw new InvalidDataException($"Cannot represent {Class.ConstantPool[InfoIndex]} as a managed bootstrap argument.")
	};
}