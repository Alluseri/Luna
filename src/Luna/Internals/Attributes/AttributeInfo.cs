using Alluseri.Luna.Utils;
using System;
using System.IO;

namespace Alluseri.Luna.Internals;

// TODO: Internal state checking for attributes like BootstrapMethods which have limited-size(ushort) lists

public abstract class AttributeInfo : ISizeable {
	public readonly string Name;
	public abstract int Size { get; } // This is regular attribute size, used for basically everything
	int ISizeable.Size => Size + 6; // This is on-disk size, only available for explicit ISizeable users, this is needed for Code, Record and other AttributeInfo writers to work properly

	public AttributeInfo(string Name) {
		this.Name = Name;
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
		Stream.Write(Size);
		Write(Stream);
		// DEBUGTRACE: Console.WriteLine($"Finished writing {Name} at {Stream.Position:X}"); // DEBUGTRACE
	}
}