using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpDSE.Program.Common
{
    public struct LFOEntry : ILFOEntry, IBinaryReadable, IBinaryWritable
    {
        public bool Enabled { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public LFOTarget Target { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public LFOWaveForm WaveForm { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public ushort OscillationRate { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public ushort OscillationDepth { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public ushort DelayMS { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public short Fade { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

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
