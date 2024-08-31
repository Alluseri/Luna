using Alluseri.Luna.Utils;
using System.IO;

namespace Alluseri.Luna.Internals;

public readonly record struct RecordComponentInfo(ushort NameIndex, ushort DescriptorIndex, AttributeInfo[] Attributes) {
	public string GetName(ConstantPool Pool) => ((ConstantUtf8) Pool[NameIndex]).Value;
	public string GetDescriptor(ConstantPool Pool) => ((ConstantUtf8) Pool[DescriptorIndex]).Value;

	public override string ToString() => $"{{ RecordComponent {NameIndex}:{DescriptorIndex} [ {GU.ToString(Attributes)} ] }}";

	public void Write(Stream Stream, ConstantPool Pool) {
		Stream.Write(NameIndex);
		Stream.Write(DescriptorIndex);
		Stream.Write((ushort) Attributes.Length);
		foreach (AttributeInfo Ai in Attributes)
			Ai.Write(Stream, Pool);
	}
}