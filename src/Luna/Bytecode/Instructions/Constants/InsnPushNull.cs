using Alluseri.Luna.Internals;
using Alluseri.Luna.Utils;
using System.IO;

namespace Alluseri.Luna.Bytecode;

public class InsnPushNull() : ZeroOpInstruction(Opcode.AConst_Null, "push.a null") { }