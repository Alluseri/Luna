using Alluseri.Luna.Utils;
using System;
using System.IO;
using System.Linq;

namespace Alluseri.Luna.Internals;

public class AnnotationInfo : ISizeable {
	public readonly ushort TypeIndex;
	public readonly (ushort NameIndex, ElementValue Value)[] Arguments;

	public AnnotationInfo(ushort TypeIndex, (ushort NameIndex, ElementValue Value)[] Pairs) {
		this.TypeIndex = TypeIndex;
		this.Arguments = Pairs;
	}

	public int Size => 4 + Arguments.Sum(Pair => 2 + Pair.Value.Size);

	public override int GetHashCode() => HashCode.Combine(TypeIndex, Arguments);
	public override bool Equals(object? Object) => Object is AnnotationInfo A && A.TypeIndex == TypeIndex && A.Arguments.SequenceEqual(Arguments);
	public override string ToString() => $"{{ Annotation #{TypeIndex} [ {string.Join(", ", Arguments.Select(Pair => $"< #{Pair.NameIndex}: {Pair.Value} >"))} ] }}";

	public static AnnotationInfo? Parse(Stream Stream) {
		if (!Stream.ReadUShort(out ushort TypeIndex) || !Stream.ReadUShort(out ushort Length))
			return null;

		GU.New(out (ushort NameIndex, ElementValue Value)[] Pairs, Length);
		for (ushort i = 0; i < Length; i++) {
			if (!Stream.ReadUShort(out ushort NameIndex))
				return null;
			if ((Pairs[i] = (NameIndex, ElementValue.Parse(Stream))!).Value == null)
				return null;
		}

		return new(TypeIndex, Pairs);
	}

	public void Write(Stream Stream) {
		Stream.Write(TypeIndex);
		Stream.Write((ushort) Arguments.Length);
		foreach ((ushort NameIndex, ElementValue Value) Pair in Arguments) {
			Stream.Write(Pair.NameIndex);
			Pair.Value.Write(Stream);
		}
	}
}