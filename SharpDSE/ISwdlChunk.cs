using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpDSE
{
    public interface ISwdlChunk<TChunk> where TChunk : class, ISwdlChunk<TChunk>, new()
    {
        bool CanImportLabel(byte[] label);
        void Import(SwdlChunk chunk, byte[] data);
    }
}
