using Alluseri.Luna.Utils;
using System;
using System.IO;

namespace Alluseri.Luna.Internals;

public class EnumEV : ElementValue {
	public readonly ushort TypeNameIndex;
	public readonly ushort ConstantNameIndex;

	public EnumEV(ushort TypeNameIndex, ushort ConstantNameIndex) : base('e') {
		this.TypeNameIndex = TypeNameIndex;
		this.ConstantNameIndex = ConstantNameIndex;
	}

	public override int Size => 5;

	public string GetTypeName(ConstantPool Pool) => ((ConstantUtf8) Pool[TypeNameIndex]).Value;
	public string GetConstantName(ConstantPool Pool) => ((ConstantUtf8) Pool[ConstantNameIndex]).Value;

	public override int GetHashCode() => HashCode.Combine(nameof(EnumEV), TypeNameIndex, ConstantNameIndex);
	public override bool Equals(object? Object) => Object is EnumEV IV && IV.TypeNameIndex == TypeNameIndex && IV.ConstantNameIndex == ConstantNameIndex;
	public override string ToString() => $"{{ EnumEV #{TypeNameIndex}, Constant #{ConstantNameIndex} }}";

	public static EnumEV? ParseEV(Stream Stream)
	=> Stream.ReadUShort(out ushort TypeNameIndex) && Stream.ReadUShort(out ushort ConstantNameIndex) ? new(TypeNameIndex, ConstantNameIndex) : null;

	protected override void WriteEV(Stream Stream) {
		Stream.Write(TypeNameIndex);
		Stream.Write(ConstantNameIndex);
	}
}