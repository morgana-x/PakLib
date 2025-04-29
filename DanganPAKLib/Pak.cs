namespace DanganPAKLib
{
    public class Pak
    {
        public List<PakEntry> FileEntries { get; set; } = new List<PakEntry>();

        private Stream PakStream { get; set; }

        private void ReadHeader()
        {
            byte[] buf = new byte[4];
            FileEntries.Clear();
            PakStream.Position = 0;

            PakStream.Read(buf);
            int numberOfFiles = BitConverter.ToInt32(buf);

            List<int> offsets = new List<int>();

            for (int i = 0; i < numberOfFiles; i++)
            {
                PakStream.Read(buf);
                offsets.Add(BitConverter.ToInt32(buf));
            }
            for (int i = 0; i < numberOfFiles; i++)
            {
                PakEntry ent = new PakEntry()
                {
                    Index = i,
                    Offset = offsets[i],
                };

                if (i < numberOfFiles - 1)
                    ent.Size = offsets[i + 1] - offsets[i];
                else
                    ent.Size = (int)PakStream.Length - offsets[i];

                FileEntries.Add(ent);
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

        public static void Repack(string Folder, string destination = "")
        {
            List<string> temp = Directory.GetFiles(Folder).OrderBy(f => f).ToList();

            temp.Sort(new CustomComparer());

            string[] files = temp.ToArray();

            if (destination == "")
                destination = Folder + ".pak";

            FileStream stream = new FileStream(destination, FileMode.Create);
            stream.Position = 0;


            MemoryStream datStream = new MemoryStream();
            datStream.Position = 0;


            stream.Write(BitConverter.GetBytes(files.Length));

            int offset = 4 + ((files.Length) * 4);

            for (int i = 0; i < files.Length; i++)
            {
                stream.Write(BitConverter.GetBytes(offset));
                byte[] data = File.ReadAllBytes(files[i]);
                datStream.Write(data);
                offset += data.Length;
            }

            datStream.Position = 0;
            datStream.CopyTo(stream);

            datStream.Dispose();
            datStream.Close();

            stream.Dispose();
            stream.Close();
        }
        public void Dispose()
        {
            PakStream.Dispose();
            PakStream.Close();
            FileEntries.Clear();
        }
        public Pak(Stream stream)
        {
            PakStream = stream;
            ReadHeader();
        }
        public Pak(string filePath)
        {
            PakStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            ReadHeader();
        }
    }
}
