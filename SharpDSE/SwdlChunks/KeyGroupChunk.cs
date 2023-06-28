using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpDSE.SwdlChunks
{
    public sealed class KeyGroupChunk : Chunk<KeyGroupChunk>
    {
        protected override bool CanImportLabel(byte[] label)
        {
            return label.SequenceEqual(SwdlChunk.KGRP);
        }

        protected override void Import(SwdlChunk chunk, BinaryReader reader)
        {
        }
    }
}
