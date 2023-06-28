using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpDSE
{
    public interface IBinaryWritable
    {
        void Write(BinaryWriter writer);
    }
}
