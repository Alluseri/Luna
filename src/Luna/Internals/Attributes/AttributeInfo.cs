using Alluseri.Luna.Utils;
using System;
using System.IO;

namespace Alluseri.Luna.Internals;

// TODO: Pedantic mode for attributes like NestHost
// TODO: Hyper-pedantic mode for every attribute that has garbage data in tail

public abstract class AttributeInfo : ISizeable {
	public readonly string Name;
	public readonly uint Size;
	int ISizeable.Size => (int) Size; // DESIGN: What do I do with this bullshit cast? :/

	public AttributeInfo(string Name, int Size) {
		this.Name = Name;
		this.Size = (uint) Size + 6;
	}
	public AttributeInfo(string Name, uint Size) {
		this.Name = Name;
		this.Size = Size + 6;
	}

	public abstract override int GetHashCode();
	public abstract override bool Equals(object? Object);
	public abstract override string ToString();

	public static AttributeInfo? Parse(Stream Stream, ConstantPool Pool) {
		if (!Stream.ReadUShort(out ushort Cv))
			return null;
		if (Pool[Cv] is not ConstantUtf8 Cu8)
			return InvalidAttribute.Parse(Stream, Cv);
		return Cu8.Value switch {
			"BootstrapMethods" => BootstrapMethodsAttribute.Parse(Stream),
			"Code" => CodeAttribute.ParseCode(Stream, Pool),
			"Deprecated" => DeprecatedAttribute.Parse(Stream),
			"EnclosingMethod" => EnclosingMethodAttribute.Parse(Stream),
			"Exceptions" => ExceptionsAttribute.Parse(Stream),
			"InnerClasses" => InnerClassesAttribute.Parse(Stream),
			"LineNumberTable" => LineNumberTableAttribute.Parse(Stream),
			"LocalVariableTable" => LocalVariableTableAttribute.Parse(Stream),
			"MethodParameters" => MethodParametersAttribute.Parse(Stream),
			"ModuleHashes" => ModuleHashesAttribute.Parse(Stream),
			"NestHost" => NestHostAttribute.Parse(Stream),
			"NestMembers" => NestMembersAttribute.Parse(Stream),
			"Record" => RecordAttribute.ParseRecord(Stream, Pool),
			"RuntimeInvisibleAnnotations" => RuntimeAnnotationsAttribute.ParseRA(Stream, false),
			"RuntimeVisibleAnnotations" => RuntimeAnnotationsAttribute.ParseRA(Stream, true),
			"RuntimeInvisibleParameterAnnotations" => RuntimeParameterAnnotationsAttribute.ParseRPA(Stream, false),
			"RuntimeVisibleParameterAnnotations" => RuntimeParameterAnnotationsAttribute.ParseRPA(Stream, true),
			"Signature" => SignatureAttribute.Parse(Stream),
			"SourceFile" => SourceFileAttribute.Parse(Stream),
			"StackMapTable" => StackMapTableAttribute.Parse(Stream),
			_ => UnknownAttribute.Parse(Stream, Cu8.Value),
		};
	}

	public virtual void Checkout(ConstantPool Pool) {
		Pool.Checkout(new ConstantUtf8(Name));
	}

	protected abstract void Write(Stream Stream);
	public virtual void Write(Stream Stream, ConstantPool Pool) {
		// DEBUGTRACE: Console.WriteLine($"Started writing {Name} at {Stream.Position:X}"); // DEBUGTRACE
		Stream.Write(Pool.IndexOf(new ConstantUtf8(Name)));
		Stream.Write(Size - 6);
		Write(Stream);
		// DEBUGTRACE: Console.WriteLine($"Finished writing {Name} at {Stream.Position:X}"); // DEBUGTRACE
	}
}