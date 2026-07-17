
using Alluseri.Luna.Bytecode;
using Alluseri.Luna.Internals;
using System;
using System.Diagnostics;

namespace Alluseri.Luna.Analysis;

public abstract record AnalysisType {
	public virtual bool Category2 => false;
	public virtual bool Reference => false;

	public sealed record Top : AnalysisType;
	public sealed record Integer : AnalysisType;
	public sealed record Float : AnalysisType;
	public sealed record Long : AnalysisType {
		public override bool Category2 => true;
	}
	public sealed record Double : AnalysisType {
		public override bool Category2 => true;
	}
	public sealed record Null : AnalysisType {
		public override bool Reference => true;
	}
	public sealed record UninitializedThis(string ClassName) : AnalysisType {
		public override bool Reference => true;
	}
	public sealed record Object(string ClassName) : AnalysisType {
		public override bool Reference => true;
	}
	// TODO: WTF AM I SUPPOSED TO DO WITH NODE??? IT'S NOT KNOWN ON INIT
	public sealed record Uninitialized(StackAnalyzer.Node NewNode, string ClassName) : AnalysisType {
		public override bool Reference => true;
	}

	public static AnalysisType Of(VerificationType Vt) => Vt switch {
		TopVerificationType => new Top(),
		IntegerVerificationType => new Integer(),
		FloatVerificationType => new Float(),
		DoubleVerificationType => new Double(),
		LongVerificationType => new Long(),
		NullVerificationType => new Null(),
		// UninitializedThisVerificationType => new UninitializedThis(),
		_ => throw new InvalidOperationException($"Not supposed to convert {Vt} directly to AnalysisType.") // TODO: This is technically misleading for an outsider, currently it's this way because of how StackAnalyzer works, the rest of these just aren't necessary for parsing
	};

	public static AnalysisType Of(StackConstant Constant) => Constant switch {
		StackConstantInteger => new Integer(),
		StackConstantFloat => new Float(),
		StackConstantLong => new Long(),
		StackConstantDouble => new Double(),
		StackConstantString => new Object("java/lang/String"),
		StackConstantClass => new Object("java/lang/Class"),
		StackConstantMethodType => new Object("java/lang/invoke/MethodType"),
		StackConstantMethodHandle => new Object("java/lang/invoke/MethodHandle"),
		_ => throw new NotImplementedException($"Cannot infer stack type for {Constant}.")
	};

	public static AnalysisType Of(TypeDescriptor Descriptor) => Descriptor switch {
		PrimitiveTypeDescriptor Primitive => AnalysisType.Of(Primitive.Type.ToVerificationType()),
		ObjectTypeDescriptor Object => new Object(Object.SymbolicTerm),
		ArrayTypeDescriptor Array => new Object(Array.SymbolicTerm),
		_ => throw new UnreachableException($"{Descriptor}")
	};

	public static AnalysisType Of(ValueKind Kind) => Kind switch {
		ValueKind.Integer => new Integer(),
		ValueKind.Long => new Long(),
		ValueKind.Float => new Float(),
		ValueKind.Double => new Double(),
		ValueKind.Reference => throw new InvalidOperationException("Can't infer type of ValueKind.Reference, use StackFrame.PopReference if this is your intention."),
		_ => throw new UnreachableException($"{Kind}")
	};

	public static AnalysisType Of(ArithmeticOperand Operand) => Operand switch {
		ArithmeticOperand.Integer => new Integer(),
		ArithmeticOperand.Long => new Long(),
		ArithmeticOperand.Float => new Float(),
		ArithmeticOperand.Double => new Double(),
		_ => throw new UnreachableException($"{Operand}")
	};

	public static AnalysisType Of(PrimitiveCastResult Result) => Result switch {
		PrimitiveCastResult.Integer or PrimitiveCastResult.Byte or PrimitiveCastResult.Char or PrimitiveCastResult.Short => new Integer(),
		PrimitiveCastResult.Long => new Long(),
		PrimitiveCastResult.Float => new Float(),
		PrimitiveCastResult.Double => new Double(),
		_ => throw new UnreachableException($"{Result}")
	};
}