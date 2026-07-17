using Alluseri.Luna.Analysis;
using System;

namespace Alluseri.Luna;

[Serializable]
public class StackAnalysisException : Exception {
	public StackAnalyzer.Node? Node;

	public StackAnalysisException() { }
	public StackAnalysisException(string Message) : base(Message) { }
	public StackAnalysisException(string Message, Exception Inner) : base(Message, Inner) { }
}