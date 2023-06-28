using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpDSE.Program
{
    public interface ILFOEntry
    {
        /// <summary>
        /// Should this LFO be applied?
        /// </summary>
        bool Enabled { get; set; }

        /// <summary>
        /// The target parameter that the LFO affects.
        /// </summary>
        LFOTarget Target { get; set; }

        /// <summary>
        /// The shape of the WaveForm the LFO uses.
        /// </summary>
        LFOWaveForm WaveForm { get; set; }

        /// <summary>
        /// The rate at which the LFO "oscillates"
        /// </summary>
        ushort OscillationRate { get; set; }

        /// <summary>
        /// The depth parameter of the LFO.
        /// </summary>
        ushort OscillationDepth { get; set; }

        /// <summary>
        /// The delay in Milliseconds before the LFO effect is applied after the sample begins playing.
        /// </summary>
        ushort DelayMS { get; set; }

        /// <summary>
        /// Fade-in/fade-out. Negative = fade out, Positive = fade in.
        /// </summary>
        short Fade { get; set; }
    }
}
