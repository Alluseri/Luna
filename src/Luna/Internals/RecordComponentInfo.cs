using Alluseri.Luna.Utils;
using System.Collections.Generic;
using System.IO;

namespace Alluseri.Luna.Internals;

public record RecordComponentInfo(ushort NameIndex, ushort DescriptorIndex, IList<AttributeInfo> Attributes) {
	public string GetName(ConstantPool Pool) => ((ConstantUtf8) Pool[NameIndex]).Value;
	public string GetDescriptor(ConstantPool Pool) => ((ConstantUtf8) Pool[DescriptorIndex]).Value;

	public override string ToString() => $"{{ RecordComponent {NameIndex}:{DescriptorIndex} [ {GU.ToString(Attributes)} ] }}";

	public void Write(Stream Stream, ConstantPool Pool) {
		Stream.Write(NameIndex);
		Stream.Write(DescriptorIndex);
		Stream.Write((ushort) Attributes.Count);
		foreach (AttributeInfo Ai in Attributes)
			Ai.Write(Stream, Pool);
	}
}