using System;

namespace Alluseri.Luna.Abstract;

public class ObjectTypeDescriptor : TypeDescriptor {
	public ObjectTypeDescriptor(string ObjectType) : base($"L{ObjectType};") { } // Requires / separators 
}