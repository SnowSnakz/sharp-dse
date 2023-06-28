using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpDSE.Wave
{
    /// <summary>
    /// Represents an entry into the wavi chunk's sample info table.
    /// </summary>
    public interface ISampleTableEntry
    {
        /// <summary>
        /// The ID / Index of this sample.
        /// </summary>
        /// <remarks>
        /// According to psy_commando's notes: "Empty entries in the Wave Table are counted!"
        /// </remarks>
        ushort ID { get; set; }

        /// <summary>
        /// The MIDI note associated with the sample. (The note that the instrument sampled is playing.) 
        /// </summary>
        byte RootKey { get; set; }

        /// <summary>
        /// The fine tuning of the samples pitch (in cents)
        /// </summary>
        sbyte FineTune { get; set; }

        /// <summary>
        /// Coarse tuning. 
        /// </summary>
        /// <remarks>
        /// According to psy_commando's notes: "Default is -7 (?)"
        /// </remarks>
        sbyte CoarseTune { get; set; }

        /// <summary>
        /// The key transpose.
        /// </summary>
        /// <remarks>
        /// According to psy_commando's notes: "The Key Transpose is basically the difference between rootkey and 60."
        /// </remarks>
        sbyte KeyTranspose { get; }

        /// <summary>
        /// The volume of this sample.
        /// </summary>
        sbyte Volume { get; set; }

        /// <summary>
        /// Stereo panning (negative = left, positive = right, 0 = center)
        /// </summary>
        sbyte Panning { get; set; }

        /// <summary>
        /// See issue #1 on https://github.com/SnowSnakz/sharp-dse for more details.
        /// </summary>
        ushort Version { get; }

        /// <summary>
        /// The format of the sample.
        /// </summary>
        SampleFormat Format { get; set; }

        /// <summary>
        /// Should the sample be looped?
        /// </summary>
        bool Looped { get; set; }

        /// <summary>
        /// The sample index of the loop's starting point.
        /// </summary>
        uint LoopStart { get; set; }

        /// <summary>
        /// The length of the loop in blocks. (1 block = 4 bytes)
        /// </summary>
        uint LoopLength { get; set; }

        /// <summary>
        /// The number of samples per block (1 block = 4 bytes)
        /// </summary>
        byte SamplesPerBlock { get; set; }

        /// <summary>
        /// The bit depth of each sample.
        /// </summary>
        byte BitDepth { get; set; }

        /// <summary>
        /// The sample rate in hertz.
        /// </summary>
        uint SampleRate { get; set; }

        /// <summary>
        /// The offset of the sample inside of the PCMD chunk.
        /// </summary>
        /// <remarks>
        /// If there isn't a PCMD chunk, this is possibly the offset of the exact sample among all the sample data loaded in memory.
        /// </remarks>
        uint SamplePosition { get; set; }

        /// <summary>
        /// Should the volume envelope be processed?
        /// </summary>
        bool EnvelopeEnabled { get; set; }

        /// <summary>
        /// Used as a multiplier for envelope paramters.
        /// </summary>
        byte EnvelopeMultiplier { get; set; }

        /// <summary>
        /// Higher values towards 0x7F means the volume at which the attack phase begins at is louder. Doesn't shorten the attack time.
        /// </summary>
        sbyte AttackVolume { get; set; }

        /// <summary>
        /// Higher values towards 0x7F means the attack phase takes longer to reach full volume! 126 is roughly 10 seconds.
        /// </summary>
        sbyte Attack { get; set; }

        /// <summary>
        /// The duration the note has to be held until the volume is smoothly decreased to the value of "Sustain Volume"
        /// </summary>
        sbyte Decay { get; set; }

        /// <summary>
        /// Higher values towards 0x7F means longer fade-out. 0x7F means never fade-out.
        /// </summary>
        sbyte Decay2 { get; set; }

        /// <summary>
        /// The volume at which the held note will stay at.
        /// </summary>
        sbyte SustainVolume { get; set; }

        /// <summary>
        /// Higher values towards 0x7F means the note is held at full volume longer after the attack phase.
        /// </summary>
        sbyte Hold { get; set; }

        /// <summary>
        /// Higher values towards 0x7F means longer release.
        /// </summary>
        sbyte Release { get; set; }


    }
}
