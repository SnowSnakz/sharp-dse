using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpDSE.Program
{
    public enum LFOWaveForm
    {
        /// <summary>
        /// Probably represents "None" but that is not explicitly defined in psy_commando's notes...
        /// </summary>
        _0,
        Square, 
        Triangle, 
        Sine,
        /// <summary>
        /// Represents 0x4 in the wshape field.
        /// This waveform is listed as "?" in psy_commando's notes.
        /// </summary>
        _4, 
        Saw, 
        Noise, 
        Random
    }
}
