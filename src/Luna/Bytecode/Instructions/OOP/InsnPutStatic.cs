using Alluseri.Luna.Internals;
using Alluseri.Luna.Utils;
using System.IO;

namespace Alluseri.Luna.Bytecode;

public class InsnPutStatic : Instruction {
	public string ClassName;
	public FieldDescriptor Field;

	public InsnPutStatic(string ClassName, FieldDescriptor Field) : base(3) {
		this.ClassName = ClassName;
		this.Field = Field;
	}

	internal override void Write(Stream Stream, CodeBuilder Builder) {
		Stream.Write(Opcode.PutStatic);
		Stream.Write(Field.CheckoutFieldRef(Builder.Pool, ClassName));
	}

	public override string ToString() => $"putstatic {ClassName}.{Field.FullDescriptor}";
}