using Area23.At.Framework.Core.Crypt.EnDeCoding;
using Area23.At.Framework.Core.Zfx;
using System.ComponentModel;
using System.Security.Policy;
using System.Text;

namespace Area23.At.Framework.Core.Crypt.Hash
{

    /// <summary>
    /// KeyHash 
    /// </summary>
    [DefaultValue(KeyHash.Hex)]
    public enum KeyHash : short
    {
        Hex =           0x9,
        OpenBSDCrypt =  0x1,
        BCrypt =        0x2,
        SCrypt =        0x3,
        MD5 =           0x4,
        Sha1 =          0x5,
        Sha256 =        0x6,
        Sha384 =        0x7,
        Sha512 =        0x8
    }

    public static class KeyHash_Extensions
    {
        private static readonly KeyHash[] keyHashes = { KeyHash.Hex, KeyHash.OpenBSDCrypt, KeyHash.BCrypt, KeyHash.SCrypt, KeyHash.MD5, KeyHash.Sha1, KeyHash.Sha256, KeyHash.Sha384, KeyHash.Sha512 };

        public static KeyHash GetKeyHashFromValue(short kValue)
        {
            foreach (KeyHash kHash in keyHashes)
            {
                if ((short)kHash == kValue)
                    return kHash;
            }
            return KeyHash.Hex;
        }

        public static string Hash(this KeyHash hash, string stringToHash)
        {
            switch (hash)
            {
                case KeyHash.SCrypt:
                    return Hex16.ToHex16(PasswdCrypt.SCrypt(stringToHash));
                case KeyHash.BCrypt:
                    return Hex16.ToHex16(PasswdCrypt.BCrypt(stringToHash));
                case KeyHash.OpenBSDCrypt:
                    return PasswdCrypt.BSDCrypt(stringToHash);
                case KeyHash.MD5:
                    return MD5Sum.HashString(stringToHash, "");
                case KeyHash.Sha1:
                    return Sha1.HashString(stringToHash);
                case KeyHash.Sha256:
                    return Sha256Sum.HashString(stringToHash, "");
                case KeyHash.Sha384:
                    return Sha384.HashString(stringToHash);
                case KeyHash.Sha512:
                    return Sha512Sum.HashString(stringToHash);
                case KeyHash.Hex:
                default:
                    return Hex16.ToHex16(Encoding.UTF8.GetBytes(stringToHash));
            }
        }

    }

}
