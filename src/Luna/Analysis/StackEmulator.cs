using Alluseri.Luna.Bytecode;
using Alluseri.Luna.Internals;
using Alluseri.Luna.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Alluseri.Luna.Analysis;

public partial class StackEmulator {
	private static readonly Dictionary<Type, Action<Instruction, StackFrameOperator>> MappedEmulations;

	static StackEmulator() {
		MappedEmulations = new();
	}

	private static void Register<T>(Action<T, StackFrameOperator> Action) where T : Instruction {
		MappedEmulations.Add(typeof(T), (Insn, Frame) => Action((T) Insn, Frame));
	}

	// TODO: This is a fucking mess
	public static void Emulate(Instruction Insn, StackFrameOperator Frame) {
		switch (Insn) {
			case InsnLoadLocal Load:
				if (Load.Kind == ValueKind.Reference) {
					Frame.Push(Frame.GetReferenceLocal(Load.Slot));
				} else {
					Frame.Push(Frame.GetAnyLocal(Load.Slot, AnalysisType.Of(Load.Kind)));
				}
				break;
			case InsnPrimitiveCast Cast:
				Frame.Exchange(AnalysisType.Of(Cast.From), AnalysisType.Of(Cast.To));
				break;
			case InsnAdd Add:
				AnalysisType AddType = AnalysisType.Of(Add.Operand);
				Frame.Pop(AddType);
				Frame.Pop(AddType);
				Frame.Push(AddType);
				break;
			case InsnIntegerBranch:
				Frame.Pop<AnalysisType.Integer>();
				Frame.Pop<AnalysisType.Integer>();
				break;
			case InsnZeroBranch:
				Frame.Pop<AnalysisType.Integer>();
				break;
			case InsnReferenceBranch:
				Frame.PopReference();
				Frame.PopReference();
				break;
			case InsnIfNullBranch:
			case InsnIfNotNullBranch:
				Frame.PopReference();
				break;
			case InsnGetField GetField:
				Frame.PopReference();
				Frame.Push(AnalysisType.Of(GetField.Field.FieldType));
				break;
			case InsnNop:
			case InsnGoto:
				break;
			case InsnReturn Return:
				if (Return.Kind != null) {
					if (Return.Kind == ValueKind.Reference) {
						Frame.PopReference();
					} else {
						Frame.Pop(AnalysisType.Of(Return.Kind.Value));
					}
				}
				break;
			default:
				throw new NotImplementedException($"Not yet! {Insn}");
		}
	}
}
