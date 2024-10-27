using System;
using System.Runtime.Serialization;

namespace Alluseri.Luna;

[Serializable]
public class EmulationException : Exception {
	public EmulationException() { }
	public EmulationException(string Message) : base(Message) { }
	public EmulationException(string Message, Exception Inner) : base(Message, Inner) { }
}