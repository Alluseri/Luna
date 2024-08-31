using Alluseri.Luna.Internals;
using Alluseri.Luna.Utils;
using System.IO;

namespace Alluseri.Luna.Abstract.Bytecode;

public class InsnReturnRef() : ZeroOpInstruction(Opcode.AReturn, "return.a") { }