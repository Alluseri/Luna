using System;
using System.Linq;

namespace Alluseri.Luna.Analysis;

// DESIGN: Better name pending

public class StackFrameOperator {
	private readonly AppContext Context;
	// Must be updated in a timely manner
	public StackFrame Frame { get; set; }

	public StackFrameOperator(StackFrame Frame, AppContext Context) {
		this.Frame = Frame;
		this.Context = Context;
	}

	// Normal type resolution: as a fallback, types get merged down to whatever the invoke/get/put is expecting (so we can get to the JVM verifier at least)
	// Pedantic type resolution: types get merged down ONLY according to AppContext, if there's not enough info, everything shall explode

	public bool Merge(StackFrame Incoming) {
		bool Changed = false;

		int LocalsLength = Math.Max(Frame.Locals.Length, Incoming.Locals.Length);
		if (Frame.Locals.Length != LocalsLength) {
			Array.Resize(ref Frame.Locals, LocalsLength);
			for (int i = 0; i < Frame.Locals.Length; i++)
				Frame.Locals[i] ??= new AnalysisType.Top();
			Changed = true;
		}

		for (int i = 0; i < LocalsLength; i++) {
			AnalysisType Current = i < Frame.Locals.Length ? Frame.Locals[i] : new AnalysisType.Top();
			AnalysisType Other = i < Incoming.Locals.Length ? Incoming.Locals[i] : new AnalysisType.Top();
			AnalysisType Merged = MergeLocal(Current, Other);
			if (Merged != Frame.Locals[i]) {
				Frame.Locals[i] = Merged;
				Changed = true;
			}
		}

		AnalysisType[] CurrentStack = Frame.Stack.ToArray();
		AnalysisType[] IncomingStack = Incoming.Stack.ToArray();
		if (CurrentStack.Length != IncomingStack.Length)
			throw new StackAnalysisException($"Cannot merge frames with different stack heights: {CurrentStack.Length} != {IncomingStack.Length}.");

		AnalysisType[] MergedStack = new AnalysisType[CurrentStack.Length];
		for (int i = 0; i < CurrentStack.Length; i++) {
			MergedStack[i] = MergeStack(CurrentStack[i], IncomingStack[i]);
			Changed |= MergedStack[i] != CurrentStack[i];
		}

		if (Changed)
			Frame.Stack = new(MergedStack.Reverse());

		return Changed;
	}

	public bool AreCompatible(AnalysisType One, AnalysisType Other) {
		// TODO: Uninit handling I guess
		if (One == Other)
			return true;
		if (One is AnalysisType.Object && Other is AnalysisType.Object) {
			// TODO: Figure out class inheritance here (do we even need this here? Is it not safe just knowing they're both refs?)
			return true;
		}
		if (One is AnalysisType.Null && Other.Reference)
			return true;
		if (Other is AnalysisType.Null && One.Reference)
			return true;
		return false;
	}

	private static AnalysisType MergeLocal(AnalysisType Current, AnalysisType Incoming) {
		if (Current == Incoming)
			return Current;
		if (Current is AnalysisType.Top || Incoming is AnalysisType.Top)
			return new AnalysisType.Top();
		if (Current is AnalysisType.Null && Incoming.Reference)
			return Incoming;
		if (Incoming is AnalysisType.Null && Current.Reference)
			return Current;
		if (Current is AnalysisType.Object && Incoming is AnalysisType.Object)
			return new AnalysisType.Object("java/lang/Object");
		return new AnalysisType.Top();
	}

	private static AnalysisType MergeStack(AnalysisType Current, AnalysisType Incoming) {
		if (Current == Incoming)
			return Current;
		if (Current is AnalysisType.Null && Incoming.Reference)
			return Incoming;
		if (Incoming is AnalysisType.Null && Current.Reference)
			return Current;
		if (Current is AnalysisType.Object && Incoming is AnalysisType.Object)
			return new AnalysisType.Object("java/lang/Object");
		throw new StackAnalysisException($"Cannot merge stack values {Current} and {Incoming}.");
	}

	// TODO: These operations seem like they'd benefit from HiddenStackTrace, but I'll wait on that
	// TODO: Pass StackAnalyzer.Node for debugging purposes (log index)
	#region Local ops

