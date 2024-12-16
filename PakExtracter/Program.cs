// See https://aka.ms/new-console-template for more information
using DanganPAKLib;

public partial class Program
{
    public static void Extract(string inputPak)
    {
        FileStream stream = new FileStream(inputPak, FileMode.Open, FileAccess.Read);
        Pak pak = new Pak(stream);
        string outPutDirectory = Directory.GetParent(inputPak).FullName + "\\" + Path.GetFileName(inputPak) + "_extracted\\";
        if (!Directory.Exists(outPutDirectory))
            Directory.CreateDirectory(outPutDirectory);
        pak.ExtractAllFiles(outPutDirectory);
        pak.Dispose();

        Console.WriteLine("Extracted " + inputPak);
    }
    public static void Repack(string inputPak)
    {
        //FileStream stream = new FileStream(inputPak, FileMode.Open, FileAccess.Read);
        //Pak pak = new Pak(stream);
        Pak.Repack(inputPak);
        Console.WriteLine("Repacked " + inputPak);
    }
    public static void Main(string[] args)
    {
        while (true)
        {
            Console.Clear();
          
            string inputPak = "";

            if (args.Length < 1)
            {
                Console.WriteLine("Drag and drop the file you want to extract\nOr\nDrag drop folder you want to repack");
                inputPak = Console.ReadLine().Replace("\"", "");
            }
   

            if (File.Exists(inputPak))
            {
                Extract(inputPak);
            }
            else if (Directory.Exists(inputPak))
            {
                Repack(inputPak);
            }
            if (args.Length > 0)
            {
                break;
            }
        }
    }

}
