using Alluseri.Luna.Internals;
using System.Collections.Generic;
using System.IO;

namespace Alluseri.Luna.Abstract.Bytecode;

// DESIGN: Move to Method.cs?

public class CodeBuilder {
	private InternalClass Class;

	public CodeBuilder(InternalClass Class) {
		this.Class = Class;
	}

	/*
	GOTO needs me to know the position of a label which it can singlehandedly offset forward or backward.
	This way is the ONLY way of figuring out instructions' locations. And it relies on instructions checking out.
	GOTO cannot check out, because a future instruction does NOT have its info updated reliably for reading.
	I cannot add a second checkout pass for GOTO specifically because it'd still point to the wrong location.
	I cannot "assume" the maximum size for GOTO during the first pass because if it decides it wants to be the short form.
	AND THIS FUCKING SHIT IS RELATIVE TOO.
	*/

	public byte[] Build(IEnumerable<Instruction> InstructionList) {
		uint Location = 0;

		foreach (Instruction Insn in InstructionList) { // First pass: update pseudo locations
			if (Insn is PseudoInstruction Pi) {
				Pi.Location = Location; // Update the pseudo's location
			} else {
				Insn.Checkout(Class.ConstantPool); // Size is updated and safe to access
				Location += Insn.Size;
			}
		}

		// All labels are now updated and when referenced in code they will point to proper indexes in code

		byte[] Bytecode = new byte[Location];

		uint Address = 0;
		using (MemoryStream Writer = new(Bytecode)) { // This will crash if we try to expand, which is great for debugging.
			foreach (Instruction Insn in InstructionList) {
				Address += Insn.Write(Writer, Class, Address);
			}
		}
		return Bytecode;
	}
}