using Alluseri.Luna.Internals;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Alluseri.Luna.Utils; // DESIGN: Or does it belong elsewhere?

public abstract class AttributeCollection : ICollection<AttributeInfo> {
	protected Dictionary<string, LinkedList<AttributeInfo>> Attributes = new();

	private int _CacheCount = 0; // TODO: UPDATE CACHE WHERE RELEVANT ARGH GFDGHDFGHH
	public int Count => _CacheCount;

	public bool IsReadOnly => false;

	public AttributeCollection() { }
	public AttributeCollection(AttributeInfo[] Infos) {
		for (int i = 0; i < Infos.Length; i++)
			Add(Infos[i]);
	}
	public AttributeCollection(IEnumerable<AttributeInfo> Infos) {
		foreach (AttributeInfo Info in Infos)
			Add(Info);
	}

	public void Add(AttributeInfo Info) => Attributes.GetOrNew(Info.Name).AddLast(Info);
	public void AddFirst(AttributeInfo Info) => Attributes.GetOrNew(Info.Name).AddFirst(Info);

	public void Clear() => Attributes.Clear();

	public bool Contains(AttributeInfo Info) => Attributes.TryGetValue(Info.Name, out var InfoList) && InfoList.Contains(Info);
	public bool Remove(AttributeInfo Info) => Attributes.TryGetValue(Info.Name, out var InfoList) && InfoList.Remove(Info);
	public void CopyTo(AttributeInfo[] Array, int ArrayIndex) => throw new NotImplementedException("was too lazy to implement, please make an issue or pr if you want to use this");

	IEnumerator<AttributeInfo> IEnumerable<AttributeInfo>.GetEnumerator() {
		foreach (LinkedList<AttributeInfo> List in Attributes.Values) {
			foreach (AttributeInfo Attr in List) {
				yield return Attr;
			}
		}
	}
	IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<AttributeInfo>) this).GetEnumerator();
}