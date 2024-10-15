using Alluseri.Luna.Internals;
using Alluseri.Luna.Utils;
using System.IO;

namespace Alluseri.Luna.Bytecode;

public class InsnPutField : Instruction {
	public string ClassName;
	public FieldDescriptor Field;

	public InsnPutField(string ClassName, FieldDescriptor Field) : base(3) {
		this.ClassName = ClassName;
		this.Field = Field;
	}

	internal override void Write(Stream Stream, CodeBuilder Builder) {
		Stream.Write(Opcode.PutField);
		Stream.Write(Field.CheckoutFieldRef(Builder.Pool, ClassName));
	}

	public override string ToString() => $"putfield {ClassName}.{Field.FullDescriptor}";
}