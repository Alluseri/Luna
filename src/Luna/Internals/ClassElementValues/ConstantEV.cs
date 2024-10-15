using Alluseri.Luna.Utils;
using System;
using System.IO;
using System.Linq;

namespace Alluseri.Luna.Internals;

public class ConstantEV : ElementValue {
	public readonly Type ConstantType;
	public readonly ushort PoolIndex;

	public ConstantEV(char Tag, ushort PoolIndex) : base(Tag) {
		ConstantType = Tag.GetConstantEV();
		this.PoolIndex = PoolIndex;
	}
	public ConstantEV(Type Tag, ushort PoolIndex) : base(Tag.GetTag()) {
		ConstantType = Tag;
		this.PoolIndex = PoolIndex;
	}

	public ConstantInfo GetValue(ConstantPool Pool) => Pool[PoolIndex];

	public override int GetHashCode() => HashCode.Combine(nameof(ConstantEV), PoolIndex, ConstantType);
	public override bool Equals(object? Object) => Object is ConstantEV IV && IV.ConstantType == ConstantType && IV.PoolIndex == PoolIndex;
	public override string ToString() => $"{{ ConstantEV {ConstantType.GetTag()}({ConstantType}) at #{PoolIndex} }}";
	public override int Size => 3;

	public static ConstantEV? ParseEV(char Tag, Stream Stream) => Stream.ReadUShort(out ushort PoolIndex) ? new(Tag, PoolIndex) : null;

	public enum Type : uint {
		Byte, Char, Double, Float, Int, Long, Short, Boolean, String
	}

	protected override void WriteEV(Stream Stream) {
		Stream.Write(PoolIndex);
	}
}

public static class ConstantEVExtensions {
	private static readonly char[] EVs = new char[] { 'B', 'C', 'D', 'F', 'I', 'J', 'S', 'Z', 's' };

	internal static ConstantEV.Type GetConstantEV(this char Type) => Type switch {
		'B' => ConstantEV.Type.Byte,
		'C' => ConstantEV.Type.Char,
		'D' => ConstantEV.Type.Double,
		'F' => ConstantEV.Type.Float,
		'I' => ConstantEV.Type.Int,
		'J' => ConstantEV.Type.Long,
		'S' => ConstantEV.Type.Short,
		'Z' => ConstantEV.Type.Boolean,
		's' => ConstantEV.Type.String,
		_ => throw new ArgumentException($"The given character('{Type}') doesn't match any of the known types.", nameof(Type))
	};
	internal static bool IsValidConstantTag(this char Type) => EVs.Contains(Type);
	public static char GetTag(this ConstantEV.Type Type) => Type <= ConstantEV.Type.String ? EVs[(uint) Type] : throw new ArgumentException($"The given type({Type}) is unknown and does not have a tag.", nameof(Type));
}