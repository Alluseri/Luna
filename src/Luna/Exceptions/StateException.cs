using System;
using System.Runtime.Serialization;

namespace Alluseri.Luna.Exceptions;

[Serializable]
public class StateException : Exception {
	public StateException() { }
	public StateException(string Message) : base(Message) { }
	public StateException(string Message, Exception Inner) : base(Message, Inner) { }
}