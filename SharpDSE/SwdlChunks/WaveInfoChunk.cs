using SharpDSE.Wave;
using SharpDSE.Wave.SampleTable;
using System.Collections;

namespace SharpDSE.SwdlChunks
{
    /*
     * To add support for a specific version:
     *   Create a child class.
     *   
     *   Override the DetectFormat method.
     *   
     *   Call ReadEntries{TEntry} if your supported version is being used, where TEntry is the structure you want to load.
     *     It is recommended that TEntry is a struct, although this is not required.
     *     TEntry is required to implement ISampleTableEntry and IBinaryReadable, and must have a parameterless constructor.
     *     Implementing IBinaryWritable is recommended, but not required. (You will have holes in saved files if you don't.)
     *     
     *   Return true if you attempt to load anything (regardless of if you actually loaded anything), false otherwise.
     *   
     */
    public class WaveInfoChunk : SwdlChunk<WaveInfoChunk>, IList<ISampleTableEntry>, IBinaryReadable, IBinaryWritable
    {
        List<ISampleTableEntry> samples = new();
        private SwdlChunk chunk;


        public int Count => samples.Count;
        public bool IsReadOnly => false;

        public ISampleTableEntry this[int index] 
        {
            get => samples[index];
            set
            {
                value.ID = checked((ushort)index); // "checked" will throw an exception if an overflow happens during the cast. (Overflow == index too big for ushort)
                samples[index] = value;
            }
        }

        protected override bool CanImportLabel(byte[] label)
        {
            // Only allow Import to be called on chunks with the wavi label.
            return label.SequenceEqual(SwdlChunk.WAVI);
        }

        protected override void Import(SwdlChunk chunk, BinaryReader reader)
        {
            this.chunk = chunk;

            // Allocate sample info table
            samples.Clear();

            Read(reader);

        }

        protected virtual bool DetectFormat(SwdlChunk chunk, ushort version, BinaryReader reader)
        {
            return false;
        }

        private void ReadEntries<TEntry>(ushort[] sampleOffsets, BinaryReader br) where TEntry : ISampleTableEntry, IBinaryReadable, new()
        {
            var stream = br.BaseStream;

            // Read the sample entries
            for (int i = 0; i < sampleOffsets.Length; i++)
            {
                // If nullptr, skip.
                if (sampleOffsets[i] == 0)
                    continue;

                // Seek if not already there.
                if (stream.Position != sampleOffsets[i])
                {
                    stream.Seek(sampleOffsets[i], SeekOrigin.Begin);
                }

                // Read sample entry.
                TEntry result = new();
                result.Read(br);
                samples.Add(result);
            }
        }

        public int IndexOf(ISampleTableEntry item)
        {
            throw new NotImplementedException();
        }

        public void Insert(int index, ISampleTableEntry item)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        public void Add(ISampleTableEntry item)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(ISampleTableEntry item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(ISampleTableEntry[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Remove(ISampleTableEntry item)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<ISampleTableEntry> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public void Write(BinaryWriter writer)
        {
            // Create a temporary stream for writing the sample data
            using var tempStream = new MemoryStream();
            BinaryWriter tbr = new(tempStream);
            
            // Precalculate the size of the WaveTable
            int waveTableSize = samples.Count * 2;

            // Write the offset of each SampleTable Entry into the WaveTable.
            for (int i = 0; i < samples.Count; i++)
            {
                // Grab the current entry
                ISampleTableEntry sample = samples[i];

                // If the entry is writable, record the temporary streams position as the offset.
                if(sample is IBinaryWritable writable)
                {
                    // The offset is the WaveTable's size + the current position of the tempStream.
                    // (The tempStream will be appended to the BaseStream after the WaveTable has been fully written.)
                    writer.Write_LE((ushort)(waveTableSize + tempStream.Position));

                    // Write the entry data into the temporary stream.
                    writable.Write(tbr);
                }
                else
                {
                    // If the entry is not writable, write a null entry. These entries will be unable to load next time the file is loaded.
                    // (This is why having your entries implement IBinaryWritable is recommended.)
                    writer.Write((ushort)0);
                }
            }

            // Write the sample info table entries.
            tempStream.CopyTo(writer.BaseStream);
        }

        public void Read(BinaryReader reader)
        {
            // Read the sample entry offsets.
            ushort[] sampleOffsets = new ushort[chunk.Owner.WaviSamplePointerCount];
            for (int i = 0; i < sampleOffsets.Length; i++)
            {
                sampleOffsets[i] = reader.ReadUInt16_LE();
            }

            // If a child class overriding DetectFormat supports the version, it should have already loaded the entries.
            if (DetectFormat(chunk, chunk.Owner.Version, reader))
                return;

            // Try to load entries.
            switch (chunk.Owner.Version)
            {
                default:
                    throw new NotSupportedException("Unknown SampleTable entry format for swdl version.");

                case 0x415:
                    ReadEntries<SampleEntryX415>(sampleOffsets, reader);
                    break;

                case 0x405:
                    throw new NotImplementedException("Version 0x405 is not yet implemented.");
            }
        }
    }
}
