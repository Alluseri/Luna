using Alluseri.Luna.Internals;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace Alluseri.Luna;

public class LunaJar {
	// TODO: Make this use the abstract class build
	public Dictionary<string, InternalClass> Classes = new();
	public string? Manifest;

	public LunaJar(string Path) : this(File.Open(Path, FileMode.Open, FileAccess.Read)) { }

	public LunaJar(FileStream Stream) { // TODO: Handle joined jars
		using ZipArchive Backend = new(Stream);

		foreach (ZipArchiveEntry Entry in Backend.Entries) {
			if (Entry.Length <= 0)
				continue;
			if (Entry.FullName == "META-INF/MANIFEST.MF")
				using (StreamReader SR = new(Entry.Open()))
					Manifest = SR.ReadToEnd();
			if (Entry.FullName.EndsWith(".class") || Entry.FullName.EndsWith(".class/")) {
				using (Stream S = Entry.Open())
					Classes[Entry.FullName] = new InternalClass(S);
			}
		}
	}
}