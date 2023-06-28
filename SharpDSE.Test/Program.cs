using SharpDSE;
using SharpDSE.SwdlChunks;

Console.WriteLine("Print information about SWDL: (path to swdl file, or blank to skip this test)");
string? file = Console.ReadLine();

if(file != null)
{
    // Remove surrounding quotation marks (quotes are added by dragging the file onto the command prompt window)
    if(file.StartsWith('"') && file.EndsWith('"'))
    {
        file = file[1..^1];
    }
}

// If no file is specified, skip this test.
if (!string.IsNullOrWhiteSpace(file))
{
    // Otherwise, open the specified file
    using var fs = File.OpenRead(file);

    // Try to load it into a managed Swdl object.
    Swdl swdl = new Swdl();
    swdl.Read(new BinaryReader(fs));

    Console.WriteLine();

    // Print some of the basic information contained within the file.
    Console.WriteLine($"File Name:     {swdl.FileName}");
    Console.WriteLine($"Creation Date: {swdl.CreationDate}");
    
    // Print some information about the chunks contained within the file.
    Console.WriteLine($"Chunks:");
    foreach (SwdlChunk chunk in swdl)
    {
        byte[] label = chunk.LabelBytes;

        string type = "Unknown";
        switch (chunk.LabelString)
        {
            case "pcmd":
                type = "Pcm Data Chunk";
                break;

            case "wavi":
                type = "Wave Information Chunk";
                break;

            case "prgi":
                type = "Program Information Chunk";
                break;

            case "kgrp":
                type = "Keygroup Chunk";
                break;

            case "eod\x20":
                type = "End of Data";
                break;
        }

        Console.WriteLine($"  At 0x{chunk.Offset:X8}: {type}");
        Console.WriteLine($"    Label:  {chunk.LabelString} {{0x{label[0]:X2}, 0x{label[1]:X2}, 0x{label[2]:X2}, 0x{label[3]:X2}}}");
        Console.WriteLine($"    Length: 0x{chunk.Length:X8}");

    }

}