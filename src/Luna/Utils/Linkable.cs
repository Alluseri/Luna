using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Alluseri.Luna.Exceptions;

namespace Alluseri.Luna.Utils;

public class Linkable<T> where T : Linkable<T> {
	public LinkableList<T>? Root { get; internal set; }
	public int LinkIndex { get; internal set; }

	public Linkable() {
		Root = null;
		LinkIndex = -1;
	}
	public Linkable(LinkableList<T>? Root) {
		LinkIndex = Root?.Append(this) ?? -1;
	}

	public void Append(LinkableList<T> NewRoot) {
		Root?.Remove(this);
		Root = null;
		LinkIndex = (NewRoot ?? throw new ArgumentNullException(nameof(NewRoot), "To assign to Linkable to null, please use Reassign.")).Append(this);
	}
	public void Assign(LinkableList<T>? NewRoot) {
		Root?.Remove(this);
		Root = NewRoot;
		LinkIndex = -1;
	}
	public void Remove() => Assign(null);
	public void InsertPrevious(T NewPrevious) => throw new NotImplementedException("Reserved for future usage and realization due to operation ambiguity.");
	public void InsertNext(T NewNext) => throw new NotImplementedException("Reserved for future usage and realization due to operation ambiguity.");

	public T? Previous {
		get => Root?.GetPrevious(this);
		set {
			if (Root == null)
				throw new InvalidOperationException("Cannot set the previous node of an unrooted Linkable.");
			if (value == null)
				throw new ArgumentNullException(nameof(value), "Cannot set previous node to null. As a potential alternative, try $.Previous.Remove() instead.");
			if (Previous == null) {
				Root.Insert(value, 0);
			} else {
				Root.Replace(Previous, value);
			}
		}
	}

	public T? Next {
		get => Root?.GetNext(this);
		set {
			if (Root == null)
				throw new InvalidOperationException("Cannot set the previous node of an unrooted Linkable.");
			if (value == null)
				throw new ArgumentNullException(nameof(value), "Cannot set next node to null. As a potential alternative, try $.Next.Remove() instead.");
			if (Next == null)
				Root.Append(value);
			else
				Root.Replace(Next, value);
		}
	}
}
