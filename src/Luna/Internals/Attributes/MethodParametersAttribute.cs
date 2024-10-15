using Alluseri.Luna.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Alluseri.Luna.Internals;

public class MethodParametersAttribute : AttributeInfo {
	public IList<MethodParameter> Parameters;

	public override int Size => 1 + (4 * Parameters.Count);

	public MethodParametersAttribute(IList<MethodParameter> Parameters) : base("MethodParameters") {
		this.Parameters = Parameters;
	}
	public MethodParametersAttribute(MethodParameter[] Parameters) : this(GU.AsList(Parameters)) { }

	public override int GetHashCode() => HashCode.Combine(Name, Parameters);
	public override bool Equals(object? Object) => Object is MethodParametersAttribute Attr && Attr.Parameters.SequenceEqual(Parameters);
	public override string ToString() => $"{{ MethodParameters [ {string.Join(", ", Parameters.Select(Parameter => $"{{ #{Parameter.NameIndex}: 0x{Parameter.AccessFlags:X4} }}"))} ] }}";

	public static AttributeInfo Parse(Stream Stream) {
		byte[] Store = new byte[Stream.ReadUInt()];
		using MemoryStream Substream = new(Store, 0, Stream.Read(Store));

		int ParameterCount = Substream.ReadByte();
		if (ParameterCount == -1)
			return new MalformedAttribute("MethodParameters", Store);

		List<MethodParameter> Mps = new(ParameterCount);

		for (byte i = 0; i < ParameterCount; i++) {
			if (
				!Substream.ReadUShort(out ushort NameIndex) ||
				!Substream.ReadUShort(out ushort AccessFlags)
			)
				return new MalformedAttribute("MethodParameters", Store);
			Mps.Add(new(NameIndex, (MethodParameterAccessFlags) AccessFlags));
		}

		return new MethodParametersAttribute(Mps);
	}

	protected override void Write(Stream Stream) {
		Stream.Write((byte) Parameters.Count);
		foreach (MethodParameter Mp in Parameters) {
			Stream.Write(Mp.NameIndex);
			Stream.Write((ushort) Mp.AccessFlags);
		}
	}
}

// DESIGN: What to do with these lingering units? Keep them here or move into a separate class?

public readonly record struct MethodParameter(ushort NameIndex, MethodParameterAccessFlags AccessFlags) {
	public string GetName(ConstantPool Pool) => ((ConstantUtf8) Pool[NameIndex]).Value;
}

[Flags]
public enum MethodParameterAccessFlags : ushort {
	Final = 0x0010,
	Synthetic = 0x1000,
	Mandated = 0x8000
}