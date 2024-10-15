using Alluseri.Luna;
using Alluseri.Luna.Bytecode;
using Alluseri.Luna.Internals;
using Alluseri.Luna.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Hashing;
using System.Linq;

namespace Alluseri.Luna {
	public static class Program {
		// ConstantValue
		// SourceDebugExtension
		// Synthetic
		// AnnotationDefault

		public static T Cast<T>(this object M) => (T) M;

		public static List<AttributeInfo> CollectMalforms(InternalClass Ic) {
			List<AttributeInfo> Malforms = new();
			foreach (FieldInfo Fi in Ic.Fields) {
				foreach (AttributeInfo Ai in Fi.Attributes) {
					if (Ai is InvalidAttribute || Ai is MalformedAttribute)
						Malforms.Add(Ai);
				}
			}
			foreach (MethodInfo Fi in Ic.Methods) {
				foreach (AttributeInfo Ai in Fi.Attributes) {
					if (Ai is InvalidAttribute || Ai is MalformedAttribute)
						Malforms.Add(Ai);
				}
			}
			foreach (AttributeInfo Ai in Ic.Attributes) {
				if (Ai is InvalidAttribute || Ai is MalformedAttribute) {
					Malforms.Add(Ai);
				} else if (Ai is CodeAttribute Ca) {
					foreach (AttributeInfo Cai in Ca.Attributes) {
						if (Cai is InvalidAttribute || Cai is MalformedAttribute)
							Malforms.Add(Cai);
					}
				}
			}
			return Malforms;
		}

		public static List<UnknownAttribute> CollectUnknowns(InternalClass Ic) {
			List<UnknownAttribute> Unk = new();
			foreach (FieldInfo Fi in Ic.Fields) {
				foreach (AttributeInfo Ai in Fi.Attributes) {
					if (Ai is UnknownAttribute U)
						Unk.Add(U);
				}
			}
			foreach (MethodInfo Fi in Ic.Methods) {
				foreach (AttributeInfo Ai in Fi.Attributes) {
					if (Ai is UnknownAttribute U)
						Unk.Add(U);
				}
			}
			foreach (AttributeInfo Ai in Ic.Attributes) {
				if (Ai is UnknownAttribute U) {
					Unk.Add(U);
				} else if (Ai is CodeAttribute Ca) {
					foreach (AttributeInfo Cai in Ca.Attributes) {
						if (Cai is UnknownAttribute UX)
							Unk.Add(UX);
					}
				}
			}
			return Unk;
		}

		public static List<T> Collect<T>(InternalClass Ic) where T : AttributeInfo {
			List<T> Found = new();
			foreach (FieldInfo Fi in Ic.Fields) {
				foreach (AttributeInfo Ai in Fi.Attributes) {
					if (Ai is T X)
						Found.Add(X);
				}
			}
			foreach (MethodInfo Fi in Ic.Methods) {
				foreach (AttributeInfo Ai in Fi.Attributes) {
					if (Ai is T X)
						Found.Add(X);
				}
			}
			foreach (AttributeInfo Ai in Ic.Attributes) {
				if (Ai is T X) {
					Found.Add(X);
				} else if (Ai is CodeAttribute Ca) {
					foreach (AttributeInfo Cai in Ca.Attributes) {
						if (Cai is T Y)
							Found.Add(Y);
					}
				}
			}
			return Found;
		}

		public static void CaseMassIO(string DirPath) {
			foreach (string Fpath in Directory.EnumerateFiles(DirPath)) {
				using Stream F = File.OpenRead(Fpath);
				Console.Write("Loading " + Path.GetFileName(Fpath) + ": ");
				Stopwatch Sw = Stopwatch.StartNew();
				InternalClass Ic = new(F);
				Sw.Stop();

				Console.WriteLine(Sw.ElapsedMilliseconds + "ms");

				using MemoryStream O = new();

				Console.Write("Writing " + Path.GetFileName(Fpath) + ": ");
				Sw = Stopwatch.StartNew();
				Ic.Write(O);
				Sw.Stop();
				Console.WriteLine(Sw.ElapsedMilliseconds + "ms");

				O.Position = 0;
				F.Position = 0;
				if (O.Length != F.Length) {
					Console.WriteLine($"Length mismatch: original is {F.Length}, written is {O.Length}.");
				} else {
					for (int i = 0; i < F.Length; i++) {
						if (F.ReadByte() != O.ReadByte()) {
							Console.WriteLine($"Incorrect byte at position {i:X}!.");
							break;
						}
					}
				}

				Console.WriteLine();
			}
		}

