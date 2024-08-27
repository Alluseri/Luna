using Alluseri.Luna.Utils;
using System;
using System.IO;
using System.Linq;

namespace Alluseri.Luna.Internals;

public class BootstrapMethod : ISizeable {
	public readonly ushort MethodHandleIndex;
	public readonly ushort[] ArgumentIndexes;

	public BootstrapMethod(ushort MethodHandleIndex, ushort[] ArgumentIndexes) {
		this.MethodHandleIndex = MethodHandleIndex;
		this.ArgumentIndexes = ArgumentIndexes;
	}

	public int Size => 4 + (2 * ArgumentIndexes.Length);

	public ConstantMethodHandle GetHandle(ConstantPool Pool) => (ConstantMethodHandle) Pool[MethodHandleIndex];
	public ConstantInfo GetArgument(ConstantPool Pool, int Index) => Pool[ArgumentIndexes[Index]];

	public override int GetHashCode() => HashCode.Combine(MethodHandleIndex, ArgumentIndexes);
	public override bool Equals(object? Object) => Object is BootstrapMethod BM && BM.MethodHandleIndex == MethodHandleIndex && BM.ArgumentIndexes.SequenceEqual(ArgumentIndexes);
	public override string ToString() => $"{{ BootstrapMethod #{MethodHandleIndex} [ {GU.ToString(ArgumentIndexes.Select(Index => $"#{Index:X4}"))} ] }}";

	public static bool Parse(Stream Stream, out BootstrapMethod? BootstrapMethod) {
		BootstrapMethod = null;

		if (!Stream.ReadUShort(out ushort MethodHandleIndex) || !Stream.ReadUShort(out ushort ArgumentIndexCount))
			return false;

		ushort[] ArgumentIndexes = new ushort[ArgumentIndexCount];
		for (ushort i = 0; i < ArgumentIndexes.Length; i++) {
			if (!Stream.ReadUShort(out ArgumentIndexes[i])) {
				return false;
			}
		}

		BootstrapMethod = new(MethodHandleIndex, ArgumentIndexes);
		return true;
	}

	public void Write(Stream Stream) {
		Stream.Write(MethodHandleIndex);
		Stream.Write((ushort) ArgumentIndexes.Length);
		foreach (ushort Argi in ArgumentIndexes)
			Stream.Write(Argi);
	}
}