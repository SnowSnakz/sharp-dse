// See https://aka.ms/new-console-template for more information
using SharpDSE;
using SharpDSE.SwdlChunks;

Console.WriteLine("Print information about SWDL: (path to swdl file, or blank to skip this test)");
string? file = Console.ReadLine();

if(file != null)
{
    if(file.StartsWith('"') && file.EndsWith('"'))
    {
        file = file[1..^1];
    }
}

if (!string.IsNullOrWhiteSpace(file))
{
    using var fs = File.OpenRead(file);

    Swdl swdl = new Swdl();
    swdl.Read(new BinaryReader(fs));

    Console.WriteLine();

    Console.WriteLine($"File Name: {swdl.FileName}");
    Console.WriteLine($"Creation Date: {swdl.CreationDate}");
    Console.WriteLine($"Chunks:");
    foreach (SwdlChunk chunk in swdl)
    {
        Console.WriteLine($"\t{chunk.LabelString.Replace("\x20", "\\x20")}: [Length = 0x{chunk.Length:x}]");
    }

}