using System;
using System.Runtime.Serialization;

namespace Alluseri.Luna.Exceptions;

[Serializable]
public class ClassFileException : Exception {
	public ClassFileException() { }
	public ClassFileException(string Message) : base(Message) { }
	public ClassFileException(string Message, Exception Inner) : base(Message, Inner) { }
}