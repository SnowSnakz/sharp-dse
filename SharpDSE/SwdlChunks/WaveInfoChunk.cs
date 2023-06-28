namespace SharpDSE.SwdlChunks
{
    // Anything marked with a ? means psy_commando put in the notes that it's only a possibility (not yet confirmed) that something is the correct usage of the raw data here.

    public sealed class WaveInfoChunk : SwdlChunk<WaveInfoChunk>
    {
        /// <summary>
        /// The data for a Sample Table Entry. 
        /// Although the entire structure is loaded, variables with unknown purposes are private to make things easier.
        /// The private variables use the exact same type and naming patterns found in psy_commando's notes.
        /// </summary>
        public struct SampleEntryX415
        {
            private ushort unk1; // (possible entry marker)
            public ushort index;
            public sbyte fineTuningCents;
            public sbyte coarseTuning;
            public byte rootKey;
            public sbyte keyTranspose;
            public sbyte volume;
            public sbyte panning;
            private byte unk5;
            private byte unk58;
            private ushort unk6;
            private ushort unk7;
            private ushort version; // (probably equal to Swdl.Version)
            public ushort sampleFormat; // 0x0000 => 8 bit PCM(?); 0x0100 => 16 bit PCM; 0x0200 => 4 bit ADPCM; 0x0300 => PSG(?)
            private byte unk9;
            public bool loopSample;
            private byte unk10;
            public byte samplesPerBlock; // (?)
            private byte unk11;
            public byte bitDepth; // Usually 0x4. Seen 0x10 for the PCM16 samples used for the sine, triangle, square, saw wave samples.
            private byte unk12;
            private byte unk62;
            private byte[] unk13;
            public uint sampleRate;
            public uint pcmdSampleOffset;
            public uint loopStart;
            public uint loopEnd;
            public bool envelopeEnabled;
            public byte envelopeMultiplier;
            private byte unk19;
            private byte unk20;
            private ushort unk21;
            private ushort unk22;
            public sbyte attackVolume;
            public sbyte attackTime;
            public sbyte decayTime;
            public sbyte sustainVolume;
            public sbyte holdTime;
            public sbyte decayTime2; // What is this for??? (Possible decay time if the note is released before the sustain phase?)
            public sbyte releaseTime;
            public sbyte unk57;


            public SampleEntryX415(BinaryReader br)
            {
                unk1 = br.ReadUInt16_LE();
                index = br.ReadUInt16_LE();
                fineTuningCents = br.ReadSByte();
                coarseTuning = br.ReadSByte();
                rootKey = br.ReadByte();
                keyTranspose = br.ReadSByte();
                volume = br.ReadSByte();
                panning = br.ReadSByte();
                unk5 = br.ReadByte();
                unk58 = br.ReadByte();
                unk6 = br.ReadUInt16_LE();
                unk7 = br.ReadUInt16_LE();
                version = br.ReadUInt16_LE();
                sampleFormat = br.ReadUInt16_LE();
                unk9 = br.ReadByte();
                loopSample = br.ReadByte() == 1; // flag indicating whether the sample should be looped or not ! 1 = looped, 0 = not looped
                unk10 = br.ReadByte();
                samplesPerBlock = br.ReadByte();
                unk11 = br.ReadByte();
                bitDepth = br.ReadByte();
                unk12 = br.ReadByte();
                unk62 = br.ReadByte();
                unk13 = br.ReadBytes(4);
                sampleRate = br.ReadUInt32_LE();
                pcmdSampleOffset = br.ReadUInt32_LE();
                loopStart = br.ReadUInt32_LE();
                loopEnd = br.ReadUInt32_LE();
                envelopeEnabled = br.ReadByte() != 0; // If not == 0, the volume envelope is processed. Otherwise its ignored.
                envelopeMultiplier = br.ReadByte();
                unk19 = br.ReadByte();
                unk20 = br.ReadByte();
                unk21 = br.ReadUInt16_LE();
                unk22 = br.ReadUInt16_LE();
                attackVolume = br.ReadSByte();
                attackTime = br.ReadSByte();
                decayTime = br.ReadSByte();
                sustainVolume = br.ReadSByte();
                holdTime = br.ReadSByte();
                decayTime2 = br.ReadSByte();
                releaseTime = br.ReadSByte();
                unk57 = br.ReadSByte();
            }
        }

        private SampleEntryX415[] v415_samples = Array.Empty<SampleEntryX415>();
        
        public int SampleEntryCount => v415_samples.Length;
        public SampleEntryX415 GetSampleX415(int sampleIndex)
        {
            return v415_samples[sampleIndex];
        }

        protected override bool CanImportLabel(byte[] label)
        {
            // Only allow Import to be called on chunks with the wavi label.
            return label.SequenceEqual(SwdlChunk.WAVI);
        }

        protected override void Import(SwdlChunk chunk, BinaryReader reader)
        {
            // Grab a reference to the stream (used for seeking)
            var stream = reader.BaseStream;

            // Read the sample entry offsets.
            ushort[] sampleOffsets = new ushort[chunk.Owner.WaviSamplePointerCount];
            for(int i = 0; i < sampleOffsets.Length; i++)
            {
                sampleOffsets[i] = reader.ReadUInt16_LE();
            }

            // Allocate sample info table
            v415_samples = new SampleEntryX415[sampleOffsets.Length];
            
            // Read the sample entries
            for(int i = 0; i < sampleOffsets.Length; i++)
            {
                // If nullptr, skip.
                if (sampleOffsets[i] == 0)
                    continue;
                
                // Seek if not already there.
                if(stream.Position != sampleOffsets[i])
                {
                    stream.Seek(sampleOffsets[i], SeekOrigin.Begin);
                }

                // Read sample entry.
                v415_samples[i] = new SampleEntryX415(reader);
            }
        }
    }
}
