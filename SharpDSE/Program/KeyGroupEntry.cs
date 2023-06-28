using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpDSE.Program
{
    /// <summary>
    /// The data for a Keygroup Entry. 
    /// Although the entire structure is loaded, variables with unknown purposes are private to make things easier.
    /// The private variables use the exact same type and naming patterns found in psy_commando's notes.
    /// </summary>
    public struct KeyGroupEntry : IBinaryReadable, IBinaryWritable
    {
        /// <summary>
        /// Represents a KeyGroupEntry where all 8 bytes are set to 0xAA, keygroups like this are meant to be ignored.
        /// </summary>
        public static KeyGroupEntry Skip => new() { id = 0xAAAA, polyphony = 0xAA, priority = 0xAA, highestVoice = 0xAA, lowestVoice = 0xAA, unk50 = 0xAA, unk51 = 0xAA };

        public ushort id;
        public byte polyphony;
        public byte priority;
        public byte highestVoice;
        public byte lowestVoice;
        private byte unk50;
        private byte unk51;

        public void Read(BinaryReader reader)
        {
            id = reader.ReadUInt16_LE();
            polyphony = reader.ReadByte();
            priority = reader.ReadByte();
            highestVoice = reader.ReadByte();
            lowestVoice = reader.ReadByte();
            unk50 = reader.ReadByte();
            unk51 = reader.ReadByte();
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write_LE(id);
            writer.Write(polyphony);
            writer.Write(priority);
            writer.Write(highestVoice);
            writer.Write(lowestVoice);
            writer.Write(unk50);
            writer.Write(unk51);
        }
    }
}
