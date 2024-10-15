using Alluseri.Luna.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Alluseri.Luna.Internals;

public class ModuleHashesAttribute : AttributeInfo {
	public ushort AlgorithmIndex;
	public IList<ModuleHash> Hashes;

	public override int Size => 4 + Hashes.Sum(H => 4 + H.Hash.Length);

	public ModuleHashesAttribute(ushort AlgorithmIndex, IList<ModuleHash> Hashes) : base("ModuleHashes") {
		this.AlgorithmIndex = AlgorithmIndex;
		this.Hashes = Hashes;
	}
	public ModuleHashesAttribute(ushort AlgorithmIndex, ModuleHash[] Hashes) : this(AlgorithmIndex, new List<ModuleHash>(Hashes)) { }

	public override int GetHashCode() => HashCode.Combine(Name, AlgorithmIndex, Hashes);
	public override bool Equals(object? Object) => Object is ModuleHashesAttribute Attr && Attr.AlgorithmIndex == AlgorithmIndex && Attr.Hashes.SequenceEqual(Hashes);
	public override string ToString() => $"{{ ModuleHashes Algorithm #{AlgorithmIndex} [ {GU.ToString(Hashes)} ] }}";

	public static AttributeInfo Parse(Stream Stream) {
		byte[] Store = new byte[Stream.ReadUInt()];
		using MemoryStream Substream = new(Store, 0, Stream.Read(Store));

		if (!Substream.ReadUShort(out ushort AlgorithmIndex) || !Substream.ReadUShort(out ushort HashesCount))
			return new MalformedAttribute("ModuleHashes", Store);

		List<ModuleHash> Hashes = new(HashesCount);
		for (ushort i = 0; i < HashesCount; i++) {
			if (!Substream.ReadUShort(out ushort ModuleNameIndex) || !Substream.ReadUShort(out ushort HashLength))
				return new MalformedAttribute("ModuleHashes", Store);

			if (!Substream.ReadSafe(HashLength, out byte[] Hash))
				return new MalformedAttribute("ModuleHashes", Store);

			Hashes.Add(new ModuleHash(ModuleNameIndex, Hash));
		}

		return new ModuleHashesAttribute(AlgorithmIndex, Hashes);
	}

	protected override void Write(Stream Stream) {
		Stream.Write(AlgorithmIndex);
		Stream.Write((ushort) Hashes.Count);
		foreach (ModuleHash Mh in Hashes)
			Mh.Write(Stream);
	}
}

public readonly record struct ModuleHash(ushort ModuleNameIndex, byte[] Hash) {
	public string GetModuleName(ConstantPool Pool) => ((ConstantModule) Pool[ModuleNameIndex]).GetName(Pool);
	public string GetAlgorithm(ConstantPool Pool) => ((ConstantUtf8) Pool[ModuleNameIndex]).Value;

	public override string ToString() => $"{{ ModuleHash #{ModuleNameIndex} [ {Convert.ToHexString(Hash)} ] }}";

	public void Write(Stream Stream) {
		Stream.Write(ModuleNameIndex);
		Stream.Write((ushort) Hash.Length);
		Stream.Write(Hash);
	}
}