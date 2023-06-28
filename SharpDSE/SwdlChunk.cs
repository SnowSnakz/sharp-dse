using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpDSE
{
    public sealed class SwdlChunk
    {
        public static byte[] WAVI => new byte[4] { 0x77, 0x61, 0x76, 0x69 };
        public static byte[] PRGI => new byte[4] { 0x70, 0x72, 0x67, 0x69 };
        public static byte[] KGRP => new byte[4] { 0x6B, 0x67, 0x72, 0x70 };
        public static byte[] PCMD => new byte[4] { 0x70, 0x63, 0x6D, 0x64 };
        public static byte[] EOD  => new byte[4] { 0x65, 0x6F, 0x64, 0x20 };

        private readonly byte[] label = new byte[4];
        private readonly byte[] data;

        public string LabelString => Encoding.ASCII.GetString(label);
        public char[] LabelChars => new char[4] { (char)label[0], (char)label[1], (char)label[2], (char)label[3] };
        public byte[] LabelBytes => (byte[])label.Clone();

        public int Length => data.Length;

        public SwdlChunk(BinaryReader br)
        {
            Stream stream = br.BaseStream;
            br.ReadBytes(4).CopyTo(label, 0);

            stream.Seek(8, SeekOrigin.Current);
            
            uint length = br.ReadUInt32_LE();
            
            data = new byte[length];

            int l = stream.Read(data);

            int excess = (int)(stream.Position % 16);

            if (excess != 0)
                stream.Seek(16 - excess, SeekOrigin.Current);

            if(l != length) 
                throw new IOException($"Unable to read correct amount of data in {LabelString.Replace("\x20", "\\x20")}.");
        }

        public TChunk As<TChunk>() where TChunk : class, ISwdlChunk<TChunk>, new()
        {
            TChunk result = new();

            if (!result.CanImportLabel(LabelBytes))
                throw new InvalidCastException($"Managed {typeof(TChunk).Name} cannot import data from an unmanaged \"{LabelString.Replace("\x20", "\\x20")}\" chunk.");

            result.Import(this, (byte[])data.Clone());
            return result;
        }
    }
}
