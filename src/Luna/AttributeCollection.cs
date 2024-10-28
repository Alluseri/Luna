using Alluseri.Luna.Internals;
using Alluseri.Luna.Utils;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Alluseri.Luna;

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

	public void Add(AttributeInfo Info) {
		Attributes.GetOrNew(Info.Name).AddLast(Info);
		_CacheCount++;
	}
	public void AddFirst(AttributeInfo Info) {
		Attributes.GetOrNew(Info.Name).AddFirst(Info);
		_CacheCount++;
	}

	public void Clear() {
		Attributes.Clear();
		_CacheCount = 0;
	}

	public bool Contains(AttributeInfo Info) => Attributes.TryGetValue(Info.Name, out var InfoList) && InfoList.Contains(Info);
	public bool Remove(AttributeInfo Info) {
		if (Attributes.TryGetValue(Info.Name, out var InfoList)) {
			if (InfoList.Remove(Info)) {
				_CacheCount--;
				return true;
			}
		}
		return false;
	}
	public void CopyTo(AttributeInfo[] Array, int ArrayIndex) {
		ArgumentOutOfRangeException.ThrowIfNegative(ArrayIndex);

		int i = ArrayIndex;
		foreach (AttributeInfo Info in this) {
			if (ArrayIndex >= Array.Length)
				throw new ArgumentException("There was not enough space in the target array from the given starting position to the array's end.");
			Array[i++] = Info;
		}
	}

	IEnumerator<AttributeInfo> IEnumerable<AttributeInfo>.GetEnumerator() {
		foreach (LinkedList<AttributeInfo> List in Attributes.Values) {
			foreach (AttributeInfo Attr in List) {
				yield return Attr;
			}
		}
	}
	IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<AttributeInfo>) this).GetEnumerator();
}