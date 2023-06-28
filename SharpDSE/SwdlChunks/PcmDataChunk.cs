using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpDSE.SwdlChunks
{
    public sealed class PcmDataChunk : ISwdlChunk<PcmDataChunk>
    {
        public bool CanImportLabel(byte[] label)
        {
            return label.SequenceEqual(SwdlChunk.PCMD);
        }

        public void Import(SwdlChunk chunk, byte[] data)
        {
        }
    }
}
