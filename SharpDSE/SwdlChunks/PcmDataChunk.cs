using SharpDSE.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpDSE.SwdlChunks
{
    public class PcmDataChunk : SwdlChunk<PcmDataChunk>
    {
        MemoryStream? permanent;
        BinaryReader? sampleReader;

        protected override bool CanImportLabel(byte[] label)
        {
            return label.SequenceEqual(SwdlChunk.PCMD);
        }

        protected override void Import(SwdlChunk chunk, BinaryReader reader)
        {
            permanent = new MemoryStream();
            sampleReader = new BinaryReader(permanent);

            reader.BaseStream.CopyTo(permanent);
        }

        public virtual SampleData LoadSampleData(ISampleTableEntry entry)
        {
            if (permanent == null || sampleReader == null)
                throw new InvalidOperationException("Not ready to process that request at the moment.");

            permanent.Seek(entry.SamplePosition, SeekOrigin.Begin);

            var result = new SampleData();
            result.Load(entry, sampleReader);
            return result;
        }
    }
}
