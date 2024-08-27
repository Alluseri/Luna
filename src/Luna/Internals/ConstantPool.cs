using System;
using System.Collections;
using System.Collections.Generic;

namespace Alluseri.Luna.Internals;

// DESIGN: ConstantPadding is currently internal. Please uninternal it after adding pedantic checks everywhere where possible(incl. IndexOf AND THE GETTER, BECAUSE REFERRING TO A PADDING IS NOT LEGAL!!!)

public class ConstantPool : IEnumerable<ConstantInfo>, IEnumerable {
	private readonly List<ConstantInfo> Pool = new();

	public ConstantInfo Add(ConstantInfo Info) {
		Pool.Add(Info ?? throw new ArgumentNullException(nameof(Info)));
		if (Info.IsWide)
			Pool.Add(new ConstantPadding());
		return Info;
	}

	public ushort Checkout(ConstantInfo Info) {
		ushort U = IndexOf(Info);
		if (U != 0)
			return U;
		else {
			Add(Info);
			return (ushort) (Info.IsWide ? Pool.Count - 1 : Pool.Count);
		}
	}

	public bool IsInRange(int Index) => Index >= 0 && Index < Pool.Count;
	public bool IsInRange(ushort Index) => Index > 0 && Index <= Pool.Count;

	/// <returns>0 when not found, as constant pool starts with 1.</returns>
	public ushort IndexOf(ConstantInfo Info) {
		for (ushort i = 1; i <= Pool.Count; i++) {
			if (Info.Equals(this[i]))
				return i;
		}
		return 0;
	}

	// Have to use Count+1 when iterating over this:
	public ConstantInfo this[ushort Index] => Pool[Index - 1]; // DESIGN: It might be better to return ConstantInfo? here.
	public ConstantInfo this[int Index] => Pool[Index];

	public T Value<T>(ushort Index) where T : ConstantInfo => (T) this[Index];

	public int Count => Pool.Count;

	public IEnumerator<ConstantInfo> GetEnumerator() {
		for (int i = 0; i < Pool.Count; i++) {
			if (Pool[i] is ConstantPadding)
				++i;
			if (i >= Pool.Count)
				yield break;
			yield return Pool[i]!;
		}
	}
	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}