using Alluseri.Luna.Internals;
using System.IO;

namespace Alluseri.Luna.Abstract.Bytecode;

public class InsnGoto : AbstractSingleBranchInstruction {
	public override bool Conditional => false;

	public InsnGoto(Label Lab) : base(Lab) { }

	internal override void Checkout(ConstantPool Pool) {
		Size = 5;
	}

	internal override void Write(Stream Stream, InternalClass Class) {

	}
}