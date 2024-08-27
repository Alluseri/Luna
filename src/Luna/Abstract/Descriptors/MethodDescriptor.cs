using Alluseri.Luna.Internals;

namespace Alluseri.Luna.Abstract;

// DESIGN: One or more of these constructors should not make it into prod. I think.

public class MethodDescriptor : Descriptor {
	public TypeDescriptor ReturnType;
	public UnqualifiedDescriptor Name;
	public CompoundTypeDescriptor Arguments;

	public override string Term => $"{Name}({Arguments}){ReturnType}";
	public string JvmType => $"({Arguments}){ReturnType}";// TODO: This definitely needs a better name.

	public MethodDescriptor(TypeDescriptor ReturnType, UnqualifiedDescriptor Name, CompoundTypeDescriptor Arguments) { // Java-style
		this.ReturnType = ReturnType;
		this.Name = Name;
		this.Arguments = Arguments;
	}

	public MethodDescriptor(ConstantPool Pool, ConstantNameAndType Signature) { // DESIGN: Or .From instead?
		ReturnType = TypeDescriptor.Parse(Signature.GetDescriptor(Pool)); // Well... I think I need a both-descriptor here... fuck
		Name = new(Signature.GetName(Pool));
	}

	public MethodDescriptor(UnqualifiedDescriptor Name, CompoundTypeDescriptor Arguments, TypeDescriptor ReturnType) : this(ReturnType, Name, Arguments) { } // LL-style
	public MethodDescriptor(UnqualifiedDescriptor Name, TypeDescriptor ReturnType, CompoundTypeDescriptor Arguments) : this(ReturnType, Name, Arguments) { } // LL-style but made for humans
}