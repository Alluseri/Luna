using Alluseri.Luna.Internals;
using System;

namespace Alluseri.Luna;

[Serializable]
public class BadInstructionReadException : Exception {
	public static BadInstructionReadException StreamUnderread => new("Bytecode stream ended prematurely.");
	public static BadInstructionReadException ExpectedPool(ConstantPool Pool, string Expected, ushort PoolIndex) => new($"Expected {Expected} at #{PoolIndex}, got {Pool[PoolIndex]}.");

	public BadInstructionReadException() { }
	public BadInstructionReadException(string Message) : base(Message) { }
	public BadInstructionReadException(string Message, Exception Inner) : base(Message, Inner) { }
}