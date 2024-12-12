using Alluseri.Luna.Internals;
using Alluseri.Luna.Utils;

namespace Alluseri.Luna; // DESIGN: What namespace is this wtf...

#pragma warning disable IDE0008

public class ClassAttributeCollection : AttributeCollection {
	public BootstrapMethodsAttribute? BootstrapMethods => Attributes.TryGetValue("BootstrapMethods", out var AttrList) ? AttrList.First?.Value as BootstrapMethodsAttribute : null;
}