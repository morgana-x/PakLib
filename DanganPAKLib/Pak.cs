using System.Text.RegularExpressions;
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;
namespace DanganPAKLib
{
    public class Pak
    {
        public List<PakEntry> FileEntries { get; set; } = new List<PakEntry>();

        public Stream PakStream { get; set; }

        public void ReadHeader()
        {
            byte[] buf = new byte[4];
            FileEntries.Clear();
            PakStream.Position = 0;

            PakStream.Read(buf);
            int numberOfFiles = BitConverter.ToInt32(buf);

            List<int> offsets = new List<int>();

            //offsets.Add((int)PakStream.Position);

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
                {
                    ent.Size = offsets[i + 1] - offsets[i];
                }
                else
                {
                    ent.Size = (int)PakStream.Length - offsets[i];
                }
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
            for (int i = 0; i < FileEntries.Count; i++)
            {
                byte[] dat = GetFileData(i);
                ExtractFile(i, Folder + i.ToString() + PakExtensionGuesser.GetMagicID(ref dat));
            }
        }
        /*public static string PadNumbers(string input)
        {
            return Regex.Replace(input, "[0-9]+", match => match.Value.PadLeft(10, '0'));
        }*/
        public int Compare(string x, string y)
        {
            var regex = new Regex("^(d+)");

            // run the regex on both strings
            var xRegexResult = regex.Match(x);
            var yRegexResult = regex.Match(y);

            // check if they are both numbers
            if (xRegexResult.Success && yRegexResult.Success)
            {
                return int.Parse(xRegexResult.Groups[1].Value).CompareTo(int.Parse(yRegexResult.Groups[1].Value));
            }

            // otherwise return as string comparison
            return x.CompareTo(y);
        }
        public static void Repack(string Folder, string OutPath = null)
        {
            string[] files = Directory.GetFiles(Folder);
            var myComparer = new CustomComparer();
            List<string> temp = files.ToList();
            temp.Sort(myComparer);
            files = temp.ToArray();

            string destination = "";

            if (OutPath != null)
            {
                destination = OutPath;
            }
            else
            {
                destination = Folder + "_repacked.pak";
            }

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
        public Pak()
        {

        }
        public Pak(Stream stream)
        {
            PakStream = stream;
            ReadHeader();
        }
    }
}
