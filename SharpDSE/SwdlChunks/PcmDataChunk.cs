using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpDSE.SwdlChunks
{
    public sealed class PcmDataChunk : Chunk<PcmDataChunk>
    {
        protected override bool CanImportLabel(byte[] label)
        {
            return label.SequenceEqual(SwdlChunk.PCMD);
        }

        protected override void Import(SwdlChunk chunk, BinaryReader reader)
        {
        }
    }
}
