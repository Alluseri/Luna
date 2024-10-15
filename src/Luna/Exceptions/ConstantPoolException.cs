using System;
using System.Runtime.Serialization;

namespace Alluseri.Luna.Exceptions;

[Serializable]
public class ConstantPoolException : Exception {
	public ConstantPoolException() { }
	public ConstantPoolException(string Message) : base(Message) { }
	public ConstantPoolException(string Message, Exception Inner) : base(Message, Inner) { }
}