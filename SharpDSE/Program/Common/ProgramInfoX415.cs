using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpDSE.Program.Common
{
    public struct ProgramInfoX415 : IProgramInfo, IBinaryReadable, IBinaryWritable
    {
        public ushort ID { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public ushort SplitCount { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public sbyte Volume { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public sbyte Panning { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public ILFOEntry[] LFOEntries { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public ISplitsEntry[] Splits { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public KeyGroupEntry[] KeyGroups { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

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
