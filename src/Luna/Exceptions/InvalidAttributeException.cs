using System;
using System.Runtime.Serialization;

namespace Alluseri.Luna.Exceptions;

[Serializable]
public class InvalidAttributeException : Exception {
	public InvalidAttributeException() { }
	public InvalidAttributeException(string Message) : base(Message) { }
	public InvalidAttributeException(string Message, Exception Inner) : base(Message, Inner) { }
}