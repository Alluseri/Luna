using Alluseri.Luna.Internals;
using Alluseri.Luna.Utils;
using System.IO;

namespace Alluseri.Luna.Abstract.Bytecode;

public class InsnGetField : Instruction {
	public override int Size => 3;

	public string ClassName;
	public FieldDescriptor Field;

	public InsnGetField(string ClassName, FieldDescriptor Field) {
		this.ClassName = ClassName;
		this.Field = Field;
	}

	public override void Write(Stream Stream, InternalClass Class) {
		Stream.Write(Opcode.GetField);
		Stream.Write(Field.CheckoutFieldRef(Class.ConstantPool, ClassName));
	}

	public override string ToString() => $"getfield {ClassName}.{Field.FullDescriptor}";
}