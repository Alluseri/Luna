using Alluseri.Luna.Exceptions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq;

namespace Alluseri.Luna.Utils;

public class LinkableList<T> : IEnumerable<T> where T : Linkable<T> { // TODO: When ready, make extend IList; thread safety?
	private List<T> Backend;
	public bool Locked { get; private set; }

	public LinkableList() {
		Backend = new();
	}
	public LinkableList(int Capacity) {
		Backend = new(Capacity);
	}
	public LinkableList(uint Capacity) : this((int) Capacity) { }

	public void Lock() => Locked = true;

	private void AssertLocked() {
		if (Locked)
			throw new StateException("Cannot perform modifications on a locked LinkableList.");
	}
	private T AssertForeignRoot(T Node) => Node.Root == this ? Node : throw new ArgumentException("The given Linkable doesn't belong to this LinkableList.", nameof(Node));
	private T AssertForeignNode(T Node) => AssertForeignRoot(AssertNull(Node));
	private static T AssertRoot(T Node) => Node.Root == null ? Node : throw new ArgumentException("The given Linkable is already rooted.", nameof(Node));
	private static T AssertNode(T Node) => AssertRoot(AssertNull(Node));
	private static T AssertNull(T Node) => Node ?? throw new ArgumentNullException(nameof(Node), "The given node is null.");

	public int Append(T New) {
		AssertLocked();
		Backend.Add(AssertNode(New));
		New.Root = this;
		return New.LinkIndex = Backend.Count - 1;
	}
	public void Insert(T New, int Index) {
		AssertLocked();
		Backend.Insert(Index, AssertNode(New));
		New.Root = this;
		New.LinkIndex = Index;
		for (int i = Index + 1; i < Backend.Count; i++) {
			Backend[i].LinkIndex++;
		}
	}
	public void Remove(T Node) {
		AssertLocked();
		AssertForeignNode(Node).Root = null;
		int I = Node.LinkIndex;
		Node.LinkIndex = -1;
		Backend.Remove(Node);
		for (int i = I; i < Backend.Count; i++) {
			Backend[i].LinkIndex--;
		}
	}
	public void Replace(T Old, T New) {
		AssertLocked();
		int I = AssertForeignNode(Old).LinkIndex;
		AssertNull(New).Assign(null);
		Backend.RemoveAt(I);
		Backend.Insert(I, New);
		New.Root = this;
		New.LinkIndex = I;
		Old.Root = null;
		Old.LinkIndex = -1;
	}
	public void Swap(T Old, T New) {
		AssertLocked();
		int O = AssertForeignNode(Old).LinkIndex;
		int N = AssertForeignNode(New).LinkIndex;
		Backend.RemoveAt(O);
		Backend.Insert(O, New);
		Backend.RemoveAt(N);
		Backend.Insert(N, Old);
		Old.LinkIndex = N;
		New.LinkIndex = O;
	}
	public int IndexOf(T Node) {
		for (int i = 0; i < Backend.Count; i++)
			if (ReferenceEquals(Backend[i], Node))
				return i;
		return -1;
	}

	public int Append(Linkable<T> New) => Append((T) New);
	public void Insert(Linkable<T> New, int Index) => Insert((T) New, Index);
	public void Remove(Linkable<T> Node) => Remove((T) Node);
	public void Replace(Linkable<T> Old, Linkable<T> New) => Replace((T) Old, (T) New);
	public void Swap(Linkable<T> Old, Linkable<T> New) => Swap((T) Old, (T) New);
	public void IndexOf(Linkable<T> Node) => IndexOf((T) Node);

	public int Add(T New) => Append(New);
	public int Add(Linkable<T> New) => Append((T) New);
	public void AddAll(IEnumerable<T> All) {
		foreach (T New in All)
			Append(New);
	}
	public void AddAll(IEnumerable<Linkable<T>> All) {
		foreach (Linkable<T> New in All)
			Append(New);
	}

	public T? GetPrevious(T Node) {
		int I = AssertForeignNode(Node).LinkIndex;
		return I == 0 ? null : Backend[I - 1];
	}
	public T? GetNext(T Node) {
		int I = AssertForeignNode(Node).LinkIndex;
		return I == Backend.Count - 1 ? null : Backend[I + 1];
	}

	public T? GetPrevious(Linkable<T> Node) => GetPrevious((T) Node);
	public T? GetNext(Linkable<T> Node) => GetNext((T) Node);

	public T this[int Index] => Backend[Index];
	public int Count => Backend.Count;
	public IEnumerator<T> GetEnumerator() => Backend.GetEnumerator();
	IEnumerator IEnumerable.GetEnumerator() => Backend.GetEnumerator();
}