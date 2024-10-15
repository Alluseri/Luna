using Alluseri.Luna.Internals;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.IO.Hashing;

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
				using (StreamReader ManifestReader = new(Entry.Open()))
					Manifest = ManifestReader.ReadToEnd();
			else if (Entry.FullName.EndsWith(".class") || Entry.FullName.EndsWith(".class/")) {
				using (Stream EntryDeflate = Entry.Open()) {
					using (MemoryStream CopyStream = new(checked((int) Entry.Length))) { // Can't represent over int. Too bad. You could do it normally. But DeflateStream is broken. ReadExactly is broken.
						EntryDeflate.CopyTo(CopyStream);
						CopyStream.Position = 0;
						Classes[Entry.FullName] = new InternalClass(CopyStream);
					}
				}
			}
		}
	}
}