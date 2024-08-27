using Alluseri.Luna.Utils;
using System;
using System.IO;
using System.Linq;

namespace Alluseri.Luna.Internals;

public class ArrayEV : ElementValue {
	public readonly ElementValue[] Values;

	public ArrayEV(ElementValue[] Values) : base('[') {
		this.Values = Values;
	}

	public override int GetHashCode() => HashCode.Combine(nameof(ArrayEV), Values);
	public override bool Equals(object? Object) => Object is ArrayEV IV && IV.Values.SequenceEqual(Values);
	public override string ToString() => $"{{ ArrayEV [ {GU.ToString(Values)} ] }}";
	public override int Size => 3 + GU.GetSize(Values);

	public static ArrayEV? ParseEV(Stream Stream) {
		if (!Stream.ReadUShort(out ushort Length))
			return null;

		ElementValue[] Values = new ElementValue[Length];
		for (int i = 0; i < Values.Length; i++) {
			if ((Values[i] = Parse(Stream)!) == null)
				return null;
		}
		return new(Values);
	}

	protected override void WriteEV(Stream Stream) {
		Stream.Write((ushort) Values.Length);
		foreach (ElementValue Ev in Values)
			Ev.Write(Stream);
	}
}