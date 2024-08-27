namespace Alluseri.Luna.Internals;

public class LongVerificationType : VerificationType {
	public override bool Category2 => true;

	public LongVerificationType() : base(VerificationTag.Long) { }
}