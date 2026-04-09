using System.Collections.Generic;

namespace Alluseri.Luna.Analysis;

public class AppContext {
	private Dictionary<string, IInheritanceUnit> InheritanceData = new();

	// Step 1: Simply load all class files into FlatInheritanceData, this will provide us with initial data

	// Step 2: Analyze the code
	// Analyze ATHROW to detect throwables - we don't handle noverify (and when we do, we don't generate SMT)
	// Analyze arguments to calls, local and field stores to determine when O is passed into S

	public void AnalyzeInheritanceFirstPass(JarFile Jf) {
		foreach (LunaClass Lc in Jf.Classes.Values) {
			InheritanceData[Lc.Name] = Lc;
		}
	}


}