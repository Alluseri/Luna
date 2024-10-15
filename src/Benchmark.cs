using Alluseri.Luna;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Alluseri.Luna;

public class Benchmark {
	private static Random random = new();

	public static string RandomString(int length) {
		const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
		return new string(Enumerable.Repeat(chars, length)
			.Select(s => s[random.Next(s.Length)]).ToArray());
	}

	string Descriptor16;
	string Descriptor64;
	string Descriptor128;
	string ArrayDescriptor16;
	string ArrayDescriptor64;
	string ArrayDescriptor128;
	string ComplexDescriptor16;
	string ComplexDescriptor64;
	string ComplexDescriptor128;

	public Benchmark() {
		Descriptor16 = $"L{RandomString(16)};";
		Descriptor64 = $"L{RandomString(64)};";
		Descriptor128 = $"L{RandomString(128)};";
		ArrayDescriptor16 = $"[[[[L{RandomString(16)};";
		ArrayDescriptor64 = $"[[[[[[[[L{RandomString(64)};";
		ArrayDescriptor128 = $"[[[[[[[[[[[[[[[[L{RandomString(128)};";
		ComplexDescriptor16 = "([I[Lluna;" + ArrayDescriptor16 + Descriptor16 + "ILjava/lang/Object;Z)";
		ComplexDescriptor64 = "([I[Lluna;" + ArrayDescriptor64 + Descriptor64 + "ILjava/lang/Object;Z)";
		ComplexDescriptor128 = "([I[Lluna;" + ArrayDescriptor128 + Descriptor128 + "ILjava/lang/Object;Z)";
	}

	/*[Benchmark] public TypeDescriptor StrgDescriptor16() => TypeDescriptor.ParseString(Descriptor16);
	[Benchmark] public TypeDescriptor SpanDescriptor16() => TypeDescriptor.ParseSpan(Descriptor16);

	[Benchmark] public TypeDescriptor StrgDescriptor64() => TypeDescriptor.ParseString(Descriptor64);
	[Benchmark] public TypeDescriptor SpanDescriptor64() => TypeDescriptor.ParseSpan(Descriptor64);

	[Benchmark] public TypeDescriptor StrgDescriptor128() => TypeDescriptor.ParseString(Descriptor128);
	[Benchmark] public TypeDescriptor SpanDescriptor128() => TypeDescriptor.ParseSpan(Descriptor128);

	[Benchmark] public TypeDescriptor StrgArrayDescriptor16() => TypeDescriptor.ParseString(ArrayDescriptor16);
	[Benchmark] public TypeDescriptor SpanArrayDescriptor16() => TypeDescriptor.ParseSpan(ArrayDescriptor16);

	[Benchmark] public TypeDescriptor StrgArrayDescriptor64() => TypeDescriptor.ParseString(ArrayDescriptor64);
	[Benchmark] public TypeDescriptor SpanArrayDescriptor64() => TypeDescriptor.ParseSpan(ArrayDescriptor64);

	[Benchmark] public TypeDescriptor StrgArrayDescriptor128() => TypeDescriptor.ParseString(ArrayDescriptor128);
	[Benchmark] public TypeDescriptor SpanArrayDescriptor128() => TypeDescriptor.ParseSpan(ArrayDescriptor128);

	[Benchmark]
	public List<TypeDescriptor> StrgComplexDescriptor16() {
		List<TypeDescriptor> Td = new();
		int Offset = 1;
		while (ComplexDescriptor16[Offset] != ')')
			Td.Add(TypeDescriptor.ParseString(ComplexDescriptor16, ref Offset));
		return Td;
	}

	[Benchmark]
	public List<TypeDescriptor> SpanComplexDescriptor16() {
		List<TypeDescriptor> Td = new();
		int Offset = 1;
		ReadOnlySpan<char> R = ComplexDescriptor16;
		while (R[Offset] != ')')
			Td.Add(TypeDescriptor.ParseSpan(R, ref Offset));
		return Td;
	}

	[Benchmark]
	public List<TypeDescriptor> StrgComplexDescriptor64() {
		List<TypeDescriptor> Td = new();
		int Offset = 1;
		while (ComplexDescriptor64[Offset] != ')')
			Td.Add(TypeDescriptor.ParseString(ComplexDescriptor64, ref Offset));
		return Td;
	}

	[Benchmark]
	public List<TypeDescriptor> SpanComplexDescriptor64() {
		List<TypeDescriptor> Td = new();
		int Offset = 1;
		ReadOnlySpan<char> R = ComplexDescriptor64;
		while (R[Offset] != ')')
			Td.Add(TypeDescriptor.ParseSpan(R, ref Offset));
		return Td;
	}

	[Benchmark]
	public List<TypeDescriptor> StrgComplexDescriptor128() {
		List<TypeDescriptor> Td = new();
		int Offset = 1;
		while (ComplexDescriptor128[Offset] != ')')
			Td.Add(TypeDescriptor.ParseString(ComplexDescriptor128, ref Offset));
		return Td;
	}

	[Benchmark]
	public List<TypeDescriptor> SpanComplexDescriptor128() {
		List<TypeDescriptor> Td = new();
		int Offset = 1;
		ReadOnlySpan<char> R = ComplexDescriptor128;
		while (R[Offset] != ')')
			Td.Add(TypeDescriptor.ParseSpan(R, ref Offset));
		return Td;
	}*/
}