	public AnalysisType GetAnyLocal(ushort Slot, AnalysisType Expected) {
		// TODO: We can probably just use GetLocal and do additional checks if Category2
		if (Expected.Category2)
			return GetCategory2Local(Slot, Expected);
		else
			return GetLocal(Slot, Expected);
	}

	public AnalysisType GetLocal(ushort Slot, AnalysisType Expected) {
		if (!Frame.HasLocal(Slot))
			throw new StackAnalysisException($"Cannot read local slot {Slot} with only {Frame.Locals.Length} slot(s) available.");
		if (!AreCompatible(Frame.Locals[Slot], Expected))
			throw new StackAnalysisException($"Cannot read {Expected} from local slot {Slot} because it contains {Frame.Locals[Slot]}.");
		return Frame.Locals[Slot];
	}

	public AnalysisType GetCategory2Local(ushort Slot, AnalysisType Expected) {
		AnalysisType Local = GetLocal(Slot, Expected);
		int NextSlot = Slot + 1;
		if (NextSlot >= Frame.Locals.Length || Frame.Locals[NextSlot] is not AnalysisType.Top)
			throw new StackAnalysisException($"Cannot read category 2 local of type {Expected} from slot {Slot} because slot {NextSlot} is not Top.");
		return Local;
	}

	public T GetLocal<T>(ushort Slot) where T : AnalysisType {
		if (!Frame.HasLocal(Slot))
			throw new StackAnalysisException($"Cannot read local slot {Slot} with only {Frame.Locals.Length} slot(s) available.");
		if (Frame.Locals[Slot] is not T Expected)
			throw new StackAnalysisException($"Cannot read {typeof(T)} from local slot {Slot} because it contains {Frame.Locals[Slot]}.");
		return Expected;
	}

	public T GetCategory2Local<T>(ushort Slot) where T : AnalysisType {
		T Local = GetLocal<T>(Slot);
		int NextSlot = Slot + 1;
		if (NextSlot >= Frame.Locals.Length || Frame.Locals[NextSlot] is not AnalysisType.Top)
			throw new StackAnalysisException($"Cannot read category 2 local of type {typeof(T)} from slot {Slot} because slot {NextSlot} is not Top.");
		return Local;
	}

	public AnalysisType GetReferenceLocal(ushort Slot) {
		if (!Frame.HasLocal(Slot))
			throw new StackAnalysisException($"Cannot read local slot {Slot} with only {Frame.Locals.Length} slot(s) available.");
		AnalysisType Local = Frame.Locals[Slot];
		if (!Local.Reference)
			throw new StackAnalysisException($"Cannot read a reference from local slot {Slot} because it contains {Local}.");
		return Local;
	}
	#endregion

	#region Stack ops
	public void Push(AnalysisType Type) => Frame.Stack.Push(Type);

	public AnalysisType Peek() {
		if (!Frame.Stack.TryPeek(out AnalysisType? Type))
			throw new StackAnalysisException($"Cannot read the operand stack because it is empty.");
		return Type;
	}

	public AnalysisType Pop() {
		if (!Frame.Stack.TryPop(out AnalysisType? Type))
			throw new StackAnalysisException($"Cannot pop the operand stack because it is empty.");
		if (Type is AnalysisType.Top)
			throw new StackAnalysisException($"Cannot pop Top from the operand stack."); // my OCD telling me to sacrifice performance to embed {Type}
		return Type;
	}

	public T Pop<T>() where T : AnalysisType {
		AnalysisType Actual = Pop();
		if (Actual is not T Result)
			throw new StackAnalysisException($"Cannot pop {typeof(T)} because the stack contains {Actual}.");
		return Result;
	}

	public AnalysisType Pop(AnalysisType Expected) {
		AnalysisType Actual = Pop();
		if (!AreCompatible(Actual, Expected))
			throw new StackAnalysisException($"Cannot pop {Expected} because the stack contains {Actual}.");
		return Actual;
	}

	public AnalysisType PopReference() {
		AnalysisType Actual = Pop();
		if (!Actual.Reference)
			throw new StackAnalysisException($"Cannot pop a reference because the stack contains {Actual}.");
		return Actual;
	}

	public void Exchange<From>(AnalysisType To) where From : AnalysisType {
		Pop<From>();
		Push(To);
	}

	public void Exchange(AnalysisType From, AnalysisType To) {
		Pop(From);
		Push(To);
	}
	#endregion
}