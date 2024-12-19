using Alluseri.Luna.Bytecode;
using Alluseri.Luna.Internals;
using System;
using System.Collections.Generic;

namespace Alluseri.Luna;

public class LunaClass : IInheritanceUnit {
	public ushort JavaVersion;
	public ClassAccessFlags AccessFlags;
	public string Name;
	public string? Super = "java/lang/Object";
	public readonly List<string> Interfaces = new();
	// public readonly List<Field> Fields = new();
	public readonly List<Method> Methods = new();
	//public readonly List<Annotation> Annotations = new();

	string IInheritanceUnit.Name => Name;
	string? IInheritanceUnit.Super => Super;
	string[] IInheritanceUnit.Interfaces => Interfaces.ToArray();

	public LunaClass(InternalClass Internal) {
		JavaVersion = Internal.JavaVersion;
		AccessFlags = Internal.AccessFlags;
		Name = new(Internal.GetThisClassName());
		Super = Internal.SuperClass == 0 ? null : new(Internal.GetSuperClassName()!);

		foreach (ConstantClass CClass in Internal.GetInterfaces()) {
			Interfaces.Add(CClass.GetName(Internal.ConstantPool)); // verifier will probably explode
		}

		CodeReader Cr = new(Internal);

		foreach (MethodInfo Mi in Internal.Methods)
			Methods.Add(new(Cr, Mi));
	}

	public InternalClass CreateInternal() => throw new NotImplementedException("Not implemented yet!");
}