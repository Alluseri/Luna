using Alluseri.Luna.Internals;
using System.Collections.Generic;

namespace Alluseri.Luna.Abstract.Bytecode;

public class StackFrame {
	public List<VerificationType> Locals;
	public LinkedList<VerificationType> Stack;
}