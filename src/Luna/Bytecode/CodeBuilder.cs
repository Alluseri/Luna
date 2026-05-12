using Alluseri.Luna.Internals;
using Alluseri.Luna.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;

namespace Alluseri.Luna.Bytecode;

public class CodeBuilder { // Perfect use case: One CodeBuilder per Class
	public ConstantPool Pool;
	public ClassAttributeCollection Attributes;

	public CodeBuilder(ConstantPool Pool) {
		this.Pool = Pool;
		this.Attributes = new();
	}
	public CodeBuilder(ConstantPool Pool, ClassAttributeCollection Attributes) {
		this.Pool = Pool;
		this.Attributes = Attributes;
	}

	/*private void ResolveGotoDeep(int Index, ) {
		// Uses recursion
	}*/

	public CodeAttribute Build(IEnumerable<Instruction> InstructionList, CodeBuilderConfig Config) {
		// We will pass on using `uint` for all checks instead of `int`, mostly because of the array size limit.

		int Location = 0;

		foreach (Instruction Insn in InstructionList) { // First pass: update pseudo locations
			if (Insn is PseudoInstruction Pi) {
				Pi.Location = Location; // Update the pseudo's location
			} else {
				Insn.Checkout(this, Location); // Size is updated and safe to access
				Location += Insn.Size;
			}
		}

		if (Location > ushort.MaxValue)
			throw new MalformedBytecodeException($"The bytecode size exceeds {ushort.MaxValue} bytes: it is {Location} bytes long.{(!Config.ComplexGotoResolution ? $" Try enabling {nameof(CodeBuilderConfig.ComplexGotoResolution)} in {nameof(CodeBuilderConfig)}." : "")}");

		// All labels are now updated and when referenced in code they will point to proper indexes in code

		List<ExceptionHandler> Handlers = new();
		List<LineEntry> LineNumbers = new(); // Attribute: LineNumber

		Dictionary<string, TryBlock> TryBlocks = new();

		foreach (Instruction Insn in InstructionList) {
			switch (Insn) {
				case TryBlockStart Tbs: {
					TryBlock Tb = TryBlocks.GetOrNew(Tbs.Name);
					Tb.Start = Tbs.Location;
					Tb.Priority = Tbs.Priority;
				}
				break;
				case TryBlockEnd Tbe: {
					TryBlocks.GetOrNew(Tbe.Name).End = Tbe.Location;
				}
				break;
				case TryBlockCatchHandler Tbch: {
					TryBlock Tb = TryBlocks.GetOrNew(Tbch.Name);
					Tb.Handler = Tbch.Location;
					Tb.CatchType = Tbch.CatchClassName;
				}
				break;
				case LineNumber Ln:
				// TODO: Do we even need this if we have the length check above?
				if (Ln.Location <= ushort.MaxValue) // We can silently ignore this without breaking output code (such a line number will never be reached in a stack trace)
					LineNumbers.Add(new LineEntry((ushort) Ln.Location, Ln.Line));
				break;
			}
		}

		foreach (KeyValuePair<string, TryBlock> TbPair in TryBlocks.OrderBy(Ks => -Ks.Value.Priority)) { // Higher priority is closer to index 0 (because of negation - normally it's the opposite)
			TryBlock Tb = TbPair.Value;
			if (Tb.IsWellFormed()) {
				Handlers.Add(Tb.BuildEH(this));
			} else if (!Config.IgnoreMalformedTryBlocks)
				throw new MalformedBytecodeException($"The TryBlock named \"{TbPair.Key}\" is malformed: {Tb.GetAbsentEntries()} is/are missing.");
		}

		List<AttributeInfo> Attributes = new(2);
		if (LineNumbers.Count > 0)
			Attributes.Add(new LineNumberTableAttribute(LineNumbers));
		// STAK MAP TABEL

		byte[] Bytecode = new byte[Location];

		{
			int Address = 0;
			using (MemoryStream Writer = new(Bytecode)) { // This will crash if we try to expand, which is great for debugging.
				foreach (Instruction Insn in InstructionList) {
					Insn.Write(Writer, this, Address);
					Address += Insn.Size;
					if (Address != Writer.Position)
						Console.WriteLine($"After writing {Insn.GetType().Name} ('{Insn}'), address is {Address} but real position is {Writer.Position}!");
				}
			}
		}

		return new CodeAttribute(8, 8, Bytecode, Handlers.ToArray(), Attributes.ToArray());
	}

	protected record class TryBlock {
		private int? _Start;
		private int? _End;
		private int? _Handler;

		public string? CatchType;
		public int Priority = 0;

		public int Start {
			get => _Start!.Value;
			set {
				if (_Start == null)
					if (_Start > ushort.MaxValue)
						throw new ArgumentOutOfRangeException(nameof(value), $"The starting point of a TryBlock needs to be at an address below {ushort.MaxValue + 1}. It was attempted to be placed at address {value}.");
					else
						_Start = value;
				else
					throw new InvalidOperationException($"This TryBlock already has a starting point set. Please eliminate overlapping TryBlock names at bytecode addresses {_Start} and {value}.");
			}
		}
		public int End {
			get => _End!.Value;
			set {
				if (_End == null)
					if (_End > ushort.MaxValue)
						throw new ArgumentOutOfRangeException(nameof(value), $"The ending point of a TryBlock needs to be at an address below {ushort.MaxValue + 1}. It was attempted to be placed at address {value}.");
					else
						_End = value;
				else
					throw new InvalidOperationException($"This TryBlock already has an ending point set. Please eliminate overlapping TryBlock names at bytecode addresses {_End} and {value}.");
			}
		}
		public int Handler {
			get => _Handler!.Value;
			set {
				if (_Handler == null)
					if (_Handler > ushort.MaxValue)
						throw new ArgumentOutOfRangeException(nameof(value), $"The handler of a TryBlock needs to be at an address below {ushort.MaxValue + 1}. It was attempted to be placed at address {value}.");
					else
						_Handler = value;
				else
					throw new InvalidOperationException($"This TryBlock already has a handler set. Please eliminate overlapping TryBlock names at bytecode addresses {_Handler} and {value}.");
			}
		}

		public bool IsWellFormed() => _Start.HasValue && _End.HasValue && _Handler.HasValue;

		public string GetAbsentEntries() => string.Join(", ", new string?[] {
			!_Start.HasValue ? "Start" : null,
			!_End.HasValue ? "End" : null,
			!_Handler.HasValue ? "Handler" : null
		}.Where(K => K != null));

		public ExceptionHandler BuildEH(CodeBuilder Builder) => new((ushort) _Start!, (ushort) _End!, (ushort) _Handler!, CatchType == null ? (ushort) 0 : Builder.Pool.Checkout(new ConstantClass(Builder.Pool.CheckoutUTF8(CatchType)))); // idiot c# why ushort cast roflolmao
	}
}

public class CodeBuilderConfig {
	public bool ComplexGotoResolution = false; // performance hit: very
											   // public bool RepairLabelIdentities = false; // performance hit: minor
	public bool IgnoreMalformedTryBlocks = false; // performance hit: none
	public ComputationLevel ComputationLevel = ComputationLevel.StackFrames; // performance hit: varies

}

public enum ComputationLevel {
	NoVerify, Maximums, StackFrames
}