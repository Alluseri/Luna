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
using System.Threading;
using System.Threading.Tasks;

namespace Alluseri.Luna {
	public static class Program {
		// ConstantValue
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
							Console.WriteLine($"Incorrect byte at position {i:X}!");
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
				foreach (AttributeInfo Ai in Ic.Attributes)
					Console.WriteLine(Ai);
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

		public static void CaseJarCollectUnknownInstructions(JarFile JarFile) {
			Dictionary<string, int> ErrorMessages = new();
			double All = 0;
			double Success = 0;
			List<int> LinesPerSuccessfulMethod = new();
			foreach (KeyValuePair<string, InternalClass> Kvp in JarFile.Classes) {
				InternalClass Ic = Kvp.Value;

				// Console.WriteLine($"Disassembling " + Kvp.Key);

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
					} catch (Exception E) {
						ErrorMessages[E.Message] = ErrorMessages.GetValueOrDefault(E.Message, 0) + 1;
						All++;
						// Console.WriteLine($"A method with malformed bytecode was not disassembled: {Mi.GetName(Ic.ConstantPool)}{Mi.GetDescriptor(Ic.ConstantPool)}.")
						// throw;
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

		public static void CaseSingularCollectUnknownInstructions(string Fpath) {
			Dictionary<string, int> ErrorMessages = new();
			double All = 0;
			double Success = 0;
			List<int> LinesPerSuccessfulMethod = new();
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
				} catch (NotSupportedException E) {
					ErrorMessages[E.Message] = ErrorMessages.GetValueOrDefault(E.Message, 0) + 1;
					All++;
				} catch {
					Console.WriteLine($"A method with malformed bytecode was not disassembled: {Mi.GetName(Ic.ConstantPool)}{Mi.GetDescriptor(Ic.ConstantPool)}.");
					throw;
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

				Sw.Restart();

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
					} catch (NotSupportedException E) {
						ErrorMessages[E.Message] = ErrorMessages.GetValueOrDefault(E.Message, 0) + 1;
						All++;
					} catch {
						Console.WriteLine($"A method with malformed bytecode was not disassembled: {Mi.GetName(Ic.ConstantPool)}{Mi.GetDescriptor(Ic.ConstantPool)}.");
						throw;
					}
				}

				Console.WriteLine($"Disassembled in {Sw.ElapsedMilliseconds}ms.");
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

			// CaseMassCollectUnknownInstructions("test/class/forged/x");

			// CaseSingularIO("test/class/forged/x/Test4.class", false);

			CaseSingularCollectUnknownInstructions("test/class/forged/x/Test.class");

			/*InternalClass KIc = new(File.OpenRead("test/class/forged/x/Test.class!"));
			KIc.Methods = KIc.Methods.Append(new MethodInfo(MethodAccessFlags.ACC_PUBLIC | MethodAccessFlags.ACC_STATIC, KIc.ConstantPool.CheckoutUTF8("bootstrap"), KIc.ConstantPool.CheckoutUTF8("([Ljava/lang/Object;)I"), [new CodeAttribute(2, 2, new byte[] { 3, 172 }, new List<ExceptionHandler>(), new List<AttributeInfo>())])).ToArray();
			using (FileStream O = File.Create("test/class/forged/x/Test.class")) {
				KIc.Write(O);
			}*/

			/*InternalClass KIc = new(File.OpenRead("test/class/forged/x/Test.class"));
			foreach (AttributeInfo Ai in KIc.Attributes) {
				if (Ai is BootstrapMethodsAttribute) {
					Console.WriteLine(Ai);
					return;
				}
			}*/

			return;

			// CaseSingularIO("test/class/obfuscated/g.class", false);

			/*InternalClass Ic = new(File.OpenRead("test/class/obfuscated/g.class"));
			foreach (MethodInfo Mi in Ic.Methods) {
				Console.WriteLine(Mi.GetName(Ic.ConstantPool) + Mi.GetDescriptor(Ic.ConstantPool));
				CodeAttribute? Ca = (CodeAttribute?) Mi.Attributes.FirstOrDefault(K => K is CodeAttribute);
				if (Ca == null)
					Console.WriteLine("\tNo CodeAttribute found.");
				else {
					List<Instruction>? T = new CodeReader(Ic).Read(Ca);
					if (T == null)
						Console.WriteLine("\tFailed to disassemble.");
					else
						foreach (Instruction I in T)
							if (I is InsnInvokeDynamic Indy && Indy.Callee.Name.Contains(';')) {
								Console.WriteLine($"\tYes, indy callee name has a semicolon: {Indy.Callee.Name}");
								foreach (BootstrapArgument Ba in Indy.Bootstrap.Arguments) {
									if (Ba is DynamicBootstrapArgument Dba && Dba.ResolveTarget.Name.Contains(';')) {
										Console.WriteLine($"\tYes, indy condy arg has a semicolon: {Dba.ResolveTarget.Name}");
									}
								}
							}
				}
				Console.WriteLine();
			}
			Console.WriteLine();
			foreach (FieldInfo Fi in Ic.Fields) {
				Console.WriteLine(Fi.GetDescriptor(Ic.ConstantPool) + " " + Fi.GetName(Ic.ConstantPool));
			}*/

			// CaseMassCollectUnknownInstructions("test/class/clean");
			// CaseMassCollectUnknownInstructions("test/class/obfuscated");

			/*string JPath = @"test\jar\obfuscated\obf.jar";
			Console.Write($"Loading '{JPath}' ({new FileInfo(JPath).Length / 1024D / 1024D:N2} MiB): ");
			Stopwatch Sw = Stopwatch.StartNew();
			LunaJar Jar = new(File.OpenRead(JPath));
			Sw.Stop();
			Console.WriteLine(Sw.ElapsedMilliseconds + "ms");
			CaseJarCollectUnknownInstructions(Jar);*/

			/*ConstantPool Pool = new();

			InternalClass Ic = new(
				(0, 53),
				Pool,
				ClassAccessFlags.ACC_PUBLIC,
				Pool.Checkout(new ConstantClass(Pool.Checkout(new ConstantUTF8("yipyap")))),
				Pool.Checkout(new ConstantClass(Pool.Checkout(new ConstantUTF8("java/lang/Object")))),
				Array.Empty<ushort>(),
				Array.Empty<FieldInfo>(),
				new MethodInfo[] {
					new(MethodAccessFlags.ACC_PUBLIC, Pool.CheckoutUTF8("luna love"), Pool.CheckoutUTF8("()V"), new AttributeInfo[] {
						new CodeAttribute(
							0, 0, new byte[8], Array.Empty<ExceptionHandler>(), new AttributeInfo[] {
								new CodeAttribute(
									0, 0, new byte[8], Array.Empty<ExceptionHandler>(), new AttributeInfo[] {
										new CustomMalformedAttribute("Code", 0x7FFFFFFF, new byte[]{00, 00})
									}
								)
							}
						)
					})
				},
				Array.Empty<AttributeInfo>()
			);

			Ic.Checkout();

			using (Stream F = File.Create(@"yipyap.class")) {
				Ic.Write(F);
			}

			CaseSingularCollect("yipyap.class");*/

			ConstantPool Cp = new();

			InternalClass Ic = new(
				13,
				Cp,
				ClassAccessFlags.ACC_PUBLIC,
				Cp.Checkout(new ConstantClass(Cp.CheckoutUTF8("dev/lunahook/lullaby/Coverall"))),
				Cp.Checkout(new ConstantClass(Cp.CheckoutUTF8("java/lang/Object"))),
				Array.Empty<ushort>(),
				Array.Empty<FieldInfo>(),
				Array.Empty<MethodInfo>(),
				Array.Empty<AttributeInfo>()
			);

			Label EscapeLab = new("MyEscapeLabel");

			/*
			Coverage:

			*/

			List<Instruction> InsnList = new() {
				new TryBlockStart("ex_silly_cast"),
				new TryBlockStart("ex_silly_cast$1"),
				new InsnPush(new StackConstantInteger(-1)),
				new InsnNew("java/lang/Integer"),
				new InsnDup_X1(),
				new InsnSwap(),
				new InsnInvokeSpecial("java/lang/Integer", new MethodDescriptor(PrimitiveType.Void, "<init>", new(PrimitiveType.Integer))),
				new InsnCheckCast("java/lang/Throwable"),
				new TryBlockEnd("ex_silly_cast$1"),
				new TryBlockEnd("ex_silly_cast"),
				new InsnThrow(),
				new TryBlockCatchHandler("ex_silly_cast", "java/lang/Error"),
				new InsnLoadReference(0),
				new InsnPush(new StackConstantInteger(0)),
				new InsnPush(new StackConstantString("рекомендую этот плейлист к прослушиванию -> https://www.youtube.com/playlist?list=PLuPCd5VIscosG72QWT03BBZriNDEbryea <- このプレイリストを聴くことをお勧めします")),
				new InsnStoreInArray(ArrayType.Reference),
				new TryBlockCatchHandler("ex_silly_cast$1"),
				// Verifier trap 1: we have a java/lang/Throwable on the stack here, but it can also be a java/lang/Error
				// The true identity of this exception is java/lang/ClassCastException
				new InsnThrow(), // todo replaca
				new InsnReturn()
			};

			CodeBuilder Cb = new(Ic.ConstantPool);
			CodeAttribute Ca = Cb.Build(InsnList, new() {
				IgnoreMalformedTryBlocks = false
			});

			#region Managed Form Log
			Console.WriteLine("Managed form:");
			foreach (Instruction Insn in InsnList) {
				Console.WriteLine($"\t{Insn}");
			}
			Console.WriteLine();
			#endregion

			#region Bytecode Form Log
			Console.WriteLine("Bytecode form:");
			Console.WriteLine(Convert.ToHexString(Ca.Bytecode));
			Console.WriteLine();
			#endregion

			Ic.Attributes = Cb.Attributes.ToArray();

			#region Disassembled Form Log
			Console.WriteLine("Disassembled form:");
			List<Instruction>? Disasm = new CodeReader(Ic).Read(Ca);
			if (Disasm == null)
				Console.WriteLine("[Disassembly failed]");
			else
				foreach (Instruction Insn in Disasm) {
					Console.WriteLine($"\t{Insn}");
				}
			#endregion

			Ic.Methods = new[] {
				new MethodInfo(MethodAccessFlags.ACC_PUBLIC | MethodAccessFlags.ACC_STATIC, Cp.CheckoutUTF8("main"), Cp.CheckoutUTF8("([Ljava/lang/String;)V"), new AttributeInfo[] {
					Ca
				})
			};

			using (FileStream Fs = File.Create("dev/lunahook/lullaby/Coverall.class")) {
				Ic.Checkout();
				Ic.Write(Fs);
			}
			Console.WriteLine("Dummy CF saved to current directory.");

			Console.WriteLine("Dummy CF demo read:");
			CaseSingularCollect("dev/lunahook/lullaby/Coverall.class");
			// CaseSingularIO("dev/lunahook/lullaby/Coverall.class", false);

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