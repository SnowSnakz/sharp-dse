using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpDSE
{
    public abstract class Chunk<TChunk> where TChunk : Chunk<TChunk>, new()
    {
        private SwdlChunk? chunk;
        public Swdl Owner => chunk?.Owner ?? throw new InvalidOperationException("Attempting to access instance properties before instance is ready.");

        protected abstract bool CanImportLabel(byte[] label);
        protected abstract void Import(SwdlChunk chunk, BinaryReader reader);

        internal bool ImportLabel(byte[] label, SwdlChunk chunk, BinaryReader reader)
        {
            if (!CanImportLabel(label))
                return false;

            this.chunk = chunk;

            Import(chunk, reader);
            return true;
        }
    }
}
