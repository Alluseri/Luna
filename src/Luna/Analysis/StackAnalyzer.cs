using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Alluseri.Luna.Bytecode;
using Alluseri.Luna.Internals;
using Alluseri.Luna.Utils;

namespace Alluseri.Luna.Analysis;

public class StackAnalyzer
{
	protected readonly AppContext Context;

	public StackAnalyzer(AppContext Context)
	{
		this.Context = Context;
	}

	class Analysis
	{
		// Rare Java superiority over C# moment
		StackAnalyzer Analyzer;

		// TODO: Some kind of record instead of all these args? (can't use Method because this is intermediate)
		public required string? InstanceClass { get; init; }
		public required MethodDescriptor Descriptor { get; init; }
		public required IList<Instruction> Code { get; init; }

		public Analysis(StackAnalyzer Analyzer)
		{
			this.Analyzer = Analyzer;
		}

		List<Node> Nodes = new();
		Dictionary<string, TryBlock> AllTryBlocks = new();
		Dictionary<PseudoInstruction, Node> PseudoToNodeMap = new();
		List<PseudoInstruction> Pseudos = new();

		public void Pass()
		{ // TODO: Most useless method ever award
			FirstPass();
			SecondPass();
			ThirdPass();
			FourthPass();
		}

		// TODO: Ensure code isn't fully empty
		// TODO: LineNumbers should be fully ignored because they don't interact with the control flow in any way!!!

		// First pass: Collect context
		void FirstPass()
		{
			Dictionary<string, TryBlock> ActiveHandlers = new();

			foreach (Instruction Insn in Code)
			{
				if (Insn is PseudoInstruction Pi)
				{
					Pseudos.Add(Pi);
					if (Pi is TryBlockStart Start)
					{
						ActiveHandlers[Start.Name] = AllTryBlocks[Start.Name] = new()
						{
							Name = Start.Name
						};
					}
					else if (Pi is TryBlockEnd End)
					{
						if (!ActiveHandlers.Remove(End.Name))
						{
							throw new StackAnalysisException($"Tried closing the try block '{End.Name}' that was never opened.");
						}
					}
				}
				else
				{
					// TODO: The world if I didn't have to allocate a local for this stupid call
					Node Node = new()
					{
						Pseudos = Pseudos.ToArray(),
						Instruction = Insn,
						ActiveHandlers = ActiveHandlers.Values.ToArray()
					};
					Nodes.Add(Node);
					Pseudos.ForEach(Pi => PseudoToNodeMap[Pi] = Node);
					Pseudos.Clear();
				}
			}

			// TODO: Cleanup first pass (remaining active handlers, etc.)
		}

		// Second pass: Make sense of all TryBlocks + assign futures
		void SecondPass()
		{
			Node? Tail = null;
			foreach (Node Node in Nodes)
			{
				Tail?.Next = Node;

				foreach (PseudoInstruction Pi in Node.Pseudos)
				{
					if (Pi is TryBlockCatchHandler Handler)
					{
						AllTryBlocks[Handler.Name].Update(Node, Handler.CatchClassName);
					}
				}

				Tail = Node;
			}
		}

		// Third pass: Traverse all nodes and identify all successors
		void ThirdPass()
		{
			foreach (Node Node in Nodes)
			{
				switch (Node.Instruction)
				{
					case AbstractSingleBranchInstruction Branch:
						Node.Successors.Add(PseudoToNodeMap[Branch.Target]);

						if (Branch.Conditional)
						{
							Node.Successors.Add(Node.Next ?? throw new StackAnalysisException("Instruction falls off the end of the method."));
						}
						break;

					case InsnLookupSwitch Switch:
						Node.Successors.Add(PseudoToNodeMap[Switch.DefaultCase]);

						foreach (Label Target in Switch.Cases.Values)
						{
							Node.Successors.Add(PseudoToNodeMap[Target]);
						}
						break;

					case InsnTableSwitch Switch:
						Node.Successors.Add(PseudoToNodeMap[Switch.DefaultCase]);

						foreach (Label Target in Switch.Cases)
						{
							Node.Successors.Add(PseudoToNodeMap[Target]);
						}
						break;

					case InsnReturn:
					case InsnThrow:
						break;

					default:
						Node.Successors.Add(Node.Next ?? throw new StackAnalysisException("Instruction falls off the end of the method."));
						break;
				}

				// foreach (TryBlock Handler in Node.ActiveHandlers)
				//	Node.ExceptionSuccessors.Add((Handler.Node, Handler.CatchClassName ?? "java/lang/Throwable"));
			}
		}

