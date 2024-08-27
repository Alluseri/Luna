using System;
using System.Runtime.Serialization;

namespace Alluseri.Luna.Exceptions;

[Serializable]
public class ParentAssignedException : Exception {
	public ParentAssignedException() { }
	public ParentAssignedException(string Message) : base(Message) { }
	public ParentAssignedException(string Message, Exception Inner) : base(Message, Inner) { }
}