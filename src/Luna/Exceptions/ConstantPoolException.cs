using System;

namespace Alluseri.Luna;

[Serializable]
public class ConstantPoolException : Exception {
	public ConstantPoolException() { }
	public ConstantPoolException(string Message) : base(Message) { }
	public ConstantPoolException(string Message, Exception Inner) : base(Message, Inner) { }
}