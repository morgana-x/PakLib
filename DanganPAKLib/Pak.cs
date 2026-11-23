namespace DanganPAKLib
{
    public class Pak
    {
        public List<PakEntry> FileEntries { get; set; } = new List<PakEntry>();
        private Stream PakStream { get; set; }
        private BinaryReader PakReader { get; set; }

        private void ReadHeader()
        {
            FileEntries.Clear();

            PakReader.BaseStream.Seek(0, SeekOrigin.Begin);

;           int numberOfFiles = PakReader.ReadInt32();

            List<uint> offsets = new List<uint>();

            for (int i = 0; i < numberOfFiles; i++)
            {
                offsets.Add(PakReader.ReadUInt32());
                if (offsets[i] > PakStream.Length || (i > 0 && offsets[i] < offsets[i - 1]))
                {
                    throw new("Invalid PAK file, corrupted offsets");
                    return;
                }
            }

            for (int i = 0; i < numberOfFiles; i++)
            {
                FileEntries.Add(new PakEntry()
                {
                    Index = i,
                    Offset = offsets[i],
                    Size = (i < numberOfFiles - 1) ? (offsets[i + 1] - offsets[i]) : (PakStream.Length - offsets[i])
                });

                if (FileEntries[i].Size > Array.MaxLength)
                {
                    throw new("Invalid PAK file, corrupted offsets");
                    FileEntries.Clear();
                    return;
                }
            }
        }

        public byte[] GetFileData(int i)
        {
            var file = FileEntries[i];
            PakStream.Position = file.Offset;
            
            byte[] data = new byte[file.Size];
            PakStream.Read(data);

            return data;
        }
        
        public void ExtractFile(int i, string path)
        {
            if (path.EndsWith(".pak"))
            {
                try
                {
                    Pak pak = new Pak(GetFileData(i));
                    pak.ExtractAllFiles(path.Substring(0, path.Length - 4));
                    pak.Dispose();
                    return;
                }
                catch (Exception e)
                {
                    Console.WriteLine("Tried extracting sub pak file but encountered error " + e.ToString() + " extracting as file!");
                }
            }
            File.WriteAllBytes(path, GetFileData(i));
        }
        
        public void ExtractAllFiles(string Folder)
        {
            if (!Directory.Exists(Folder))
                Directory.CreateDirectory(Folder);

            for (int i = 0; i < FileEntries.Count; i++)
            {
                byte[] dat = GetFileData(i);
                ExtractFile(i, Path.Combine(Folder, i.ToString("D4")) + PakExtensionGuesser.GetMagicID(ref dat));
            }
        }
        
        public static void ExtractAllFiles(string packPath, string outFolder)
        {
            Pak pak = new Pak(packPath);
            pak.ExtractAllFiles(outFolder);
            pak.Dispose();
        }
        
        private static byte[] Repack(string Folder)
        {
            MemoryStream stream = new MemoryStream();
            Repack(Folder, stream);
            byte[] data = new byte[stream.Length];
            stream.Position = 0;
            stream.Read(data);
            stream.Dispose();
            stream.Close();
            return data;
        }
        
        private static void Repack(string Folder, Stream stream)
        {
            List<string> temp = Directory.GetFiles(Folder).Where(x => !Path.GetFileName(x).StartsWith('.')).ToList();
            temp.AddRange(Directory.GetDirectories(Folder));
            temp.Sort(new CustomComparer());

            string[] files = temp.ToArray();

            stream.Position = 0;

            MemoryStream datStream = new MemoryStream();
            datStream.Position = 0;

            stream.Write(BitConverter.GetBytes(files.Length));

            int offset = 4 + ((files.Length) * 4);

            for (int i = 0; i < files.Length; i++)
            {
                stream.Write(BitConverter.GetBytes(offset));

                byte[] data = File.Exists(files[i]) ? File.ReadAllBytes(files[i]) : Repack(files[i]);
                datStream.Write(data);

                offset += data.Length;
            }

            datStream.Position = 0;
            datStream.CopyTo(stream);

            datStream.Dispose();
            datStream.Close();
        }
        
        public static void Repack(string Folder, string destination = "")
        {
            if (destination == "")
                destination = Folder + ".pak";

            using (var fs = new FileStream(destination, FileMode.Create, FileAccess.Write))
                Repack(Folder, fs);
        }
        
        public void Dispose()
        {
            PakStream.Dispose();
            PakStream.Close();
            PakReader.Dispose();
            FileEntries.Clear();
        }
        
        public Pak(Stream stream)
        {
            PakStream = stream;
            PakReader = new BinaryReader(PakStream);
            ReadHeader();
        }
        
        public Pak(string filePath)
        {
            PakStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            PakReader = new BinaryReader(PakStream);
            ReadHeader();
        }
        
        public Pak(byte[] data)
        {
            PakStream = new MemoryStream(data);
            PakReader = new BinaryReader(PakStream);
            ReadHeader();
        }
    }
}

