using System.ComponentModel;

namespace Area23.At.Framework.Library.Zfx
{
    [DefaultValue(None)]
    public enum ZipType
    {
        None = 0x0,
        Zip = 0x1,
        GZip = 0x2,
        BZip2 = 0x3,
        Z7 = 0x4
    }
}
