using System;
using System.Runtime.Serialization;

namespace Alluseri.Luna;

// TODO: I'm inclined to believe that this is useless (core Luna library will not have a full blown emulator)
[Serializable]
public class EmulationException : Exception {
	public EmulationException() { }
	public EmulationException(string Message) : base(Message) { }
	public EmulationException(string Message, Exception Inner) : base(Message, Inner) { }
}