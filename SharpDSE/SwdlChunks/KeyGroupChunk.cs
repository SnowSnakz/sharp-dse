using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpDSE.SwdlChunks
{
    public sealed class KeyGroupChunk : ISwdlChunk<KeyGroupChunk>
    {
        public bool CanImportLabel(byte[] label)
        {
            return label.SequenceEqual(SwdlChunk.KGRP);
        }

        public void Import(SwdlChunk chunk, BinaryReader reader)
        {
        }
    }
}
