namespace Alluseri.Luna.Bytecode;

public enum BranchCondition : uint {
	Equal, NotEqual, LessThan, GreaterEqual, GreatherThan, LessEqual
}

public enum ReferenceBranchCondition : uint {
	Equal, NotEqual
}

internal static class BranchConditionExtensions {
	public static string GetInstructionSign(this BranchCondition Op) => Op switch {
		BranchCondition.Equal => "==",
		BranchCondition.NotEqual => "!=",
		BranchCondition.LessThan => "<",
		BranchCondition.GreaterEqual => ">=",
		BranchCondition.GreatherThan => ">",
		BranchCondition.LessEqual => "<=",
		_ => "?"
	};
	public static string GetInstructionSign(this ReferenceBranchCondition Op) => Op switch {
		ReferenceBranchCondition.Equal => "==",
		ReferenceBranchCondition.NotEqual => "!=",
		_ => "?"
	};
}