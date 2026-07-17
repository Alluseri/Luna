using Alluseri.Luna.Bytecode;

namespace Alluseri.Luna.Analysis;

// DESIGN: This handles both Stack and Constants (and honestly the latter section is so small idk why I'm even making it separate there)
public partial class StackEmulator {
	private static void RegisterStack() {
		Register<InsnDup>((_, Frame) => {
			if (Frame.Peek() is not { Category2: false } DupType)
				throw new StackAnalysisException("Cannot dup a wide value.");
			Frame.Push(DupType);
		});
		Register<InsnPush>((Push, Frame) => Frame.Push(AnalysisType.Of(Push.Constant)));
		Register<InsnPushNull>((_, Frame) => Frame.Push(new AnalysisType.Null()));
	}
}