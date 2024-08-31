namespace Alluseri.Luna.Abstract.Bytecode;

public class InsnLoadByteFromArray() : ZeroOpInstruction(Opcode.BALoad, "loadfromarray.bz") { }
public class InsnLoadCharFromArray() : ZeroOpInstruction(Opcode.CALoad, "loadfromarray.c") { }
public class InsnLoadDoubleFromArray() : ZeroOpInstruction(Opcode.DALoad, "loadfromarray.d") { }
public class InsnLoadFloatFromArray() : ZeroOpInstruction(Opcode.FALoad, "loadfromarray.f") { }
public class InsnLoadIntegerFromArray() : ZeroOpInstruction(Opcode.IALoad, "loadfromarray.i") { }
public class InsnLoadLongFromArray() : ZeroOpInstruction(Opcode.LALoad, "loadfromarray.l") { }
public class InsnLoadRefFromArray() : ZeroOpInstruction(Opcode.AALoad, "loadfromarray.a") { }
public class InsnLoadShortFromArray() : ZeroOpInstruction(Opcode.SALoad, "loadfromarray.s") { }