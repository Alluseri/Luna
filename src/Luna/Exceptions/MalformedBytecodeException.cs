using System;

namespace Alluseri.Luna;

[Serializable]
public class MalformedBytecodeException : Exception {
	public MalformedBytecodeException() { }
	public MalformedBytecodeException(string Message) : base(Message) { }
	public MalformedBytecodeException(string Message, Exception Inner) : base(Message, Inner) { }
}