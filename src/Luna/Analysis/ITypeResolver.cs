namespace Alluseri.Luna.Analysis;

// DESIGN: "resolver" doesn't sound right here
public interface ITypeResolver {
	bool AreCompatible(AnalysisType One, AnalysisType Other);
	AnalysisType MergeLocal(AnalysisType One, AnalysisType Other);
	AnalysisType MergeStack(AnalysisType One, AnalysisType Other);
}