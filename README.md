# Luna
An experimental JVM class file reverse engineering library for parsing, editing and writing class files as well as bytecode.

Massive thanks to [@jumanji144](https://github.com/jumanji144) for providing his knowledge about the JVM internals and bearing my rants about the JVM spec in his DMs. It's unlikely this library would be possible without his help.

## This thing is unfinished, pls dont use
Like actually even if you're thinking about it please don't.

Just lookup all the `// DESIGN:` and `// TODO:` comments and you'll quickly figure out how much stuff is subject to change sooner or later.

You have to build this project from source so like just dont pls ok?

Also the API is currently HEAVILY inconsistent because I'm doing major refactors mid writing which is really REALLY stupid and I really do hate myself for this but yeah.

**Despite the level of abstractness, this is not entry-level! E.g. no identifier filtering is done.**

## Important notice
Please don't hesitate to open issues regarding functionality of the library. Instead of wondering to yourself "why tf is this method/field marked as internal when I clearly need it right now?", "why does this method throw NotImplementedException?", "why is this piece of metadata is not preserved?" or "why do I have to do this when I could have an easier way of doing this?", you should ask that question in a GitHub Issue and get it resolved directly.

## TODO
todo readme lol idk

## Unsupported obfuscation techniques
- Merged jar files: Luna doesn't implement a custom .zip reader like [LL-Java-Zip](https://github.com/Col-E/LL-Java-Zip).
- Invalid descriptors: An exception is thrown if a descriptor cannot be parsed or is invalid. Annotations with invalid descriptors may be discarded without notice during the I->A transfer process.

## Why you may want to use Luna
It's obfuscation-resilient and it's fast. I'm actually kinda optimizing this thing, I even benchmarked some stuff.

## Why you may not want to use Luna
This is a personal project. The abstractions in this library are unconventional, for example `LDC` is split into `InsnPushDouble`, `InsnPushFloat`, `InsnPushInteger`, `InsnPushLong` and `InsnPushString`. The ToString() representations don't match any existing JVM bytecode schemes, leaning towards MSIL instead: `IRETURN` -> `return.i`, `FSTORE` -> `store.f`, `LDC` -> `push`.