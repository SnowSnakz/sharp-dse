using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpDSE.Wave.SampleTable
{
    /// <summary>
    /// The data for a Sample Table Entry. 
    /// Although the entire structure is loaded, variables with unknown purposes are private to make things easier.
    /// The private variables use the exact same type and naming patterns found in psy_commando's notes.
    /// </summary>
    public struct SampleEntryX415 : ISampleTableEntry, IBinaryWritable, IBinaryReadable
    {
        private ushort unk1; // (possible entry marker)
        public ushort index;
        public sbyte fineTuningCents;
        public sbyte coarseTuning;
        public byte rootKey;
        public sbyte keyTranspose;
        public sbyte volume;
        public sbyte panning;
        private byte unk5;
        private byte unk58;
        private ushort unk6;
        private ushort unk7;
        private ushort version; // (probably equal to Swdl.Version)
        public ushort sampleFormat; // 0x0000 => 8 bit PCM(?); 0x0100 => 16 bit PCM; 0x0200 => 4 bit ADPCM; 0x0300 => PSG(?)
        private byte unk9;
        public byte loopSample;
        private byte unk10;
        public byte samplesPerBlock; // (?)
        private byte unk11;
        public byte bitDepth; // Usually 0x4. Seen 0x10 for the PCM16 samples used for the sine, triangle, square, saw wave samples.
        private byte unk12;
        private byte unk62;
        private byte[] unk13;
        public uint sampleRate;
        public uint pcmdSampleOffset;
        public uint loopStart;
        public uint loopLength;
        public byte envelopeEnabled;
        public byte envelopeMultiplier;
        private byte unk19;
        private byte unk20;
        private ushort unk21;
        private ushort unk22;
        public sbyte attackVolume;
        public sbyte attackTime;
        public sbyte decayTime;
        public sbyte sustainVolume;
        public sbyte holdTime;
        public sbyte decayTime2; // What is this for??? (Possible decay time if the note is released before the sustain phase?)
        public sbyte releaseTime;
        public sbyte unk57;

        public ushort ID { get => index; set => index = value; }

        public byte RootKey { get => rootKey; set => rootKey = value; }

        public sbyte FineTune { get => fineTuningCents; set => fineTuningCents = value; }
        public sbyte CoarseTune { get => coarseTuning; set => coarseTuning = value; }

        public sbyte KeyTranspose => keyTranspose;

        public sbyte Volume { get => volume; set => volume = value; }
        public sbyte Panning { get => panning; set => panning = value; }

        public ushort Version => version;

        public SampleFormat Format
        {
            get
            {
                switch (sampleFormat)
                {
                    default:
                        throw new NotSupportedException($"Unsupported SampleFormat: 0x{sampleFormat:x4}");

                    case 0x0000:
                        return SampleFormat.Pcm8;

                    case 0x0100:
                        return SampleFormat.Pcm16;

                    case 0x0200:
                        return SampleFormat.AdPcm;

                    case 0x0300:
                        return SampleFormat.Psg;
                }
            }
            set
            {
                switch (value)
                {
                    default:
                        throw new NotImplementedException($"SampleFormat.{Enum.GetName(typeof(SampleFormat), value)} is not supported by this implementation.");

                    case SampleFormat.Pcm8:
                        sampleFormat = 0x0000;
                        break;

                    case SampleFormat.Pcm16:
                        sampleFormat = 0x0100;
                        break;

                    case SampleFormat.AdPcm:
                        sampleFormat = 0x0200;
                        break;

                    case SampleFormat.Psg:
                        sampleFormat = 0x0300;
                        break;
                }
            }
        }
        public bool Looped { get => loopSample == 1; set => loopSample = (byte)(value ? 1 : 0); }
        public uint LoopStart { get => loopStart; set => loopStart = value; }
        public uint LoopLength { get => loopLength; set => loopLength = value; }
        public byte SamplesPerBlock { get => samplesPerBlock; set => samplesPerBlock = value; }
        public byte BitDepth { get => bitDepth; set => bitDepth = value; }
        public uint SampleRate { get => sampleRate; set => sampleRate = value; }
        public uint SamplePosition { get => pcmdSampleOffset; set => pcmdSampleOffset = value; }
        public bool EnvelopeEnabled { get => envelopeEnabled != 0; set => envelopeEnabled = (byte)(value ? 1 : 0); }
        public byte EnvelopeMultiplier { get => envelopeMultiplier; set => envelopeMultiplier = value; }
        public sbyte AttackVolume { get => attackVolume; set => attackVolume = value; }
        public sbyte Attack { get => attackTime; set => attackTime = value; }
        public sbyte Decay { get => decayTime; set => decayTime = value; }
        public sbyte Decay2 { get => decayTime2; set => decayTime2 = value; }
        public sbyte SustainVolume { get => sustainVolume; set => sustainVolume = value; }
        public sbyte Hold { get => holdTime; set => holdTime = value; }
        public sbyte Release { get => releaseTime; set => releaseTime = value; }

        public void Read(BinaryReader reader)
        {
            unk1 = reader.ReadUInt16_LE();
            index = reader.ReadUInt16_LE();
            fineTuningCents = reader.ReadSByte();
            coarseTuning = reader.ReadSByte();
            rootKey = reader.ReadByte();
            keyTranspose = reader.ReadSByte();
            volume = reader.ReadSByte();
            panning = reader.ReadSByte();
            unk5 = reader.ReadByte();
            unk58 = reader.ReadByte();
            unk6 = reader.ReadUInt16_LE();
            unk7 = reader.ReadUInt16_LE();
            version = reader.ReadUInt16_LE();
            sampleFormat = reader.ReadUInt16_LE();
            unk9 = reader.ReadByte();
            loopSample = reader.ReadByte(); // flag indicating whether the sample should be looped or not ! 1 = looped, 0 = not looped
            unk10 = reader.ReadByte();
            samplesPerBlock = reader.ReadByte();
            unk11 = reader.ReadByte();
            bitDepth = reader.ReadByte();
            unk12 = reader.ReadByte();
            unk62 = reader.ReadByte();
            unk13 = reader.ReadBytes(4);
            sampleRate = reader.ReadUInt32_LE();
            pcmdSampleOffset = reader.ReadUInt32_LE();
            loopStart = reader.ReadUInt32_LE();
            loopLength = reader.ReadUInt32_LE();
            envelopeEnabled = reader.ReadByte(); // If not == 0, the volume envelope is processed. Otherwise its ignored.
            envelopeMultiplier = reader.ReadByte();
            unk19 = reader.ReadByte();
            unk20 = reader.ReadByte();
            unk21 = reader.ReadUInt16_LE();
            unk22 = reader.ReadUInt16_LE();
            attackVolume = reader.ReadSByte();
            attackTime = reader.ReadSByte();
            decayTime = reader.ReadSByte();
            sustainVolume = reader.ReadSByte();
            holdTime = reader.ReadSByte();
            decayTime2 = reader.ReadSByte();
            releaseTime = reader.ReadSByte();
            unk57 = reader.ReadSByte();
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write_LE(unk1);
            writer.Write_LE(index);
            writer.Write(fineTuningCents);
            writer.Write(coarseTuning);
            writer.Write(rootKey);
            writer.Write(keyTranspose);
            writer.Write(volume);
            writer.Write(panning);
            writer.Write(unk5);
            writer.Write(unk58);
            writer.Write_LE(unk6);
            writer.Write_LE(unk7);
            writer.Write_LE(version);
            writer.Write_LE(sampleFormat);
            writer.Write(unk9);
            writer.Write(loopSample);
            writer.Write(unk10);
            writer.Write(samplesPerBlock);
            writer.Write(unk11);
            writer.Write(BitDepth);
            writer.Write(unk12);
            writer.Write(unk62);
            writer.Write(unk13);
            writer.Write_LE(sampleRate);
            writer.Write_LE(pcmdSampleOffset);
            writer.Write_LE(loopStart);
            writer.Write_LE(loopLength);
            writer.Write(envelopeEnabled);
            writer.Write(envelopeMultiplier);
            writer.Write(unk19);
            writer.Write(unk20);
            writer.Write_LE(unk21);
            writer.Write_LE(unk22);
            writer.Write(attackVolume);
            writer.Write(attackTime);
            writer.Write(decayTime);
            writer.Write(sustainVolume);
            writer.Write(holdTime);
            writer.Write(decayTime2);
            writer.Write(releaseTime);
            writer.Write(unk57);
        }
    }
}
