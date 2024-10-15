using Alluseri.Luna.Internals;
using Alluseri.Luna.Utils;
using System.IO;

namespace Alluseri.Luna.Bytecode;

public class InsnGetStatic : Instruction {
	public string ClassName;
	public FieldDescriptor Field;

	public InsnGetStatic(string ClassName, FieldDescriptor Field) : base(3) {
		this.ClassName = ClassName;
		this.Field = Field;
	}

	internal override void Write(Stream Stream, CodeBuilder Builder) {
		Stream.Write(Opcode.GetStatic);
		Stream.Write(Field.CheckoutFieldRef(Builder.Pool, ClassName));
	}

	public override string ToString() => $"getstatic {ClassName}.{Field.FullDescriptor}";
}