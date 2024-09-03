using Alluseri.Luna.Internals;
using System.IO;

namespace Alluseri.Luna.Abstract.Bytecode;

public abstract class PseudoInstruction(bool IsLabel = false) : Instruction(0) {
	public readonly bool IsLabel = IsLabel;

	internal uint Location = 0;

	internal sealed override void Write(Stream Stream, InternalClass Class) { }
	internal sealed override void Checkout(ConstantPool Pool) { }
}