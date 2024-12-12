namespace Alluseri.Luna;

public interface IInheritanceUnit {
	public string Name { get; }
	public string? Super { get; }
	public string[] Interfaces { get; }
}