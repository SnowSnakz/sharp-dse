using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SharpDSE.Wave
{
    public class SampleData
    {
        // ADPCM Stuff
        private static readonly sbyte[] ImaIndexTable = new sbyte[16]
        {
            -1, -1, -1, -1, 2, 4, 6, 8,
            -1, -1, -1, -1, 2, 4, 6, 8
        };

        private static readonly short[] ImaStepTable = new short[89]
        {
            7, 8, 9, 10, 11, 12, 13, 14, 16, 17,
            19, 21, 23, 25, 28, 31, 34, 37, 41, 45,
            50, 55, 60, 66, 73, 80, 88, 97, 107, 118,
            130, 143, 157, 173, 190, 209, 230, 253, 279, 307,
            337, 371, 408, 449, 494, 544, 598, 658, 724, 796,
            876, 963, 1060, 1166, 1282, 1411, 1552, 1707, 1878, 2066,
            2272, 2499, 2749, 3024, 3327, 3660, 4026, 4428, 4871, 5358,
            5894, 6484, 7132, 7845, 8630, 9493, 10442, 11487, 12635, 13899,
            15289, 16818, 18500, 20350, 22385, 24623, 27086, 29794, 32767
        };

        protected short[] data = Array.Empty<short>();
        private uint sampleRate;

        public short[] PcmData => (short[])data.Clone();
        public uint SampleRate => sampleRate;

        public virtual void Load(ISampleInfo entry, BinaryReader reader)
        {
            sampleRate = entry.SampleRate;
            data = new short[entry.LoopLength * entry.SamplesPerBlock];

            switch (entry.Format)
            {
                default:
                    throw new NotImplementedException();

                case SampleFormat.Pcm8:
                    LoadPcm8(reader);
                    break;

                case SampleFormat.Pcm16:
                    LoadPcm16(reader);
                    break;

                case SampleFormat.AdPcm:
                    LoadAdPcm(reader);
                    break;
            }
        }

        protected void LoadPcm8(BinaryReader reader)
        {
            for(int i = 0; i < data.Length; i++)
            {
                data[i] = (short)(reader.ReadSByte() * 128);
            }
        }

        protected void LoadPcm16(BinaryReader reader)
        {
            for(int i = 0; i < data.Length; i++)
            {
                byte[] ibuf = reader.ReadBytes(2);

                if (!BitConverter.IsLittleEndian)
                    Array.Reverse(ibuf);

                data[i] = BitConverter.ToInt16(ibuf);
            }
        }

        private void MoveForward(ref int predictor, ref short stepIndex, ref short step, byte nibble)
        {
            stepIndex += ImaIndexTable[nibble];

            if (stepIndex < 0) stepIndex = 0;
            if (stepIndex > 88) stepIndex = 88;

            bool sign = (nibble & 8) == 8;
            int delta = nibble & 7;
            int diff = step >> 3;
            if ((delta & 4) == 4) diff += step;
            if ((delta & 2) == 2) diff += (step >> 1);
            if ((delta & 1) == 1) diff += (step >> 2);
            if (sign) predictor -= (short)diff;
            else predictor += (short)diff;

            step = ImaStepTable[stepIndex];
        }

        protected void LoadAdPcm(BinaryReader reader)
        {
            byte[] ibuf = reader.ReadBytes(2);

            if (!BitConverter.IsLittleEndian)
                Array.Reverse(ibuf);

            int predictor = BitConverter.ToInt16(ibuf);

            ibuf = reader.ReadBytes(2);

            short stepIndex = ibuf[0] < 89 ? (ibuf[1] < 89 ? ibuf[1] : ibuf[0]) : ibuf[1];
            short step = ImaStepTable[stepIndex % 89];

            for (int i = 0; i < data.Length; i += 2)
            {
                byte samp2 = reader.ReadByte();

                byte n1 = (byte)((samp2 & 0xF0) >> 4);
                byte n2 = (byte)(samp2 & 0x0F);

                MoveForward(ref predictor, ref stepIndex, ref step, n1);

                data[i] = (short)(predictor = Math.Clamp(predictor, short.MinValue, short.MaxValue));

                // Don't process second nibble if end of data is reached.
                if (i + 1 >= data.Length)
                    break;

                MoveForward(ref predictor, ref stepIndex, ref step, n2);

                data[i + 1] = (short)(predictor = Math.Clamp(predictor, short.MinValue, short.MaxValue));
            }
        }
    }
}
