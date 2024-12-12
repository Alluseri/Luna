using Alluseri.Luna.Internals;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Alluseri.Luna.Bytecode;

public class InsnPush : Instruction {
	public StackConstant Constant;

	public InsnPush(StackConstant Constant) {
		this.Constant = Constant;
	}

	internal override void Checkout(CodeBuilder Builder, int Address) => Constant.CheckoutLdc(Builder, out Size);

	internal override void Write(Stream Stream, CodeBuilder Builder, int Address) {
		Constant.WriteLdc(Stream);
	}

	public override string ToString() => Constant.ToLdcString();
}