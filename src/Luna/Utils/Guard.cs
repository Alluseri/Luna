using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Alluseri.Luna;

// Thanks .NET engineers for being annoying as shit (this could've been avoided)
internal class Guard {
	[StackTraceHidden]
	[MethodImpl(MethodImplOptions.NoInlining)]
	public static void ThrowIfGreaterThan<T>(T Value, T Other, string Message, [CallerArgumentExpression(nameof(Value))] string? ParamName = null) where T : struct, Enum {
		if (Comparer<T>.Default.Compare(Value, Other) > 0) {
			throw new ArgumentOutOfRangeException(ParamName, Value, Message);
		}
	}
	[StackTraceHidden]
	[MethodImpl(MethodImplOptions.NoInlining)]
	public static void ThrowIfOutOfBounds<T>(T Value, T LowerBound, T UpperBound, string Message, [CallerArgumentExpression(nameof(Value))] string? ParamName = null) where T : struct, Enum {
		if (Comparer<T>.Default.Compare(Value, LowerBound) < 0 || Comparer<T>.Default.Compare(Value, UpperBound) > 0) {
			throw new ArgumentOutOfRangeException(ParamName, Value, Message);
		}
	}
}