using System;

namespace Alluseri.Luna;

[Serializable]
public class ClassFileException : Exception {
	public ClassFileException() { }
	public ClassFileException(string Message) : base(Message) { }
	public ClassFileException(string Message, Exception Inner) : base(Message, Inner) { }
}