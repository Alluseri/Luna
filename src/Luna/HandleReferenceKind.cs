namespace Alluseri.Luna;

public enum MethodHandleReferenceKind : byte { // DESIGN: Rename for shortness
	GetField = 1, GetStatic, PutField, PutStatic, InvokeVirtual, InvokeStatic, InvokeSpecial, NewInvokeSpecial, InvokeInterface
}