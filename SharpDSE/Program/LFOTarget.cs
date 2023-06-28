using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpDSE.Program
{
    public enum LFOTarget
    {
        None, 
        Pitch,
        Volume, 
        Panning, 
        /// <summary>
        /// Represents 0x4 in the dest field.
        /// This target is listed as "???" in psy_commando's notes.
        /// </summary>
        _4
    }
}
