using Alluseri.Luna.Bytecode;
using Alluseri.Luna.Internals;
using System.Collections.Generic;
using System.Linq;

namespace Alluseri.Luna;

public class Method {
	public MethodAccessFlags AccessFlags;
	public MethodDescriptor Descriptor;
	public List<Instruction>? Code; // CodeAttribute
	public IList<string> Throws; // ExceptionsAttribute; should we really have forced alloc here?
	public bool Deprecated; // DeprecatedAttribute

	public Method(CodeReader Reader, MethodInfo InternalMethod) {
		AccessFlags = InternalMethod.AccessFlags;
		Descriptor = MethodDescriptor.FromSignature(InternalMethod.GetName(Reader.Class.ConstantPool), InternalMethod.GetDescriptor(Reader.Class.ConstantPool));

		Code = InternalMethod.Attributes.FirstOrDefault(k => k is CodeAttribute) is CodeAttribute Ca ? Reader.Read(Ca) : null;

		ExceptionsAttribute? Ex = InternalMethod.Attributes.FirstOrDefault(k => k is ExceptionsAttribute) as ExceptionsAttribute;
		if (Ex != null) { // TODO: Is this yip yap necessary or the verifier will explode anyway?
			Throws = new List<string>(Ex.ExceptionIndexes.Count);
			foreach (ushort PoolIndex in Ex.ExceptionIndexes) {
				if (Reader.Class.ConstantPool[PoolIndex] is ConstantClass CC) {
					Throws.Add(CC.GetName(Reader.Class.ConstantPool));
				}
			}
		} else
			Throws = new List<string>(0);

		Deprecated = InternalMethod.Attributes.Any(k => k is DeprecatedAttribute); // This is supposed to use annotations really
	}

	// AnnotationDefaultAttribute
	// MethodParametersAttribute
	// SyntheticAttribute
	// SignatureAttribute

	// (In)VisibleParameterAnnotations (needs abstract Annotation)
	// (In)VisibleAnnotations (needs abstract Annotation)
	// (In)VisibleTypeAnnotations (needs abstract Annotation and the attribute itself lol)
}