using System;

namespace Alluseri.Luna.Utils;

public static class Assert {
	public static int RangeIE(int Value, int MinIncl, int MaxExcl) {
		if (Value < MinIncl || Value >= MaxExcl)
			throw new ArgumentOutOfRangeException($"The given argument is out of range: {Value} is not in the range [{MinIncl}, {MaxExcl})");
		return Value;
	}
	public static int RangeII(int Value, int MinIncl, int MaxIncl) {
		if (Value < MinIncl || Value > MaxIncl)
			throw new ArgumentOutOfRangeException($"The given argument is out of range: {Value} is not in the range [{MinIncl}, {MaxIncl}]");
		return Value;
	}
	public static int RangeIE(string ArgumentName, int Value, int MinIncl, int MaxExcl) {
		if (Value < MinIncl || Value >= MaxExcl)
			throw new ArgumentOutOfRangeException($"Argument {ArgumentName} is out of range: {Value} is not in the range [{MinIncl}, {MaxExcl})");
		return Value;
	}
	public static int RangeII(string ArgumentName, int Value, int MinIncl, int MaxIncl) {
		if (Value < MinIncl || Value > MaxIncl)
			throw new ArgumentOutOfRangeException($"Argument {ArgumentName} is out of range: {Value} is not in the range [{MinIncl}, {MaxIncl}]");
		return Value;
	}
}