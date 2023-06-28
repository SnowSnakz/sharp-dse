using SharpDSE.Wave;
using SharpDSE.Wave.Common;
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
     *   
     *   Note: The reason why ISampleTableEntry is recommended to be a struct, is because modifying the SampleTable would introduce errors due to C# pass-by-reference. (classes)
     *     This isn't a big deal if you are extremely careful not to reuse any SampleTable entries, but in practice it's better use pass-by-copy (structs) for this.
     */
    public class WaveInfoChunk : SwdlChunk<WaveInfoChunk>, IList<ISampleInfo>, IBinaryReadable, IBinaryWritable
    {
        private readonly List<ISampleInfo> samples = new();
        private SwdlChunk? chunk;

        // A dictionary that is used to convert ids to new ones.
        // On the managed side of things, empty entries are stripped, so all of the ids may change (destructively)
        private readonly Dictionary<int, int> oldref = new();

        public int Count => samples.Count;
        public bool IsReadOnly => false;

        public ISampleInfo this[int index]
        {
            get => samples[index];
            set
            {
                value.ID = checked((ushort)index); // "checked" will throw an exception if an overflow happens during the cast. (Overflow == index too big for ushort)
                samples[index] = value;
            }
        }

        /// <summary>
        /// Same as <see cref="this[int]"/>, but you can specify if the provided <paramref name="index"/> assumes that empty entries still exist.
        /// Since empty entries are removed on the managed side of things, the actual IDs of samples will have changed to remove the gaps.
        /// This can be a problem for songs/banks that have not been updated to the new ids, so setting <paramref name="legacy"/> to true
        /// will allow the correct sample to be retrieved in those cases.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="legacy"></param>
        /// <returns></returns>
        public ISampleInfo Get(int index, bool legacy)
        {
            if(legacy)
            {
                index = oldref[index];
            }

            return samples[index];
        }

        public bool TryFindOldID(int index, out int old)
        {
            foreach((int oldId, int newId) in oldref)
            {
                if(newId == index)
                {
                    old = oldId;
                    return true;
                }
            }

            old = 0;
            return false;
        }

        protected override bool CanImportLabel(byte[] label)
        {
            // Only allow Import to be called on chunks with the wavi label.
            return label.SequenceEqual(SwdlChunk.WAVI);
        }

        protected override void Import(SwdlChunk chunk, BinaryReader reader)
        {
            this.chunk = chunk;

            // Read the entries.
            Read(reader);
        }
        
        protected virtual bool DetectFormat(SwdlChunk chunk, ushort version, BinaryReader reader)
        {
            return false;
        }

        protected void ReadEntries<TEntry>(ushort[] sampleOffsets, BinaryReader br) where TEntry : ISampleInfo, IBinaryReadable, new()
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

                // Add old id to old-reference table.
                oldref.Add(i, samples.Count);

                // Update the id.
                result.ID = (ushort)samples.Count;

                // Add the sample entry.
                samples.Add(result);
            }
        }

        public int IndexOf(ISampleInfo item)
        {
            return samples.IndexOf(item);
        }

        public void Insert(int index, ISampleInfo item)
        {
            item.ID = checked((ushort)index);
            samples.Insert(index, item);
            
            // All entries after the inserted one probably have an outdated index, so update them.
            for(int i = index + 1; i < samples.Count; i++)
            {
                samples[i].ID = checked((ushort)i);
            }

            // Update old ids.
            var oldIds = oldref.Keys.ToArray();
            var newIds = oldref.Values.ToArray();

            for(int i = 0; i < oldIds.Length; i++)
            {
                var oldId = oldIds[i];
                var newId = newIds[i];

                if (newId >= index)
                {
                    oldref[oldId] = newId + 1;
                }
            }
        }

        public void RemoveAt(int index)
        {
            samples.RemoveAt(index);

            // All entries after the removed one probably have an outdated index, so update them.
            for(int i = index; i < samples.Count; i++)
            {
                samples[i].ID = checked((ushort)i);
            }

            // Update old ids.
            var oldIds = oldref.Keys.ToArray();
            var newIds = oldref.Values.ToArray();

            for (int i = 0; i < oldIds.Length; i++)
            {
                var oldId = oldIds[i];
                var newId = newIds[i];

                if (newId == index)
                {
                    oldref.Remove(oldId);
                    continue;
                }

                if (newId > index)
                {
                    oldref[oldId] = newId - 1;
                }
            }
        }

        public void Add(ISampleInfo item)
        {
            // Set the ID of the item being added.
            item.ID = checked((ushort)samples.Count);
            samples.Add(item);
        }

        public void Clear()
        {
            samples.Clear();
        }

        public bool Contains(ISampleInfo item)
        {
            return samples.Contains(item);
        }

        public void CopyTo(ISampleInfo[] array, int arrayIndex)
        {
            samples.CopyTo(array, arrayIndex);
        }

        public bool Remove(ISampleInfo item)
        {
            int index = samples.IndexOf(item);
            
            if(index != -1)
            {
                // Calling RemoveAt because it has code to update the ID properties of the affected entries.
                RemoveAt(index);
                return true;
            }

            return false;
        }

        public IEnumerator<ISampleInfo> GetEnumerator()
        {
            return samples.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return samples.GetEnumerator();
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
                ISampleInfo sample = samples[i];

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
            if (chunk == null)
                throw new InvalidOperationException("Unable to process that request at this time.");

            // Get rid of the previous old-reference table.
            oldref.Clear();

            // Get rid of the previous sample table.
            samples.Clear();

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

                case 0x402:
                    ReadEntries<SampleEntryX402>(sampleOffsets, reader);
                    break;
            }
        }
    }
}
