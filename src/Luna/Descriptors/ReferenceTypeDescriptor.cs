using Alluseri.Luna.Internals;
using Alluseri.Luna.Utils;
using System;

namespace Alluseri.Luna;

public abstract class ReferenceTypeDescriptor : TypeDescriptor {
	protected ReferenceTypeDescriptor(string Term) : base(Term) { }

	public virtual string SymbolicTerm => Term;
	public ushort CheckoutSymbolic(ConstantPool Pool) => Pool.Checkout(new ConstantClass(Pool.CheckoutUTF8(SymbolicTerm)));

	public static ReferenceTypeDescriptor ParseSymbolic(string Value) => (ReferenceTypeDescriptor?) ArrayTypeDescriptor.Parse(Value) ?? new ObjectTypeDescriptor(Value);
}