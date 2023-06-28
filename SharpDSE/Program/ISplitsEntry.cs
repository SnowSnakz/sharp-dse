using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpDSE.Program
{
    public interface ISplitsEntry
    {
        ushort ID { get; set; }
        byte BendRange { get; set; }
        sbyte LowestKey { get; set; }
        sbyte HighestKey { get; set; }
        sbyte LowestVelocity { get; set; }
        sbyte HighestVelocity { get; set; }
        ushort SampleID { get; set; }
        byte FineTuneOverride { get; set; }
        sbyte CoarseTuneOverride { get; set; }
        byte RootKeyOverride { get; set; }
        sbyte KeyTransposeOverride { get; set; }
        sbyte VolumeOverride { get; set; }
        sbyte PanningOverride { get; set; }
    }
}
