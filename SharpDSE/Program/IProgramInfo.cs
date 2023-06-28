using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpDSE.Program
{
    public interface IProgramInfo
    {
        /// <summary>
        /// Correspond to instrument ID used in the corresponding smd file.
        /// </summary>
        ushort ID { get; set; }

        /// <summary>
        /// The volume of the entire program.
        /// </summary>
        sbyte Volume { get; set; }

        /// <summary>
        /// The panning of the entire program. (0 = left, 64 = center, 127 = right)
        /// </summary>
        sbyte Panning { get; set; }

        /// <summary>
        /// Contains all of the LFO table entries.
        /// </summary>
        ILFOEntry[] LFOEntries { get; set; }

        /// <summary>
        /// Contains all of the Splits table entries.
        /// </summary>
        ISplitsEntry[] Splits { get; set; }

        /// <summary>
        /// In version 0x402 this array has 8 values, should be an empty array if it is not supported by current version.
        /// </summary>
        KeyGroupEntry[] KeyGroups { get; set; }
    }
}
