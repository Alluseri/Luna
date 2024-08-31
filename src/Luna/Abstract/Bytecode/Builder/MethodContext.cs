using Alluseri.Luna.Internals;
using System.Collections.Generic;
using System.IO;

namespace Alluseri.Luna.Abstract.Bytecode;

public class CodeBuilder {
	private InternalClass Class;

	public CodeBuilder(InternalClass Class) {
		this.Class = Class;
	}

	/*public void Write(Stream Stream, IEnumerable<Instruction> InstructionList) {
		Dictionary<byte, LLabel> Labs = new();

		uint Loc = 0;
		foreach (Instruction Insn in InstructionList) {
			if (Insn is LunaLabel Lab) {

			} else {
				Insn.Checkout(Class); // Private/internal Size
				Loc += Insn.Size;
			}
		}
	}*/
}