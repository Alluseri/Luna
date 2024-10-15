using System;

namespace Alluseri.Luna;

internal class Debug {
	public static T LogAny<T>(string Log, T Data) {
		Console.WriteLine(Log);
		return Data;
	}

	public static T LogPlus<T>(string Log, T Data) {
		Console.WriteLine(Log);
		Console.WriteLine(Data);
		return Data;
	}
}