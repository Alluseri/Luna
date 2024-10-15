using Alluseri.Luna.Internals;
using System.IO;

namespace Alluseri.Luna.Bytecode;

public abstract class PseudoInstruction(bool IsLabel = false) : Instruction(0) {
	public readonly bool IsLabel = IsLabel;

	internal int Location = 0;

	internal sealed override void Write(Stream Stream, CodeBuilder Class) { }
	internal sealed override void Checkout(ConstantPool Pool) { }
}