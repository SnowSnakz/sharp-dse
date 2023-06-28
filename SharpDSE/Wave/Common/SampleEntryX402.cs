using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpDSE.Wave.Common
{
    public struct SampleEntryX402 : ISampleInfo, IBinaryReadable, IBinaryWritable
    {
        public ushort ID { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public byte RootKey { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public sbyte FineTune { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public sbyte CoarseTune { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public sbyte KeyTranspose => throw new NotImplementedException();

        public sbyte Volume { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public sbyte Panning { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public ushort Version => throw new NotImplementedException();

        public SampleFormat Format { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool Looped { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public uint LoopStart { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public uint LoopLength { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public byte SamplesPerBlock { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public byte BitDepth { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public uint SampleRate { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public uint SamplePosition { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool EnvelopeEnabled { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public byte EnvelopeMultiplier { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public sbyte AttackVolume { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public sbyte Attack { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public sbyte Decay { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public sbyte Decay2 { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public sbyte SustainVolume { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public sbyte Hold { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public sbyte Release { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void Read(BinaryReader reader)
        {
            throw new NotImplementedException();
        }

        public void Write(BinaryWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}
