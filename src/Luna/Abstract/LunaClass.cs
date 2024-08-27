using Alluseri.Luna.Internals;
using System;
using System.Collections.Generic;

namespace Alluseri.Luna.Abstract;

public class LunaClass {
	public ushort JavaVersion;
	public ClassAccessFlags AccessFlags;
	public FullClassNameDescriptor Name;
	public FullClassNameDescriptor? Super = new("java/lang/Object");
	public readonly List<FullClassNameDescriptor> Interfaces = new();
	public readonly List<Field> Fields = new();
	//public readonly List<Method> Methods = new();
	//public readonly List<Annotation> Annotations = new();

	public LunaClass(InternalClass Internal) {
		JavaVersion = Internal.JavaVersion;
		AccessFlags = Internal.AccessFlags;
		Name = new(Internal.GetThisClassName());
		Super = Internal.SuperClass == 0 ? null : new(Internal.GetSuperClassName()!);
	}

	public InternalClass CreateInternal() => throw new NotSupportedException("Not implemented yet!");
}