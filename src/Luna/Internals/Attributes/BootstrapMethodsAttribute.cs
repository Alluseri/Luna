using Alluseri.Luna.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Alluseri.Luna.Internals;

public class BootstrapMethodsAttribute : AttributeInfo {
	public IList<BootstrapMethod> Content; // TODO: Better name, this one SUCKS

	public override int Size => 2 + GU.GetSize(Content);

	public BootstrapMethodsAttribute(IList<BootstrapMethod> Methods) : base("BootstrapMethods") {
		Content = Methods;
	}
	public BootstrapMethodsAttribute(BootstrapMethod[] Parameters) : this(GU.AsList(Parameters)) { }

	public ushort Checkout(BootstrapMethod Method) {
		checked {
			int Idx = Content.IndexOf(Method);
			if (Idx != -1)
				return (ushort) Idx;
			Idx = Content.Count;
			Content.Add(Method);
			return (ushort) Idx;
		}
	}

	public override int GetHashCode() => HashCode.Combine(Name, Content);
	public override bool Equals(object? Object) => Object is BootstrapMethodsAttribute Attr && Attr.Content.SequenceEqual(Content);
	public override string ToString() => $"{{ BootstrapMethods [ {GU.ToString(Content)} ] }}";

	public static AttributeInfo Parse(Stream Stream) {
		byte[] Store = new byte[Stream.ReadUInt()];
		using MemoryStream Substream = new(Store, 0, Stream.Read(Store));

		if (!Substream.ReadUShort(out ushort BootstrapMethodCount))
			return new MalformedAttribute("BootstrapMethods", Store);

		List<BootstrapMethod> BootstrapMethods = new(BootstrapMethodCount);
		for (ushort i = 0; i < BootstrapMethodCount; i++) {
			if (!BootstrapMethod.Parse(Substream, BootstrapMethods))
				return new MalformedAttribute("BootstrapMethods", Store);
		}

		return new BootstrapMethodsAttribute(BootstrapMethods);
	}

	protected override void Write(Stream Stream) {
		Stream.Write((ushort) Content.Count);
		foreach (BootstrapMethod Bm in Content)
			Bm.Write(Stream);
	}
}