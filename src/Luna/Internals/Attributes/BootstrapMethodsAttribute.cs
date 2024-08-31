using Alluseri.Luna.Utils;
using System;
using System.IO;
using System.Linq;

namespace Alluseri.Luna.Internals;

public class BootstrapMethodsAttribute : AttributeInfo {
	public readonly BootstrapMethod[] BootstrapMethods;

	public BootstrapMethodsAttribute(BootstrapMethod[] Parameters) : base("BootstrapMethods", 2 + GU.GetSize(Parameters)) {
		BootstrapMethods = Parameters;
	}

	public override int GetHashCode() => HashCode.Combine(Name, BootstrapMethods);
	public override bool Equals(object? Object) => Object is BootstrapMethodsAttribute Attr && Attr.BootstrapMethods.SequenceEqual(BootstrapMethods);
	public override string ToString() => $"{{ BootstrapMethods [ {GU.ToString(BootstrapMethods)} ] }}";

	public static AttributeInfo Parse(Stream Stream) {
		byte[] Store = new byte[Stream.ReadUInt()];
		using MemoryStream Substream = new(Store, 0, Stream.Read(Store));

		if (!Substream.ReadUShort(out ushort BootstrapMethodCount))
			return new MalformedAttribute("BootstrapMethods", Store);

		BootstrapMethod[] BootstrapMethods = new BootstrapMethod[BootstrapMethodCount];
		for (ushort i = 0; i < BootstrapMethods.Length; i++) {
			if (!BootstrapMethod.Parse(Substream, out BootstrapMethods[i]!))
				return new MalformedAttribute("BootstrapMethods", Store);
		}

		return new BootstrapMethodsAttribute(BootstrapMethods);
	}

	protected override void Write(Stream Stream) {
		Stream.Write((ushort) BootstrapMethods.Length);
		foreach (BootstrapMethod Bm in BootstrapMethods)
			Bm.Write(Stream);
	}
}