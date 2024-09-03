using Alluseri.Luna.Internals;
using Alluseri.Luna.Utils;
using System;
using System.IO;

namespace Alluseri.Luna.Abstract.Bytecode;

public abstract class Instruction {
	internal uint Size;

	public Instruction() { }
	public Instruction(uint Size) {
		this.Size = Size;
	}

	protected static uint GetLdcSize(ushort PoolIndex) => PoolIndex > byte.MaxValue ? 3U : 2U; // You are never expected to call this if you're already a WIDE opcode.

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
	internal abstract uint Write(Stream Stream, InternalClass Class, uint Address); // DESIGN: What do I do with the excess amount of internal methods? I don't think I'll ever trust the user with .Write, .Size and .Checkout.
}