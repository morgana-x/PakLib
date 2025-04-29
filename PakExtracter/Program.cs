using DanganPAKLib;

public partial class Program
{
    public static void Execute(string path)
    {
        if (File.Exists(path))
        {
            Pak.ExtractAllFiles(path, path.Replace(".pak", "_extracted"));
            Console.WriteLine($"Extracted {path}.");
            return;
        }

        if (Directory.Exists(path))
        {
            Pak.Repack(path);
            Console.WriteLine($"Repacked {path}.");
            return;
        }

        Console.WriteLine($"Couldn't find file or folder \"{path}\"");
    }
    public static void Main(string[] args)
    {
        if (args.Length > 0)
        {
            Execute(args[0]);
            return;
        }

        while (true)
        {
            Console.Clear();
            Console.WriteLine("Drag and drop the Pak file to Extract or Folder to Repack");
            Execute(Console.ReadLine().Replace("\"",""));
        }
    }
}
