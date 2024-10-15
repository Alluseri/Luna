using Alluseri.Luna.Internals;
using Alluseri.Luna.Utils;
using System.IO;

namespace Alluseri.Luna.Bytecode;

public class InsnReturnRef() : ZeroOpInstruction(Opcode.AReturn, "return.a") { }