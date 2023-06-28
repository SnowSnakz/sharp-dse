using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpDSE
{
    public interface IBinaryReadable
    {
        void Read(BinaryReader reader);
    }
}
