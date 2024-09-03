using System.Collections.Generic;

namespace Alluseri.Luna.Utils;

internal static class CommonExtensions {
	public static V GetOrSet<K, V>(this IDictionary<K, V> Self, K Key) where V : new() {
		if (!Self.TryGetValue(Key, out V? Value))
			Self[Key] = Value = new V();

		return Value;
	}
}