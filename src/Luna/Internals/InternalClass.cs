using Alluseri.Luna.Exceptions;
using Alluseri.Luna.Utils;
using System.IO;
using System.Linq;

namespace Alluseri.Luna.Internals;

/*
	DESIGN IMPLICATIONS
	This mostly has the basis of being as writable as it can be without changing the internal state of the object, as in:
	- You can add new entries to the ConstantPool, but it does not affect the internal references to the pool from other elements of the class file.
	- Likewise with Version: as we don't check for backwards-compatibility(or anything for that matter) in a class that belongs to Luna.Internals,
	  we only write this field to the output file as it is never referenced by anything else.
	- Same as the above applies to Interfaces: we only write it to output, never read it internally.
	- If we look recursively into Fields and Methods, then the same is applicable there.
	- If you end up overwriting Attributes, this won't affect the internal state as we never reference them directly.

	Getter functions(GetThisClass, GetSuperClass, GetInterfaces) expect valid data, which is beneficial because there can be no CCEs in valid classes.
*/

public class InternalClass {
	public readonly (ushort Minor, ushort Major) Version;
	public readonly ConstantPool ConstantPool;
	public readonly ClassAccessFlags AccessFlags;
	public readonly ushort ThisClass;
	public readonly ushort SuperClass;
	public readonly ushort[] Interfaces;
	public readonly FieldInfo[] Fields;
	public readonly MethodInfo[] Methods;
	public readonly AttributeInfo[] Attributes;

	public ushort JavaVersion => (ushort) (Version.Major - 44);

	public InternalClass(Stream Stream) {
		if (Stream.ReadUInt() != 0xCAFEBABE)
			throw new ClassFileException("The given file is not a valid JVM class file.");

		Version = (Stream.ReadUShort(), Stream.ReadUShort());

		ushort ConstantPoolLength = Stream.ReadUShort();
		ConstantPool = new();
		for (ushort i = 1; i < ConstantPoolLength; i++) {
			try {
				if (ConstantPool.Add(ConstantInfo.Parse(Stream)).IsWide)
					i++;
			} catch (ConstantPoolException CPE) {
				throw new ClassFileException($"Malformed constant pool: illegal tag of entry {i}/{ConstantPoolLength} on position {Stream.Position - 1:X}.", CPE);
			}
		}

		AccessFlags = (ClassAccessFlags) Stream.ReadUShort();
		ThisClass = Stream.ReadUShort();
		SuperClass = Stream.ReadUShort();

		GU.New(out Interfaces, Stream.ReadUShort());
		for (ushort i = 0; i < Interfaces.Length; i++)
			Interfaces[i] = Stream.ReadUShort();

		GU.New(out Fields, Stream.ReadUShort());
		for (ushort i = 0; i < Fields.Length; i++)
			Fields[i] = new FieldInfo(Stream, ConstantPool);

		GU.New(out Methods, Stream.ReadUShort());
		for (ushort i = 0; i < Methods.Length; i++)
			Methods[i] = new MethodInfo(Stream, ConstantPool);

		GU.New(out Attributes, Stream.ReadUShort());
		for (ushort i = 0; i < Attributes.Length; i++)
			Attributes[i] = AttributeInfo.Parse(Stream, ConstantPool) ?? throw new ClassFileException($"Cannot parse an incomplete or otherwise malformed class file: ran out of bytes while reading the cpool index of attribute #{i} (owned by a ClassFile).");
	}

	public InternalClass(
		(ushort Minor, ushort Major) Version,
		ConstantPool ConstantPool,
		ClassAccessFlags AccessFlags,
		ushort ThisClass,
		ushort SuperClass,
		ushort[] Interfaces,
		FieldInfo[] Fields,
		MethodInfo[] Methods,
		AttributeInfo[] Attributes
	) {
		this.Version = Version;
		this.ConstantPool = ConstantPool;
		this.AccessFlags = AccessFlags;
		this.ThisClass = ThisClass;
		this.SuperClass = SuperClass;
		this.Interfaces = Interfaces;
		this.Fields = Fields;
		this.Methods = Methods;
		this.Attributes = Attributes;
	}

	public void Checkout() {
		foreach (FieldInfo Field in Fields)
			foreach (AttributeInfo Attr in Field.Attributes)
				Attr.Checkout(ConstantPool);

		foreach (MethodInfo Method in Methods)
			foreach (AttributeInfo Attr in Method.Attributes)
				Attr.Checkout(ConstantPool);

		foreach (AttributeInfo Attr in Attributes)
			Attr.Checkout(ConstantPool);
	}

	public void Write(Stream OutputStream) {
		OutputStream.Write(0xCAFEBABE);
		OutputStream.Write(Version.Minor);
		OutputStream.Write(Version.Major);

		OutputStream.Write((ushort) (ConstantPool.Count + 1));
		foreach (ConstantInfo Ci in ConstantPool)
			Ci.Write(OutputStream);

		OutputStream.Write((ushort) AccessFlags);
		OutputStream.Write(ThisClass);
		OutputStream.Write(SuperClass);

		OutputStream.Write((ushort) Interfaces.Length);
		foreach (ushort Interface in Interfaces)
			OutputStream.Write(Interface);

		OutputStream.Write((ushort) Fields.Length);
		foreach (FieldInfo Field in Fields)
			Field.Write(OutputStream, ConstantPool);

		OutputStream.Write((ushort) Methods.Length);
		foreach (MethodInfo Method in Methods)
			Method.Write(OutputStream, ConstantPool);

		OutputStream.Write((ushort) Attributes.Length);
		foreach (AttributeInfo Attr in Attributes)
			Attr.Write(OutputStream, ConstantPool);
	}

	public ConstantClass GetThisClass() => (ConstantClass) ConstantPool[ThisClass];
	public string GetThisClassName() => ConstantPool.Value<ConstantUtf8>(GetThisClass().NameIndex).Value;
	public ConstantClass? GetSuperClass() => SuperClass == 0 ? null : (ConstantClass) ConstantPool[SuperClass];
	public string? GetSuperClassName() => SuperClass == 0 ? null : ConstantPool.Value<ConstantUtf8>(GetSuperClass()!.NameIndex).Value;
	public ConstantClass[] GetInterfaces() => Interfaces.Select(Idx => (ConstantClass) ConstantPool[Idx]).ToArray(); // DESIGN: Should we return IEnumerable instead?
}