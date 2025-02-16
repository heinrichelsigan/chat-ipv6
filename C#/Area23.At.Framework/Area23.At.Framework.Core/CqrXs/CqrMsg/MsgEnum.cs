using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Area23.At.Framework.Core.CqrXs.CqrMsg
{
    public enum MsgEnum
    {
        None = 0,
        RawWithHashAtEnd = 1,
        JsonSerialized = 2,
        JsonDeserialized = 3
    }
}
