using Alluseri.Luna.Exceptions;
using System.IO;

namespace Alluseri.Luna.Internals;

public abstract class ConstantInfo {
	public readonly ConstantInfoTag Tag;
	public virtual bool IsWide => false;

	public ConstantInfo(ConstantInfoTag Tag) {
		this.Tag = Tag;
	}

	public abstract override bool Equals(object? obj);
	public abstract override int GetHashCode();
	public abstract override string ToString();

	public abstract void Write(Stream Stream);

	public static ConstantInfo Parse(Stream Stream) {
		int Tag = Stream.ReadByte();
		return (ConstantInfoTag) Tag switch {
			ConstantInfoTag.UTF8 => new ConstantUtf8(Stream),
			ConstantInfoTag.Integer => new ConstantInteger(Stream),
			ConstantInfoTag.Float => new ConstantFloat(Stream),
			ConstantInfoTag.Long => new ConstantLong(Stream),
			ConstantInfoTag.Double => new ConstantDouble(Stream),
			ConstantInfoTag.Class => new ConstantClass(Stream),
			ConstantInfoTag.String => new ConstantString(Stream),
			ConstantInfoTag.FieldRef => new ConstantFieldRef(Stream),
			ConstantInfoTag.MethodRef => new ConstantMethodRef(Stream),
			ConstantInfoTag.InterfaceMethodRef => new ConstantInterfaceMethodRef(Stream),
			ConstantInfoTag.NameAndType => new ConstantNameAndType(Stream),
			ConstantInfoTag.MethodHandle => new ConstantMethodHandle(Stream),
			ConstantInfoTag.MethodType => new ConstantMethodType(Stream),
			ConstantInfoTag.Dynamic => new ConstantDynamic(Stream),
			ConstantInfoTag.InvokeDynamic => new ConstantInvokeDynamic(Stream),
			ConstantInfoTag.Module => new ConstantModule(Stream),
			ConstantInfoTag.Package => new ConstantPackage(Stream),
			_ => throw new ConstantPoolException($"Tried parsing a constant pool entry with an invalid tag: {Tag}.")
		};
	}
}

public enum ConstantInfoTag : byte {
	UTF8 = 1,
	Integer = 3,
	Float,
	Long,
	Double,
	Class,
	String,
	FieldRef,
	MethodRef,
	InterfaceMethodRef,
	NameAndType,
	MethodHandle = 15,
	MethodType,
	Dynamic,
	InvokeDynamic,
	Module,
	Package,
	LunaPadding = 255
}