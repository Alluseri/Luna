using Alluseri.Luna.Internals;
using System.Collections.Generic;

namespace Alluseri.Luna.Bytecode;

public class StackFrame { // TODO: Rewrite as a record... blah blah
	public List<VerificationType> Locals;
	public LinkedList<VerificationType> Stack;
}