using Alluseri.Luna.Internals;
using System.Collections.Generic;
using System.IO;
using System.Net.Http.Headers;

namespace Alluseri.Luna.Bytecode;

public class CodeBuilder { // Perfect use case: One CodeBuilder per Class
	public ConstantPool Pool;
	public ClassAttributeCollection Attributes;

	public CodeBuilder(ConstantPool Pool) {

	}
	public CodeBuilder(ConstantPool Pool, ClassAttributeCollection Attributes) {

	}

	/*private void ResolveGotoDeep(int Index, ) {
		// Uses recursion
	}*/

	public byte[] Build(IEnumerable<Instruction> InstructionList) {
		int Location = 0;

		foreach (Instruction Insn in InstructionList) { // First pass: update pseudo locations
			if (Insn is PseudoInstruction Pi) {
				Pi.Location = Location; // Update the pseudo's location
			} else {
				Insn.Checkout(Pool); // Size is updated and safe to access
				Location += Insn.Size;
			}
		}

		// TODO: Check if the size of our method exceeds 65536.
		// TODO: Check the exception table edge-case when method size is 65535.

		// All labels are now updated and when referenced in code they will point to proper indexes in code

		byte[] Bytecode = new byte[Location];

		int Address = 0;
		using (MemoryStream Writer = new(Bytecode)) { // This will crash if we try to expand, which is great for debugging.
			foreach (Instruction Insn in InstructionList) {
				if (Insn is AbstractSingleBranchInstruction BrInsn)
					BrInsn.Write(Writer, this, Address);
				else
					Insn.Write(Writer, this);
				Address += Insn.Size;
			}
		}

		return Bytecode;
	}
}

public class CodeBuilderConfig {
	public bool ComplexGotoResolution = false;
	public bool RepairLabelIdentities = false;
}