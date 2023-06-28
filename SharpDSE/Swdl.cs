using System.Collections;
using System.Numerics;
using System.Text;

namespace SharpDSE
{
    public sealed class Swdl : IEnumerable<SwdlChunk>
    {
        private static readonly byte[] Magic = new byte[4] { 0x73, 0x77, 0x64, 0x6C };

        private DateTime creationDate;
        private string fileName = "<unknown>"; 
        private readonly List<SwdlChunk> chunks = new();

        private byte bankId, swdlId;
        private int samplePointerCount, presetPointerCount;

        public DateTime CreationDate => creationDate;
        public string FileName => fileName;

        public byte BankID => bankId;
        public byte SwdlID => swdlId;

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

        public void Read(BinaryReader br)
        {
            Stream stream = br.BaseStream;

            byte[] ibuf = br.ReadBytes(4);

            if (!ibuf.SequenceEqual(Magic))
                throw new InvalidDataException();

            stream.Seek(4, SeekOrigin.Current);

            uint flen = br.ReadUInt32_LE();
            
            ushort version = br.ReadUInt16_LE();
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

                // add chunk to chunk list
                chunks.Add(current);
            }
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