		public static void CaseSingularIO(string FilePath, bool DumpPool) {
			using Stream F = File.OpenRead(FilePath);
			Console.Write("Loading input file: ");
			Stopwatch Sw = Stopwatch.StartNew();
			InternalClass Ic = new(F);
			Sw.Stop();

			Console.WriteLine(Sw.ElapsedMilliseconds + "ms");
			Console.WriteLine("Source file: " + Ic.Attributes.FirstOrDefault(A => A is SourceFileAttribute)?.Cast<SourceFileAttribute>().GetName(Ic.ConstantPool));
			int? Bootstraps = Ic.Attributes.FirstOrDefault(A => A is BootstrapMethodsAttribute)?.Cast<BootstrapMethodsAttribute>().Content.Count;
			if (Bootstraps.HasValue)
				Console.WriteLine("Bootstrap method count: " + Bootstraps);

			List<AttributeInfo> Malforms = CollectMalforms(Ic);
			if (Malforms.Count > 0) {
				Console.WriteLine($"There are {Malforms.Count} malformed attributes present:");
				foreach (AttributeInfo Ma in Malforms) {
					Console.WriteLine($"{Ma} - {Ma.Name} [{Ma.Size} bytes]");
				}
			}

			List<UnknownAttribute> Unks = Collect<UnknownAttribute>(Ic);
			if (Unks.Count > 0) {
				Console.WriteLine($"There are {Unks.Count} unknown attributes:");
				foreach (UnknownAttribute Ma in Unks) {
					Console.WriteLine($"{Ma} - {Ma.Name} [{Ma.Size} bytes]");
				}
			}

			if (DumpPool) {
				for (ushort i = 1; i <= Ic.ConstantPool.Count; i++)
					Console.WriteLine(i + "(" + i.ToString("X4") + "): " + Ic.ConstantPool[i]);
			}

			Sw = Stopwatch.StartNew();
			using FileStream Fs = File.Create(FilePath + ".luna");
			Ic.Write(Fs);
			Sw.Stop();
			Console.Write($"Wrote file successfully in {Sw.ElapsedMilliseconds}ms.");

		}

		public static void CaseMassCollectSpecific<T>(string DirPath, bool DumpPool = false) where T : AttributeInfo {
			foreach (string Fpath in Directory.EnumerateFiles(DirPath)) {
				using Stream F = File.OpenRead(Fpath);
				Console.Write("Loading " + Path.GetFileName(Fpath) + ": ");
				Stopwatch Sw = Stopwatch.StartNew();
				InternalClass Ic = new(F);
				Sw.Stop();

				Console.WriteLine(Sw.ElapsedMilliseconds + "ms");
				Console.WriteLine("Source file: " + Ic.Attributes.FirstOrDefault(A => A is SourceFileAttribute)?.Cast<SourceFileAttribute>().GetName(Ic.ConstantPool));
				int? Bootstraps = Ic.Attributes.FirstOrDefault(A => A is BootstrapMethodsAttribute)?.Cast<BootstrapMethodsAttribute>().Content.Count;
				if (Bootstraps.HasValue)
					Console.WriteLine("Bootstrap method count: " + Bootstraps);

				List<AttributeInfo> Malforms = CollectMalforms(Ic);
				if (Malforms.Count > 0) {
					Console.WriteLine($"There are {Malforms.Count} malformed attributes present:");
					foreach (AttributeInfo Ma in Malforms) {
						Console.WriteLine($"{Ma} - {Ma.Name} [{Ma.Size} bytes]");
					}
				}

				List<UnknownAttribute> Unks = Collect<UnknownAttribute>(Ic);
				if (Unks.Count > 0) {
					Console.WriteLine($"There are {Unks.Count} unknown attributes:");
					foreach (UnknownAttribute Ma in Unks) {
						Console.WriteLine($"{Ma} - {Ma.Name} [{Ma.Size} bytes]");
					}
				}

				if (DumpPool) {
					for (ushort i = 1; i <= Ic.ConstantPool.Count; i++)
						Console.WriteLine($"{i}({i:X4}): {Ic.ConstantPool[i]}");
				}

				List<T> Annos = Collect<T>(Ic);
				foreach (T Rat in Annos) {
					Console.WriteLine(Rat);
				}

				Console.WriteLine();
			}
		}

