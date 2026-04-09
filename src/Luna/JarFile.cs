using Alluseri.Luna.Internals;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace Alluseri.Luna;

public class JarFile {
	public Dictionary<string, LunaClass> Classes = new(); // TODO: What even is the point if LunaClass has a Name field?
	public string? Manifest;

	// public JarFile(string Path) : this(File.Open(Path, FileMode.Open, FileAccess.Read)) { } // DISPOSE WHERE HELLO??

	public JarFile(FileStream Stream) {
		using ZipArchive Backend = new(Stream);

		foreach (ZipArchiveEntry Entry in Backend.Entries) {
			if (Entry.Length <= 0)
				continue;
			if (Entry.FullName == "META-INF/MANIFEST.MF")
				using (StreamReader ManifestReader = new(Entry.Open()))
					Manifest = ManifestReader.ReadToEnd();
			else if (Entry.FullName.EndsWith(".class") || Entry.FullName.EndsWith(".class/")) {
				try {
					using (Stream EntryDeflate = Entry.Open()) {
						using (MemoryStream CopyStream = new(checked((int) Entry.Length))) { // Can't represent over int? Too bad. You could do it normally, but DeflateStream is broken. ReadExactly is broken. Hell, everything is broken.
							EntryDeflate.CopyTo(CopyStream);
							CopyStream.Position = 0;
							Classes[Entry.FullName] = new LunaClass(new InternalClass(CopyStream));
							// Classes[Entry.FullName] = new InternalClass(CopyStream);
						}
					}
				} catch {
					// TODO: Handle failures (like a failed class entries list)
				}
			}
		}
	}

	// IEnumerator IEnumerable.GetEnumerator() => Classes.Values.GetEnumerator();
	// public IEnumerator<LunaClass> GetEnumerator() => Classes.Values.GetEnumerator();
}