using Alluseri.Luna.Exceptions;
using Alluseri.Luna.Utils;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Alluseri.Luna.Internals;

public class ConstantUtf8 : ConstantInfo { // TODO: Test Chinese characters(UTF-32 surrogates) and confirm their functionality
	public readonly string Value;
	public ConstantUtf8(Stream Stream) : this(Stream.ReadSegment(Stream.ReadUShort())) { }
	public ConstantUtf8(byte[] Bytes) : base(ConstantInfoTag.UTF8) {
		StringBuilder Sb = new();
		for (int i = 0; i < Bytes.Length; i++) {
			byte Byte = Bytes[i];
			// DESIGN: Rewrite this into a switch (Byte) pattern matching thingy?
			if (Byte == 0 || Byte >= 0xf0)
				throw new ConstantPoolException($"Illegal UTF8 literal: byte 0x{Byte:X2} doesn't pass spec checks.");
			else if (Byte >> 7 == 0)
				Sb.Append((char) Byte);
			else if (Byte >> 4 == 14)
				Sb.Append((char) (((Byte & 0xf) << 12) + ((Bytes[++i] & 0x3f) << 6) + (Bytes[++i] & 0x3f)));
			else if (Byte >> 5 == 6)
				Sb.Append((char) (((Byte & 0x1f) << 6) + (Bytes[++i] & 0x3f)));
		}
		Value = Sb.ToString();
	}
	public ConstantUtf8(string Value) : base(ConstantInfoTag.UTF8) {
		this.Value = Value;
	}

	public override int GetHashCode() => HashCode.Combine(Tag, Value);
	public override bool Equals(object? Object) => Object is ConstantUtf8 Constant && Constant.Value == Value;
	public override string ToString() => $"{{ Utf8 \"{Value}\" }}";

	public static explicit operator string(ConstantUtf8 Self) => Self.Value;

	public override void Write(Stream Stream) {
		Stream.Write((byte) ConstantInfoTag.UTF8);
		Stream.Write((ushort) Value.ToCharArray().Sum(C => C == '\x00' ? 2 : C <= '\x7F' ? 1 : C <= '\u07FF' ? 2 : 3)); // HOLY FUCK I HATE JVM SO MUCH I HOPE YOU ALL DIE LMFAO
		foreach (char C in Value.ToCharArray()) {
			if (C == '\x00')
				Stream.Write((ushort) 0xC080);
			else if (C <= '\x7F')
				Stream.Write((byte) C); // And I hate C#, too, don't you worry
			else if (C <= '\u07FF') {
				Stream.WriteByte((byte) (0xC0 | (C >> 6)));
				Stream.WriteByte((byte) (0x80 | (0x3F & C)));
			} else {
				Stream.WriteByte((byte) (0xE0 | (C >> 12)));
				Stream.WriteByte((byte) (0x80 | (0x3F & (C >> 6))));
				Stream.WriteByte((byte) (0x80 | (0x3F & C)));
			}
		}
	}
}