		public static void CaseSingularCollect(string FilePath) {
			using Stream F = File.OpenRead(FilePath);
			Stopwatch Sw = Stopwatch.StartNew();
			InternalClass Ic = new(F);
			Sw.Stop();
			Console.WriteLine("Loaded class in " + Sw.ElapsedMilliseconds + "ms");
			Console.WriteLine("Source file: " + Ic.Attributes.FirstOrDefault(A => A is SourceFileAttribute)?.Cast<SourceFileAttribute>().GetName(Ic.ConstantPool));
			Console.WriteLine("Bootstrap method count: " + Ic.Attributes.FirstOrDefault(A => A is BootstrapMethodsAttribute)?.Cast<BootstrapMethodsAttribute>().Content.Count);
			Console.WriteLine("It has " + Ic.Fields.Length + " fields");
			Console.WriteLine("It has " + Ic.Methods.Length + " methods");
			Console.WriteLine("It has " + Ic.Attributes.Length + " attributes:");
			foreach (AttributeInfo Ai in Ic.Attributes) {
				Console.WriteLine($"{Ai.Name} [{Ai.Size} bytes]");
			}

			List<AttributeInfo> Malforms = CollectMalforms(Ic);
			Console.WriteLine($"There are overall {Malforms.Count} malformed attributes:");
			foreach (AttributeInfo Ma in Malforms) {
				Console.WriteLine($"{Ma} - {Ma.Name} [{Ma.Size} bytes]");
			}

			List<UnknownAttribute> Unks = CollectUnknowns(Ic);
			Console.WriteLine($"There are overall {Unks.Count} unknown attributes:");
			foreach (AttributeInfo Ma in Unks) {
				Console.WriteLine($"{Ma} - {Ma.Name} [{Ma.Size} bytes]");
			}
		}

		public static void CaseMassCollectUnknownInstructions(string DirPath) {
			Dictionary<string, int> ErrorMessages = new();
			double All = 0;
			double Success = 0;
			List<int> LinesPerSuccessfulMethod = new();
			foreach (string Fpath in Directory.EnumerateFiles(DirPath)) {
				using Stream F = File.OpenRead(Fpath);
				Console.Write("Loading " + Path.GetFileName(Fpath) + ": ");
				Stopwatch Sw = Stopwatch.StartNew();
				InternalClass Ic = new(F);
				Sw.Stop();

				Console.WriteLine(Sw.ElapsedMilliseconds + "ms");

				foreach (MethodInfo Mi in Ic.Methods) {
					try {
						CodeAttribute? Ca = (CodeAttribute?) Mi.Attributes.FirstOrDefault(K => K is CodeAttribute);
						if (Ca != null) {
							List<Instruction>? Instructions = new CodeReader(Ic).Read(Ca);
							if (Instructions == null) {
								Console.WriteLine($"A method with malformed bytecode was not disassembled: {Mi.GetName(Ic.ConstantPool)}{Mi.GetDescriptor(Ic.ConstantPool)}.");
								continue;
							}
							All++;
							Success++;
							LinesPerSuccessfulMethod.Add(Instructions.Count);
						}
					} catch (InvalidDataException E) {
						ErrorMessages[E.Message] = ErrorMessages.GetValueOrDefault(E.Message, 0) + 1;
						All++;
					} catch (NotImplementedException E) {
						ErrorMessages[E.Message] = ErrorMessages.GetValueOrDefault(E.Message, 0) + 1;
						All++;
					}
				}
			}

			IOrderedEnumerable<KeyValuePair<string, int>> Log = ErrorMessages.OrderByDescending(K => K.Value);
			Console.WriteLine("Exceptions sorted by appearance:");
			foreach (KeyValuePair<string, int> L in Log) {
				Console.WriteLine($"{L.Value} times: {L.Key}");
			}
			Console.WriteLine($"{Success:N0}/{All:N0} ({Success / All * 100:N2}%) methods were disassembled successfully.");
			Console.WriteLine($"Average instructions per successfully disassembled method: {LinesPerSuccessfulMethod.Average():N1}.");
		}

