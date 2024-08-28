# Luna
An experimental JVM class file reverse engineering library for parsing, editing and writing class files as well as bytecode.

## This thing is unfinished, pls dont use
Like actually even if you're thinking about it please don't.

Just lookup all the `// DESIGN:` comments and you'll quickly figure out how much stuff is subject to change sooner or later.

That being said, the `Internals` namespace is fully done and production-ready, so you may use that. I don't guarantee **that** won't change either, though. It's just way less likely.

## TODO
todo readme lol idk

## Unsupported obfuscation techniques
- Merged jar files: Luna doesn't implement a custom .zip reader like [LL-Java-Zip](https://github.com/Col-E/LL-Java-Zip).

## Why you may want to use Luna
It's obfuscation-resilient and it's fast. I'm actually kinda optimizing this thing, I even benchmarked some stuff.

## Why you may not want to use Luna
This is a personal project. The abstractions in this library are unconventional, for example `LDC` is split into `InsnPushDouble`, `InsnPushFloat`, `InsnPushInteger`, `InsnPushLong` and `InsnPushString`. The ToString() representations don't match any existing JVM bytecode schemes, leaning towards MSIL instead: `IRETURN` -> `return.i`, `FSTORE` -> `store.f`, `LDC` -> `push`.