using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpDSE.SwdlChunks
{
    public sealed class ProgramInfoChunk : Chunk<ProgramInfoChunk>
    {
        protected override bool CanImportLabel(byte[] label)
        {
            return label.SequenceEqual(SwdlChunk.PRGI);
        }

        protected override void Import(SwdlChunk chunk, BinaryReader reader)
        {
        }
    }
}
