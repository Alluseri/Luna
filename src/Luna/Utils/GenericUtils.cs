using System.Collections.Generic;
using System.Linq;

namespace Alluseri.Luna.Utils;

public static class GU {
	public static T[] New<T>(out T[] Array, int Length) => Array = new T[Length];
	public static string ToString<T>(T[] Array) => string.Join(", ", Array);
	public static string ToString(object?[] Array) => string.Join(", ", Array);
	public static string ToString<T>(IEnumerable<T> Enumerable) => string.Join(", ", Enumerable);
	public static string ToString(IEnumerable<object?> Enumerable) => string.Join(", ", Enumerable);
	public static int GetSize(ISizeable[] Array) => Array.Sum(Entry => Entry.Size);
	public static int GetSize(IEnumerable<ISizeable> Enumerable) => Enumerable.Sum(Entry => Entry.Size);
	public static List<T> AsList<T>(params T[] Args) => new(Args);
	public static T[] VarArgs<T>(params T[] Args) => Args;
	public static T[] Append<T>(this T[] Self, T[] Array) {
		T[] Output = new T[Self.Length + Array.Length];
		for (int i = 0; i < Self.Length; i++) {
			Output[i] = Self[i];
		}
		for (int i = 0; i < Array.Length; i++) {
			Output[Self.Length + i] = Array[i];
		}
		return Output;
	}
}