using Alluseri.Luna.Utils;
using System;
using System.IO;

namespace Alluseri.Luna.Internals;

public class UninitializedVerificationType : VerificationType {
	public readonly ushort NewInsnOffset;

	public UninitializedVerificationType(ushort NewInsnOffset) : base(VerificationTag.Uninitialized) {
		this.NewInsnOffset = NewInsnOffset;
	}

	public override int Size => 3;

	// TODO: I need to have a specific instruction list class that allows you to [] based on the byte index
	// ^ JVM devs are fucking idiots
	// TODO: public Instruction GetInitializer(Instruction[] Insns) => 

	public override int GetHashCode() => HashCode.Combine(Tag, NewInsnOffset);
	public override bool Equals(object? Object) => Object is UninitializedVerificationType VT && VT.NewInsnOffset == NewInsnOffset;
	public override string ToString() => $"{{ UninitializedVerificationType Insn#{NewInsnOffset} }}";

	public override void Write(Stream Stream) {
		Stream.Write((byte) Tag);
		Stream.Write(NewInsnOffset);
	}
}