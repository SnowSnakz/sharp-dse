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

            Array.Reverse(ibuf);
            return BitConverter.ToUInt32(ibuf);
        }

        private static ushort ToUInt16_LE(byte[] ibuf)
        {
            if (BitConverter.IsLittleEndian)
                return BitConverter.ToUInt16(ibuf);

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
    }
}
