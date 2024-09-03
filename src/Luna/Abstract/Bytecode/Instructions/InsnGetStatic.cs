using Alluseri.Luna.Internals;
using Alluseri.Luna.Utils;
using System.IO;

namespace Alluseri.Luna.Abstract.Bytecode;

public class InsnGetStatic : Instruction {
	public string ClassName;
	public FieldDescriptor Field;

	public InsnGetStatic(string ClassName, FieldDescriptor Field) : base(3) {
		this.ClassName = ClassName;
		this.Field = Field;
	}

	internal override void Write(Stream Stream, InternalClass Class) {
		Stream.Write(Opcode.GetStatic);
		Stream.Write(Field.CheckoutFieldRef(Class.ConstantPool, ClassName));
	}

	public override string ToString() => $"getstatic {ClassName}.{Field.FullDescriptor}";
}