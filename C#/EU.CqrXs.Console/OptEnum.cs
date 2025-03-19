using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EU.CqrXs.Console
{
    public enum OptEnum
    {
        Usage   = 0x0,
        InParam = 0x1,
        OutP    = 0x2,
        Zip     = 0x3,
        Unzip   = 0x4,
        Encode  = 0x5,
        Decode  = 0x6,        
        Crypt   = 0x7,
        Key     = 0x8,
        Decrypt = 0x9,
        HashSum = 0xa,
        Help    = 0xb
    }
}
