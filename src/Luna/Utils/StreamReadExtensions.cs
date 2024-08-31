using System;
using System.Buffers.Binary;
using System.IO;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Alluseri.Luna.Utils;

internal static class StreamReadExtensions {
	private const uint ChunkSize = 1024 * 100; // Seems reasonable enough :3

	public static byte[] ReadSegment(this Stream Self, int Length) {
		byte[] Data = new byte[Length];
		Self.Read(Data);
		return Data;
	}

	public static byte[] ReadSegment(this Stream Self, uint Length) {
		byte[] Data = new byte[Length];
		Self.Read(Data);
		return Data;
	}

	public static bool ReadByte(this Stream Self, out byte Output) {
		int K = Self.ReadByte();
		Output = (byte) K;
		return K != -1;
	}

	public static bool ReadSByte(this Stream Self, out sbyte Output) {
		int K = Self.ReadByte();
		Output = (sbyte) K;
		return K != -1;
	}

	public static bool ReadSafe(this Stream Self, uint Length, out byte[] Data) {
		if (Length <= ChunkSize) // This won't cause issues with allocation
			return Self.Read(Data = new byte[Length]) == Length;
		else if (Length > 0x7FFFFFC7) { // Not representable in C#
			Data = Array.Empty<byte>();
			return false;
		} else { // Try chunked
			uint FullChunkCount = Length / ChunkSize;
			byte[][] FullChunks = new byte[FullChunkCount][];
			for (uint k = 0; k < FullChunkCount; k++) {
				if (Self.Read(FullChunks[k] = new byte[ChunkSize]) != ChunkSize) { // Hopefully we trip one of these
					Data = Array.Empty<byte>();
					return false;
				}
			}
			// Hopefully we never reach here
			byte[] Remainder = new byte[Length - (FullChunkCount * ChunkSize)];
			if (Self.Read(Remainder) != Remainder.Length) {
				// Thank God
				Data = Array.Empty<byte>();
				return false;
			}
			// Depressing.
			byte[] Full = new byte[Length];
			int FullIndex = 0;

			foreach (byte[] Chunk in FullChunks) {
				Buffer.BlockCopy(Chunk, 0, Full, FullIndex, Chunk.Length);
				FullIndex += Chunk.Length;
			}

			Buffer.BlockCopy(Remainder, 0, Full, FullIndex, Remainder.Length);

			Data = Full;
			return true;
		}
	}


	public static short ReadShort(this Stream Self) {
		Span<byte> Span = stackalloc byte[2];
		Self.Read(Span);
		return BinaryPrimitives.ReadInt16BigEndian(Span);
	}

	public static ushort ReadUShort(this Stream Self) {
		Span<byte> Span = stackalloc byte[2];
		Self.Read(Span);
		return BinaryPrimitives.ReadUInt16BigEndian(Span);
	}

	public static int ReadInt(this Stream Self) {
		Span<byte> Span = stackalloc byte[4];
		Self.Read(Span);
		return BinaryPrimitives.ReadInt32BigEndian(Span);
	}

	public static uint ReadUInt(this Stream Self) {
		Span<byte> Span = stackalloc byte[4];
		Self.Read(Span);
		return BinaryPrimitives.ReadUInt32BigEndian(Span);
	}

	public static long ReadLong(this Stream Self) {
		Span<byte> Span = stackalloc byte[8];
		Self.Read(Span);
		return BinaryPrimitives.ReadInt64BigEndian(Span);
	}

	public static ulong ReadULong(this Stream Self) {
		Span<byte> Span = stackalloc byte[8];
		Self.Read(Span);
		return BinaryPrimitives.ReadUInt64BigEndian(Span);
	}

	public static float ReadFloat(this Stream Self) {
		Span<byte> Span = stackalloc byte[4];
		Self.Read(Span);
		return BinaryPrimitives.ReadSingleBigEndian(Span);
	}

	public static double ReadDouble(this Stream Self) {
		Span<byte> Span = stackalloc byte[8];
		Self.Read(Span);
		return BinaryPrimitives.ReadDoubleBigEndian(Span);
	}



	public static ushort ReadUShort(this Stream Self, out bool Valid) {
		Span<byte> Span = stackalloc byte[2];
		Valid = Self.Read(Span) == 2;
		return BinaryPrimitives.ReadUInt16BigEndian(Span);
	}



	public static bool ReadShort(this Stream Self, out short Value) {
		Span<byte> Span = stackalloc byte[2];
		Value = 0;
		if (Self.Read(Span) != 2)
			return false;
		Value = BinaryPrimitives.ReadInt16BigEndian(Span);
		return true;
	}

	public static bool ReadUShort(this Stream Self, out ushort Value) {
		Span<byte> Span = stackalloc byte[2];
		Value = 0;
		if (Self.Read(Span) != 2)
			return false;
		Value = BinaryPrimitives.ReadUInt16BigEndian(Span);
		return true;
	}

	public static bool ReadInt(this Stream Self, out int Value) {
		Span<byte> Span = stackalloc byte[4];
		Value = 0;
		if (Self.Read(Span) != 4)
			return false;
		Value = BinaryPrimitives.ReadInt32BigEndian(Span);
		return true;
	}

	public static bool ReadUInt(this Stream Self, out uint Value) {
		Span<byte> Span = stackalloc byte[4];
		Value = 0;
		if (Self.Read(Span) != 4)
			return false;
		Value = BinaryPrimitives.ReadUInt32BigEndian(Span);
		return true;
	}

	public static bool ReadLong(this Stream Self, out long Value) {
		Span<byte> Span = stackalloc byte[8];
		Value = 0;
		if (Self.Read(Span) != 8)
			return false;
		Value = BinaryPrimitives.ReadInt64BigEndian(Span);
		return true;
	}

	public static bool ReadULong(this Stream Self, out ulong Value) {
		Span<byte> Span = stackalloc byte[8];
		Value = 0;
		if (Self.Read(Span) != 8)
			return false;
		Value = BinaryPrimitives.ReadUInt64BigEndian(Span);
		return true;
	}

	public static bool ReadFloat(this Stream Self, out float Value) {
		Span<byte> Span = stackalloc byte[4];
		Value = 0;
		if (Self.Read(Span) != 4)
			return false;
		Value = BinaryPrimitives.ReadSingleBigEndian(Span);
		return true;
	}

	public static bool ReadDouble(this Stream Self, out double Value) {
		Span<byte> Span = stackalloc byte[8];
		Value = 0;
		if (Self.Read(Span) != 8)
			return false;
		Value = BinaryPrimitives.ReadDoubleBigEndian(Span);
		return true;
	}
}