namespace SharpDSE.SwdlChunks
{
    public sealed class WaveInfoChunk : ISwdlChunk<WaveInfoChunk>
    {
        public bool CanImportLabel(byte[] label)
        {
            // Only allow Import to be called on chunks with the wavi label.
            return label.SequenceEqual(SwdlChunk.WAVI);
        }

        public void Import(SwdlChunk chunk, BinaryReader reader)
        {
        }
    }
}
