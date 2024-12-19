using System.Collections.Generic;

namespace Alluseri.Luna.Context;

public class AppContext {
	private Dictionary<string, IInheritanceUnit> FlatInheritanceData = new();

	// Step 1: Simply load all class files into FlatInheritanceData, this will provide us with initial data

	// Step 2: Analyze the code
	// Analyze ATHROW to detect throwables - we don't handle noverify (and when we do, we don't generate SMT)
	// Analyze arguments to calls, local and field stores to determine when O is passed into S

	public void Analyze(JarFile Jf) {
		/*foreach (LunaClass Lc in Jf) {
			FlatInheritanceData[Lc.Name] = Lc; // mewoe wmowo meomweo memow moew mo ewm ewo mewo mewo moew m oewm oew mo ewm eow moew mewo meowe moew meow
		}*/
	}
}