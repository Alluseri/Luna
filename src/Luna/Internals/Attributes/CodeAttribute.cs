using Alluseri.Luna.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Alluseri.Luna.Internals;

public class CodeAttribute : AttributeInfo {
	public ushort MaxStackDepth;
	public ushort MaxLocals;
	public byte[] Bytecode;
	public IList<ExceptionHandler> ExceptionTable;
	public IList<AttributeInfo> Attributes;

	public override int Size => 12 + Bytecode.Length + (ExceptionTable.Count * 8) + GU.GetSize(Attributes);

	public CodeAttribute(ushort MaxStackDepth, ushort MaxLocals, byte[] Bytecode, IList<ExceptionHandler> ExceptionTable, IList<AttributeInfo> Attributes) : base("Code") {
		this.MaxStackDepth = MaxStackDepth;
		this.MaxLocals = MaxLocals;
		this.Bytecode = Bytecode;
		this.ExceptionTable = ExceptionTable;
		this.Attributes = Attributes;
	}
	public CodeAttribute(ushort MaxStackDepth, ushort MaxLocals, byte[] Bytecode, ExceptionHandler[] ExceptionTable, AttributeInfo[] Attributes)
	: this(MaxStackDepth, MaxLocals, Bytecode, GU.AsList(ExceptionTable), GU.AsList(Attributes)) { }

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
			// CodeLength > ushort.MaxValue ||
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

		if (!Substream.ReadUShort(out ushort AttributesCount))
			return new MalformedAttribute("Code", Store);

		List<AttributeInfo> Attributes = new(AttributesCount);
		for (ushort j = 0; j < AttributesCount; j++) {
			AttributeInfo? Attr = AttributeInfo.Parse(Substream, Pool);
			if (Attr == null)
				return new MalformedAttribute("Code", Store);
			Attributes.Add(Attr);
		}

		return new CodeAttribute(MaxStack, MaxLocals, Code, Handlers, Attributes);
	}

	public override void Checkout(ConstantPool Pool) {
		base.Checkout(Pool);
		foreach (AttributeInfo Ai in Attributes)
			Ai.Checkout(Pool);
	}

	protected override void Write(Stream Stream) => throw new NotSupportedException($"{Name} has to be written using the Write(Stream, InternalConstantPool) method.");
	public override void Write(Stream Stream, ConstantPool Pool) {
		Stream.Write(Pool.IndexOf(new ConstantUtf8(Name)));
		Stream.Write(Size);
		Stream.Write(MaxStackDepth);
		Stream.Write(MaxLocals);
		Stream.Write(Bytecode.Length); // As int
		Stream.Write(Bytecode);
		Stream.Write((ushort) ExceptionTable.Count);
		foreach (ExceptionHandler Eh in ExceptionTable)
			Eh.Write(Stream);
		Stream.Write((ushort) Attributes.Count);
		foreach (AttributeInfo Ai in Attributes)
			Ai.Write(Stream, Pool);
	}
}