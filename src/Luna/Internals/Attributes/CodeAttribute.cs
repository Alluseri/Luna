using Alluseri.Luna.Utils;
using System;
using System.IO;
using System.Linq;

namespace Alluseri.Luna.Internals;

public class CodeAttribute : AttributeInfo {
	public readonly ushort MaxStackDepth;
	public readonly ushort MaxLocals;
	public readonly byte[] Bytecode;
	public readonly ExceptionHandler[] ExceptionTable;
	public readonly AttributeInfo[] Attributes;

	public CodeAttribute(ushort MaxStackDepth, ushort MaxLocals, byte[] Bytecode, ExceptionHandler[] ExceptionTable, AttributeInfo[] Attributes)
		: base("Code", 12 + Bytecode.Length + (ExceptionTable.Length * 8) + GU.GetSize(Attributes)) {
		this.MaxStackDepth = MaxStackDepth;
		this.MaxLocals = MaxLocals;
		this.Bytecode = Bytecode;
		this.ExceptionTable = ExceptionTable;
		this.Attributes = Attributes;
	}

	public override int GetHashCode() => HashCode.Combine(Name, MaxStackDepth, MaxLocals, Bytecode, ExceptionTable, Attributes);
	public override bool Equals(object? Object) => Object is CodeAttribute Attr && Attr.MaxStackDepth == MaxStackDepth && Attr.MaxLocals == MaxLocals && Attr.Bytecode.SequenceEqual(Bytecode) && Attr.ExceptionTable.SequenceEqual(ExceptionTable) && Attr.Attributes.SequenceEqual(Attributes);
	public override string ToString() => $"{{ Code Stack[{MaxStackDepth}], Locals[{MaxLocals}], Bytecode[{Bytecode.Length}], Exceptions [ {GU.ToString(ExceptionTable)} ], Attributes [ {GU.ToString(Attributes)} ] }}";

	public static AttributeInfo ParseCode(Stream Stream, ConstantPool Pool) {
		byte[] Store = new byte[Stream.ReadUInt()];
		using MemoryStream Substream = new(Store, 0, Stream.Read(Store));

		if (
			!Substream.ReadUShort(out ushort MaxStack) ||
			!Substream.ReadUShort(out ushort MaxLocals) ||
			!Substream.ReadUInt(out uint CodeLength) ||
			!Substream.ReadSafe(CodeLength, out byte[] Code) ||
			!Substream.ReadUShort(out ushort ExceptionTableLength)
		)
			return new MalformedAttribute("Code", Store);

		ExceptionHandler[] Handlers = new ExceptionHandler[ExceptionTableLength];
		for (ushort i = 0; i < ExceptionTableLength; i++) {
			if (
				!Substream.ReadUShort(out ushort Start) ||
				!Substream.ReadUShort(out ushort End) ||
				!Substream.ReadUShort(out ushort Handler) ||
				!Substream.ReadUShort(out ushort CatchTypeIndex)
			)
				return new MalformedAttribute("Code", Store);

			Handlers[i] = new(Start, End, Handler, CatchTypeIndex);
		}

		if (!Substream.ReadUShort(out ushort AttributeCount))
			return new MalformedAttribute("Code", Store);

		AttributeInfo[] Ai = new AttributeInfo[AttributeCount];
		for (ushort i = 0; i < AttributeCount; i++) {
			// TODO: Handle nulls and malforms separately since malforms can be put into attributes and keep going, nulls mean end of stream for sure.
			// This needs proper AttributeInfo array sizing since it might decide to break in some stupid edge cases.
			if ((Ai[i] = AttributeInfo.Parse(Substream, Pool)!) == null || Ai[i] is MalformedAttribute) {
				AttributeInfo[] Backup = Ai;
				Ai = new AttributeInfo[i];
				Array.Copy(Backup, Ai, i);
				break;
			}
		}

		return new CodeAttribute(MaxStack, MaxLocals, Code, Handlers, Ai);
	}

	public override void Checkout(ConstantPool Pool) {
		base.Checkout(Pool);
		foreach (AttributeInfo Ai in Attributes)
			Ai.Checkout(Pool);
	}

	protected override void Write(Stream Stream) => throw new NotSupportedException($"{Name} has to be written using the Write(Stream, InternalConstantPool) method.");
	public override void Write(Stream Stream, ConstantPool Pool) {
		Stream.Write(Pool.IndexOf(new ConstantUtf8(Name)));
		Stream.Write(Size - 6);
		Stream.Write(MaxStackDepth);
		Stream.Write(MaxLocals);
		Stream.Write(Bytecode.Length); // As int
		Stream.Write(Bytecode);
		Stream.Write((ushort) ExceptionTable.Length);
		foreach (ExceptionHandler Eh in ExceptionTable)
			Eh.Write(Stream);
		Stream.Write((ushort) Attributes.Length);
		foreach (AttributeInfo Ai in Attributes)
			Ai.Write(Stream, Pool);
	}
}