namespace Alluseri.Luna.Internals;

public class DoubleVerificationType : VerificationType {
	public override bool Category2 => true;

	public DoubleVerificationType() : base(VerificationTag.Double) { }
}