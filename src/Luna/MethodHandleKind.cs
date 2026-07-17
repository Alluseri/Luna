namespace Alluseri.Luna;

public enum MethodHandleKind : byte {
	GetField = 1, GetStatic, PutField, PutStatic, InvokeVirtual, InvokeStatic, InvokeSpecial, NewInvokeSpecial, InvokeInterface
}