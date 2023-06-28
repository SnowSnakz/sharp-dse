using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpDSE
{
    internal static class Extensions
    {
        private static uint ToUInt32_LE(byte[] ibuf)
        {
            if (BitConverter.IsLittleEndian)
                return BitConverter.ToUInt32(ibuf);

            // Convert Big Endian to Little Endian
            Array.Reverse(ibuf);
            return BitConverter.ToUInt32(ibuf);
        }

        private static ushort ToUInt16_LE(byte[] ibuf)
        {
            if (BitConverter.IsLittleEndian)
                return BitConverter.ToUInt16(ibuf);

            // Convert Big Endian to Little Endian
            Array.Reverse(ibuf);
            return BitConverter.ToUInt16(ibuf);
        }

        public static uint ReadUInt32_LE(this BinaryReader br)
        {
            byte[] ibuf = br.ReadBytes(4);
            return ToUInt32_LE(ibuf);
        }

        public static ushort ReadUInt16_LE(this BinaryReader br)
        {
            byte[] ibuf = br.ReadBytes(2);
            return ToUInt16_LE(ibuf);
        }

        public static void Write_LE(this BinaryWriter bw, ushort value)
        {
            byte[] ibuf = BitConverter.GetBytes(value);

            // Convert Big Endian to Little Endian
            if (!BitConverter.IsLittleEndian)
                Array.Reverse(ibuf);

            bw.Write(ibuf);
        }

        public static void Write_LE(this BinaryWriter bw, uint value)
        {
            byte[] ibuf = BitConverter.GetBytes(value);

            // Convert Big Endian to Little Endian
            if (!BitConverter.IsLittleEndian)
                Array.Reverse(ibuf);

            bw.Write(ibuf);
        }
    }
}
