using Alluseri.Luna.Internals;
using Alluseri.Luna.Utils;
using System.IO;

namespace Alluseri.Luna.Abstract.Bytecode;

public class InsnReturnInteger() : ZeroOpInstruction(Opcode.IReturn, "return.i") { }