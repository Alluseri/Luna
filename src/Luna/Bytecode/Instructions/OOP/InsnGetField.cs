using Alluseri.Luna.Internals;
using Alluseri.Luna.Utils;
using System.IO;

namespace Alluseri.Luna.Bytecode;

public class InsnGetField : Instruction {
	public string ClassName;
	public FieldDescriptor Field;

	public InsnGetField(string ClassName, FieldDescriptor Field) : base(3) {
		this.ClassName = ClassName;
		this.Field = Field;
	}

	internal override void Write(Stream Stream, CodeBuilder Builder) {
		Stream.Write(Opcode.GetField);
		Stream.Write(Field.CheckoutFieldRef(Builder.Pool, ClassName));
	}

	public override string ToString() => $"getfield {ClassName}.{Field.FullDescriptor}";
}