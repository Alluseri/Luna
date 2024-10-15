using Alluseri.Luna.Bytecode;
using System;
using System.Buffers.Binary;
using System.IO;
using System.Numerics;
using System.Text.Json.Nodes;

namespace Alluseri.Luna.Utils;

internal static class StreamWriteExtensions {
	public static void Write(this Stream Self, Opcode Data) { // I have the right to do this and I know it's dumb as fuck.
		Self.WriteByte((byte) Data);
	}

	public static void Write(this Stream Self, Opcode Data, int Offset) => Self.WriteByte((byte) (((byte) Data) + Offset));

	public static void Write(this Stream Self, Opcode Data, uint Offset) => Self.WriteByte((byte) (((byte) Data) + Offset));

	public static void Write(this Stream Self, short Data) {
		Span<byte> Span = stackalloc byte[2];
		BinaryPrimitives.WriteInt16BigEndian(Span, Data);
		Self.Write(Span);
	}

	public static void Write(this Stream Self, ushort Data) {
		Span<byte> Span = stackalloc byte[2];
		BinaryPrimitives.WriteUInt16BigEndian(Span, Data);
		Self.Write(Span);
	}

	public static void Write(this Stream Self, int Data) {
		Span<byte> Span = stackalloc byte[4];
		BinaryPrimitives.WriteInt32BigEndian(Span, Data);
		Self.Write(Span);
	}

	public static void Write(this Stream Self, uint Data) {
		Span<byte> Span = stackalloc byte[4];
		BinaryPrimitives.WriteUInt32BigEndian(Span, Data);
		Self.Write(Span);
	}

	public static void Write(this Stream Self, long Data) {
		Span<byte> Span = stackalloc byte[8];
		BinaryPrimitives.WriteInt64BigEndian(Span, Data);
		Self.Write(Span);
	}

	public static void Write(this Stream Self, ulong Data) {
		Span<byte> Span = stackalloc byte[8];
		BinaryPrimitives.WriteUInt64BigEndian(Span, Data);
		Self.Write(Span);
	}

	public static void Write(this Stream Self, float Data) {
		Span<byte> Span = stackalloc byte[4];
		BinaryPrimitives.WriteSingleBigEndian(Span, Data);
		Self.Write(Span);
	}

	public static void Write(this Stream Self, double Data) {
		Span<byte> Span = stackalloc byte[8];
		BinaryPrimitives.WriteDoubleBigEndian(Span, Data);
		Self.Write(Span);
	}

	public static void Write(this Stream Self, byte Data) => Self.WriteByte(Data);
	public static void Write(this Stream Self, sbyte Data) => Self.WriteByte((byte) Data);
}