		public static void Main(string[] Args) {
			// BenchmarkRunner.Run<Benchmark>();

			CaseMassCollectUnknownInstructions("test/class/clean");
			CaseMassCollectUnknownInstructions("test/class/obfuscated");

			/*ConstantPool Cp = new();

			InternalClass Ic = new(
				13,
				Cp,
				ClassAccessFlags.ACC_PUBLIC,
				Cp.Checkout(new ConstantClass(Cp.CheckoutUtf8("dev/lunahook/Test"))),
				Cp.Checkout(new ConstantClass(Cp.CheckoutUtf8("java/lang/Object"))),
				Array.Empty<ushort>(),
				Array.Empty<FieldInfo>(),
				Array.Empty<MethodInfo>(),
				Array.Empty<AttributeInfo>()
			);

			Label EscapeLab = new("MyEscapeLabel");
			Label StackOverflowLab = new("RipStack");

			List<Instruction> InsnList = new() {
				new InsnPushInteger(40),
				new InsnDup(),
				new InsnStoreInteger(1),
				new InsnPushInteger(3),
				new InsnMultiply(ArithmeticOperand.Integer),
				new InsnGoto(EscapeLab),
				//StackOverflowLab,
				//new InsnDup(),
				//new InsnPushNull(),
				//new InsnGoto(StackOverflowLab),
				EscapeLab,
				// TODO: PrimitiveType.Descriptor() extension method
				new InsnInvokeStatic("dev/lunahook/Logger", new MethodDescriptor(
					new PrimitiveTypeDescriptor(PrimitiveType.Void),
					"logInteger",
					new CompoundTypeDescriptor(
						new PrimitiveTypeDescriptor(PrimitiveType.Int)
					)
				)),
				new InsnReturn()
			};

			Console.WriteLine("Managed form:");
			foreach (Instruction Insn in InsnList) {
				Console.WriteLine($"\t{Insn}");
			}
			Console.WriteLine();

			Console.WriteLine("Bytecode form:");
			byte[] Bytecode = new CodeBuilder(Ic).Build(InsnList);
			Console.WriteLine(Convert.ToHexString(Bytecode));
			Console.WriteLine();

			CodeAttribute Ca = new(8, 8, Bytecode, Array.Empty<ExceptionHandler>(), new[] {
				new LineNumberTableAttribute(new[] {
					new LineEntry(0, 4),
					new LineEntry(4, 9)
				})
			});

			Console.WriteLine("Disassembled form:");
			List<Instruction>? Disasm = new CodeReader(Ic).Read(Ca);
			if (Disasm == null)
				Console.WriteLine("[Disassembly failed]");
			else
				foreach (Instruction Insn in Disasm) {
					Console.WriteLine($"\t{Insn}");
				}

			Ic.Methods = new[] {
				new MethodInfo(MethodAccessFlags.ACC_PUBLIC, Cp.CheckoutUtf8("runTest"), Cp.CheckoutUtf8("()V"), new[] {
					Ca
				})
			};

			using (FileStream Fs = File.OpenWrite("dummycf.class")) {
				Ic.Checkout();
				Ic.Write(Fs);
			}
			Console.WriteLine("Dummy CF saved to current directory.");

			Console.WriteLine("Dummy CF demo read:");
			CaseSingularCollect("dummycf.class");*/


			/*using Stream F = File.OpenRead(@"test/class/obfuscated/$$E.class");
			Stopwatch Sw = Stopwatch.StartNew();
			InternalClass Ic = new(F);
			Sw.Stop();
			Console.WriteLine("Loaded class in " + Sw.ElapsedMilliseconds + "ms");
			Console.WriteLine("Source file: " + Ic.Attributes.FirstOrDefault(A => A is SourceFileAttribute)?.Cast<SourceFileAttribute>().GetName(Ic.ConstantPool));
			Console.WriteLine("It has " + Ic.Fields.Length + " fields");
			Console.WriteLine("It has " + Ic.Methods.Length + " methods");
			BootstrapMethod[]? Bm = Ic.Attributes.FirstOrDefault(A => A is BootstrapMethodsAttribute)?.Cast<BootstrapMethodsAttribute>().BootstrapMethods;
			if (Bm == null) {
				Console.WriteLine("This class has no bootstrap methods.");
			} else {
				Console.WriteLine($"Bootstrap methods: {Bm.Length}");
				// foreach (BootstrapMethod B in Bm) {
				// 	Console.WriteLine($"\t{B.GetHandle(Ic.ConstantPool)}: [\n\t\t{string.Join(",\n\t\t", B.ArgumentIndexes.Select(I => Ic.ConstantPool[I]))}\n\t] for a total of {B.ArgumentIndexes.Length} arguments.");
				// }
				// Console.WriteLine();
			}

			foreach (MethodInfo Mi in Ic.Methods) {
				Console.WriteLine("");
				try {
					Console.WriteLine($"Method {Mi.GetName(Ic.ConstantPool)}{Mi.GetDescriptor(Ic.ConstantPool)}:");
					CodeAttribute? Ca = (CodeAttribute?) Mi.Attributes.FirstOrDefault(K => K is CodeAttribute);
					if (Ca != null) {

						Console.WriteLine($"(Total: {Instructions.Count})");
					} else if (Mi.AccessFlags.HasFlag(MethodAccessFlags.ACC_ABSTRACT)) {
						Console.WriteLine($"\tMethod is abstract.");
					} else {
						Console.WriteLine($"\tIllegal Code Attribute (or missing).");
					}
				} catch (Exception E) {
					Console.WriteLine(E);
				}
			}*/

			//for (ushort i = 1; i <= Ic.ConstantPool.Count; i++)
			//	Console.WriteLine(i + "(" + i.ToString("X4") + "): " + Ic.ConstantPool[i]);
		}
	}
}