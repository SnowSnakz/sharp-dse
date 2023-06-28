using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpDSE.SwdlChunks
{
    public sealed class ProgramInfoChunk : ISwdlChunk<ProgramInfoChunk>
    {
        public bool CanImportLabel(byte[] label)
        {
            return label.SequenceEqual(SwdlChunk.PRGI);
        }

        public void Import(SwdlChunk chunk, byte[] data)
        {
        }
    }
}
