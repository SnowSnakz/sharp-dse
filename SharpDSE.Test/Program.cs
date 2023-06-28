using OpenTK.Audio.OpenAL;
using SharpDSE;
using SharpDSE.SwdlChunks;
using SharpDSE.Wave;

// Initialize OpenAL
ALDevice device = ALC.OpenDevice(null);
if (device == ALDevice.Null)
    throw new InvalidOperationException("Failed to open OpenAL device");

ALContext context = ALC.CreateContext(device, Array.Empty<int>());
if (context == ALContext.Null)
    throw new InvalidOperationException("Failed to init OpenAL context");

ALC.MakeContextCurrent(context);

Console.WriteLine($"OpenAL: {AL.Get(ALGetString.Version)} {AL.Get(ALGetString.Vendor)}");


int source = AL.GenSource();
int buffer;// = AL.GenBuffer();

// AL.BindBufferToSource(source, buffer);

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
    DateTime start = DateTime.Now;

    Console.WriteLine();
    Console.WriteLine($"Attempting to load \"{file}\" as SWDL file (Length = 0x{fs.Length:x})");

    Swdl swdl = new Swdl();
    swdl.Read(new BinaryReader(fs));

    Console.WriteLine($"Took {DateTime.Now - start} to load the specified file.");
    Console.WriteLine();

    // Print some of the basic information contained within the file.
    Console.WriteLine($"Version:       0x{swdl.Version:X4}");
    Console.WriteLine($"File Name:     {swdl.FileName}");
    Console.WriteLine($"Creation Date: {swdl.CreationDate}");

    bool hadSamples = false;

    // Print some information about the chunks contained within the file.
    Console.WriteLine($"Chunks: {swdl.ChunkCount}");
    foreach (SwdlChunk chunk in swdl)
    {
        byte[] label = chunk.LabelBytes;

        string type = "Unknown";
        int extra = 0;

        switch (chunk.LabelString)
        {
            case "pcmd":
                type = "Pcm Data Chunk";
                break;

            case "wavi":
                type = "Wave Information Chunk";
                extra = 1;
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

        switch(extra)
        {
            default:
                break;

            case 1:
                WaveInfoChunk wavi = chunk.As<WaveInfoChunk>();
                Console.WriteLine($"    Sample Count: {wavi.Count}");

                hadSamples = wavi.Count > 0;

                break;
        }
    }

    Console.WriteLine();

    if(hadSamples)
    {
        Console.Write("Would you like to play all of the samples from the swdl file? [Y/N, default=N]: ");

        bool playSamples;

        ConsoleKeyInfo ki = Console.ReadKey();
        Console.WriteLine();

        switch(ki.KeyChar)
        {
            default:
                playSamples = false;
                break;

            case 'Y':
            case 'y':
                playSamples = true;
                break;
        }

        if(playSamples)
        {
            WaveInfoChunk? wavi = swdl.GetChunk(SwdlChunk.WAVI)?.As<WaveInfoChunk>();

            if(wavi == null)
                throw new InvalidOperationException();

            PcmDataChunk? pcmd = swdl.GetChunk(SwdlChunk.PCMD)?.As<PcmDataChunk>();

            if(pcmd == null)
            {
                Console.Write("The current swdl file does not define a PCMD chunk, which likely means it's referencing an external PCMD chunk... Would you like to load the SWDL file that contains this chunk? ");
                if(Console.ReadKey().KeyChar == 'y')
                {
                    Console.WriteLine();
                    Console.WriteLine("Please specify the path to the external swdl file: ");

                    using var fs2 = File.OpenRead(Console.ReadLine() ?? "");

                    Swdl external = new Swdl();
                    external.Read(new BinaryReader(fs2));

                    pcmd = external.GetChunk(SwdlChunk.PCMD)?.As<PcmDataChunk>();
                }
            }

            if(pcmd == null)
                throw new InvalidDataException("No PCMD chunk found in specified file...");

            if (playSamples)
            {
                for(int i = 0; i < wavi.Count; i++)
                {
                    var sample = wavi[i];

                    Console.WriteLine($"Loading {i} of {wavi.Count}... ({sample.BitDepth}-bits {sample.Format} - {sample.LoopLength * sample.SamplesPerBlock} samples @ {sample.SampleRate}hz)");

                    SampleData data = pcmd.LoadSampleData(sample);

                    buffer = AL.GenBuffer();
                    AL.BufferData(buffer, ALFormat.Mono16, data.PcmData, (int)sample.SampleRate);
                    AL.BindBufferToSource(source, buffer);

                    Console.Write("Playing... ");
                    AL.SourcePlay(source);

                    while(AL.GetSourceState(source) == ALSourceState.Playing);

                    Console.WriteLine("Done!");
                    Console.WriteLine();

                    AL.DeleteBuffer(buffer);
                }
            }

        }
    }

}