﻿using System;
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

        public Swdl Owner { get; }
        public int Offset { get; }
        public int Length => data.Length;

        private List<object> conv = new();

        public SwdlChunk(Swdl owner, BinaryReader br)
        {
            Owner = owner;

            Stream stream = br.BaseStream;
            Offset = (int)stream.Position;

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

        public TChunk As<TChunk>() where TChunk : SwdlChunk<TChunk>, new()
        {
            foreach(var obj in conv)
            {
                if (obj is TChunk objT)
                    return objT;
            }

            TChunk result = new();

            using var stream = new MemoryStream(data, 0, data.Length, false, false);

            if (!result.ImportLabel(LabelBytes, this, new BinaryReader(stream)))
                throw new InvalidCastException($"Managed {typeof(TChunk).Name} failed to import data from \"{LabelString.Replace("\x20", "\\x20")}\" label.");

            conv.Add(result);

            return result;
        }
    }

    public abstract class SwdlChunk<TChunk> where TChunk : SwdlChunk<TChunk>, new()
    {
        private SwdlChunk? chunk;
        public SwdlChunk From => chunk ?? throw new InvalidOperationException("The chunk is not ready to process that request.");

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
