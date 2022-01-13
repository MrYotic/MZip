using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace MZip
{
    internal class MZip
    {
        internal List<Entry> entries = new List<Entry>();
        internal class Entry
        {
            internal Entry(string name)
            {
                Open = new MemoryStream();
                Name = name.Split('\\').Last();
                FullName = name;
            }
            internal Entry(string name, Stream stream)
            {
                Open = new MemoryStream();
                new MZip().Copy(stream, Open);
                Name = name.Split('\\').Last();
                FullName = name;
            }
            internal Entry(string name, string path)
            {
                Open = new MemoryStream();
                using (var sf = new FileStream(path, FileMode.Open, FileAccess.Read))
                    new MZip().Copy(sf, Open);
                Name = name.Split('\\').Last();
                FullName = name;
            }
            internal Stream Open;
            internal string Name;
            internal string FullName;
        }
        internal void AddEntry(string name, string path)
        {
            entries.Add(new Entry(name, path));
        }
        internal void AddEntry(string name)
        {
            entries.Add(new Entry(name));
        }
        internal void AddEntry(string name, Stream stream)
        {
            entries.Add(new Entry(name, stream));
        }
        internal List<Entry> Sum(List<Entry> var1, List<Entry> var2)
        {
            if(var1.Count() <= var2.Count()) var2.AddRange(var1);
            else var1.AddRange(var2);
        }
        internal void AddRangeEntry(List<Entry> entries)
        {
           this.entries.AddRange(entries);
        }
        internal void Build(string path)
        {
            using (var memoryStream = new MemoryStream())
            {
                using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                    init(archive);
                using (var fileStream = new FileStream(path, FileMode.Create))
                    Copy(memoryStream, fileStream);
            }
        }
        internal void Build(in Stream zip)
        {
            using (var memoryStream = new MemoryStream())
            {
                using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                    init(archive);
                Copy(memoryStream, zip);
            }
        }
        private void init(in ZipArchive zip)
        {
            for(int i = 0; i < entries.Count; i++)
            {
                var entry = zip.CreateEntry(entries[i].FullName);
                using (var dentryStream = entries[i].Open)
                using (var stream = entry.Open())
                    Copy(dentryStream, stream);                              
            }
        }
        private void Copy(in Stream origin, in Stream ripped)
        {
            origin.Seek(0, SeekOrigin.Begin);
            origin.CopyTo(ripped);
        }
    }
}
