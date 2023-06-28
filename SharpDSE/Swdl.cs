using System.Collections;
using System.Collections.Concurrent;
using System.Numerics;
using System.Text;

namespace SharpDSE
{
    public sealed class Swdl : IEnumerable<SwdlChunk>
    {
        private class WaitForChunkTask
        {
            public bool hasChunk;
            public SwdlChunk? chunk;
            public byte[] label;

            public WaitForChunkTask(byte[] label)
            { 
                this.label = label;
            }

            public static SwdlChunk TaskFn(object? state)
            {
                if (state is not WaitForChunkTask task)
                    throw new InvalidOperationException("What...?");

                // Wait for hasChunk to be true.
                for (; !task.hasChunk;) ;

                // If the chunk is null, it's probably not present in the file.
                if (task.chunk == null)
                    throw new InvalidOperationException("The specified chunk did not load.");

                // Return the loaded chunk
                return task.chunk;
            }
        }

        private static readonly byte[] Magic = new byte[4] { 0x73, 0x77, 0x64, 0x6C };

        private DateTime creationDate;
        private string fileName = "<unknown>"; 
        private readonly List<SwdlChunk> chunks = new();

        private byte bankId, swdlId;
        private int samplePointerCount, presetPointerCount;
        private ushort version;

        public DateTime CreationDate => creationDate;
        public string FileName => fileName;
        public ushort Version => version;

        public byte BankID => bankId;
        public byte SwdlID => swdlId;

        public int ChunkCount => chunks.Count;

        public int WaviSamplePointerCount => samplePointerCount;
        public int PrgiPresetPointerCount => presetPointerCount;

        public SwdlChunk? GetChunk(byte[] label)
        {
            foreach(SwdlChunk chunk in chunks)
            {
                if (chunk.LabelBytes.SequenceEqual(label))
                    return chunk;
            }

            return null;
        }

        List<WaitForChunkTask> waiting = new();

        public async Task<SwdlChunk> WaitForChunk(byte[] label)
        {
            SwdlChunk? result = GetChunk(label);

            if(result != null)
            {
                return await Task.FromResult(result);
            }

            var state = new WaitForChunkTask(label);
            waiting.Add(state);

            return await new Task<SwdlChunk>(WaitForChunkTask.TaskFn, state);
        }

        public void Read(BinaryReader br)
        {
            chunks.Clear();

            Stream stream = br.BaseStream;

            byte[] ibuf = br.ReadBytes(4);

            if (!ibuf.SequenceEqual(Magic))
                throw new InvalidDataException();

            stream.Seek(4, SeekOrigin.Current);

            uint flen = br.ReadUInt32_LE();
            
            version = br.ReadUInt16_LE();
            if(version != 0x415)
                throw new NotSupportedException("Only swdl version 0x415 is supported at this time.");

            bankId = br.ReadByte();
            swdlId = br.ReadByte();

            stream.Seek(8, SeekOrigin.Current);

            // Creation Date
            ushort year = br.ReadUInt16_LE();
            byte month = br.ReadByte();
            byte day = br.ReadByte();
            byte hour = br.ReadByte();
            byte minute = br.ReadByte();
            byte second = br.ReadByte();
            byte centisecond = br.ReadByte();

            creationDate = new DateTime(year, month, day, hour, minute, second, centisecond * 10);

            char[] fname = br.ReadChars(16);

            int i;
            for(i = 0; i < 16; i++)
            {
                if (fname[i] == '\0')
                    break;
            }

            fileName = new string(fname, 0, i);

            stream.Seek(16, SeekOrigin.Current);
            uint pcmdlen = br.ReadUInt32_LE();

            bool externPcmd = (pcmdlen & 0xFFFF0000) == 0xAAAA0000;

            stream.Seek(2, SeekOrigin.Current);

            samplePointerCount = br.ReadUInt16_LE();
            presetPointerCount = br.ReadUInt16_LE();

            stream.Seek(2, SeekOrigin.Current);

            uint wavilen = br.ReadUInt32_LE();

            SwdlChunk? current = null;
            while(current == null || current.LabelString != "eod\x20")
            {
                current = new SwdlChunk(this, br);

                switch(current.LabelString)
                {
                    case "pcmd":
                        // Do not validate pcmdlen if an external pcmd is used.
                        if (externPcmd)
                            break;

                        // validate pcmdlen
                        if(pcmdlen != current.Length)
                        {
                            throw new InvalidDataException("The actual length of the pcmd chunk does not match the length specified in the swdl header.");
                        }

                        break;

                    case "wavi":
                        // validate wavilen
                        if(wavilen != current.Length)
                        {
                            throw new InvalidDataException("The actual length of the wavi chunk does not match the length specified in the swdl header.");
                        }
                        break;
                }

                for(int j = 0; j < waiting.Count; j++)
                {
                    var state = waiting[j];

                    if(state.label.SequenceEqual(current.LabelBytes))
                    {
                        state.chunk = current;
                        state.hasChunk = true;
                    }

                    waiting.RemoveAt(j--);
                }

                // add chunk to chunk list
                chunks.Add(current);
            }

            foreach(var state in waiting)
            {
                state.hasChunk = true;
            }

            waiting.Clear();
        }

        public IEnumerator<SwdlChunk> GetEnumerator()
        {
            return chunks.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return chunks.GetEnumerator();
        }
    }
}