using Alluseri.Luna;
using Alluseri.Luna.Internals;
using Alluseri.Luna.Utils;
using System;
using System.IO;

namespace Alluseri.Luna.Bytecode;

public abstract class Instruction {
	internal int Size;

	public Instruction() { }
	public Instruction(int Size) {
		this.Size = Size;
	}

	protected static int GetLdcSize(ushort PoolIndex) => PoolIndex > byte.MaxValue ? 3 : 2; // You are never expected to call this if you're already a WIDE opcode.

	protected static Stream Ldc(Stream Stream, ushort PoolIndex, bool Wide = false) {
		if (Wide || PoolIndex > byte.MaxValue) {
			Stream.WriteByte((byte) (Wide ? Opcode.Ldc2_W : Opcode.Ldc_W));
			Stream.Write(PoolIndex);
		} else {
			Stream.Write(Opcode.Ldc);
			Stream.Write((byte) PoolIndex);
		}
		return Stream;
	}

	public abstract override string ToString();

	internal virtual void Checkout(ConstantPool Pool) { }
	internal abstract void Write(Stream Stream, CodeBuilder Class);
}