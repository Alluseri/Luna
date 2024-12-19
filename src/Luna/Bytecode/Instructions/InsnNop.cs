using Alluseri.Luna.Internals;
using Alluseri.Luna.Utils;
using System;
using System.IO;

namespace Alluseri.Luna.Bytecode;

public class InsnNop() : ZeroOpInstruction(Opcode.Nop, "nop") { }