		// Fourth pass: Actual evaluation
		void FourthPass()
		{
			StackFrame HeadFrame = new()
			{
				Locals = BuildHeadLocals(InstanceClass, Descriptor).ToArray(),
				Stack = []
			};
			Nodes[0].MergeFrame(new StackFrameOperator(HeadFrame, Analyzer.Context));
			Nodes[0].NeedsStackFrame = true;

			// Note: will have to use ReferenceEquals for uninits (holy shit this is so dumb and convoluted)
			Queue<Node> Pass = new();
			Pass.Enqueue(Nodes[0]);

			while (Pass.Count > 0)
			{
				Node Current = Pass.Dequeue();

				if (Current.ActiveHandlers.Length > 0)
				{
					foreach (TryBlock Eh in Current.ActiveHandlers)
					{
						Propagate(Pass, Eh.Node, Current.Frame!.CopyWithStack([
							new AnalysisType.Object(Eh.CatchClassName ?? "java/lang/Throwable")
						]));
					}
				}

				// I guess we can't really rely on readonliness, because annoyingly enough
				// The entire frame is mutable, and making it fully immutable is aids for... mutating it
				StackFrame After = Current.Frame!.Copy();
				StackFrameOperator Op = new(After, Analyzer.Context);

				StackEmulator.Emulate(Current.Instruction, Op); // Mutates the frame

				foreach (Node Successor in Current.Successors)
				{
					Propagate(Pass, Successor, After.Copy());
				}
			}
		}

		public void Debug()
		{
			// TODO: These could be Node.ToStrings?
			// Debug
			Console.WriteLine("> Complete Log");
			foreach (Node Node in Nodes)
			{
				if (Node.Frame != null)
				{
					Console.WriteLine($"{Node.Instruction}: [{string.Join(", ", Node.Frame.Stack)}] | [{string.Join(", ", Node.Frame.Locals.Select(T => T.GetType().Name))}]");
				}
				else
				{
					Console.WriteLine($"{Node.Instruction}: Dead code");
				}
			}

			Console.WriteLine();
			Console.WriteLine("> Where It Matters");
			foreach (Node Node in Nodes)
			{
				if (Node.NeedsStackFrame)
				{
					Console.WriteLine($"{Node.Instruction}: [{string.Join(", ", Node.Frame!.Stack)}] | [{string.Join(", ", Node.Frame.Locals.Select(T => T.GetType().Name))}]");
				}
				else if (Node.Frame == null)
				{
					Console.WriteLine($"{Node.Instruction}: Dead code");
				}
			}
		}

		// TODO: Can be completely inlined btw
		void Propagate(Queue<Node> Pass, Node Target, StackFrame Incoming)
		{
			if (Target.MergeFrame(new StackFrameOperator(Incoming, Analyzer.Context)))
			{
				Pass.Enqueue(Target);
			}
		}

		// Fucking stupid ass method
		List<AnalysisType> BuildHeadLocals(string? InstanceClass, MethodDescriptor Descriptor)
		{
			List<AnalysisType> Locals = new();
			if (InstanceClass != null)
			{
				Locals.Add(new AnalysisType.Object(InstanceClass));
			}
			foreach (TypeDescriptor Td in Descriptor.Arguments.Descriptors)
			{
				AnalysisType Type = AnalysisType.Of(Td);
				Locals.Add(Type);
				if (Type.Category2)
					Locals.Add(new AnalysisType.Top());
			}
			return Locals;
		}
	}

	public sealed class TryBlock
	{
		public string Name;
		public string? CatchClassName;
		public Node Node;

		public void Update(Node Node, string? CatchClassName)
		{
			this.Node = Node;
			this.CatchClassName = CatchClassName;
		}
	}

	public class Node
	{
		public PseudoInstruction[] Pseudos;
		public Instruction Instruction;
		public TryBlock[] ActiveHandlers;

		// public int Index;

		public Node? Next;
		public List<Node> Successors = new();
		public bool NeedsStackFrame = false;
		// public List<(Node Node, string? ExceptionType)> ExceptionSuccessors;

		public StackFrame? Frame;

		// TODO: This sole method makes me wants to get rid of the entirety of StackFrameOperator. Seriously, this is ugly beyond fucking belief.
		// public bool MergeFrame(StackFrame Frame) {
		public bool MergeFrame(StackFrameOperator Op)
		{
			if (this.Frame == null)
			{
				this.Frame = Op.Frame.Copy();
				return true;
			}
			else
			{
				NeedsStackFrame = true;
				bool Ret = Op.Merge(this.Frame);
				this.Frame = Op.Frame;
				return Ret;
			}
		}
	}
}
