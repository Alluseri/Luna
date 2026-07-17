using Alluseri.Luna;
using Alluseri.Luna.Analysis;
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

	public abstract override string ToString();

	internal virtual void Checkout(CodeBuilder Builder, int Address) { }
	internal abstract void Write(Stream Stream, CodeBuilder Class, int Address);
}