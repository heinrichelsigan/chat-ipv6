﻿using Area23.At.Framework.Core.Crypt.Cipher.Symmetric;
using Area23.At.Framework.Core.Crypt.EnDeCoding;
using System.ComponentModel;

namespace Area23.At.Framework.Core.Crypt.Cipher
{

    /// <summary>
    /// CipherEnum maps BlockCipher algorithms <see cref="Org.BouncyCastle.Crypto.IBlockCipher"/>
    /// </summary>
    [DefaultValue("Aes")]
    public enum CipherEnum : byte
    {
        Aes = 0x0,

        BlowFish = 0x1,
        Camellia = 0x2,
        Cast6 = 0x3,
        Des3 = 0x4,
        Fish2 = 0x5,
        Fish3 = 0x6,
        Gost28147 = 0x7,
        Idea = 0x8,
        RC532 = 0x9,
        Seed = 0xa,
        SkipJack = 0xb,
        Serpent = 0xc,
        Tea = 0xd,
        XTea = 0xe,

        ZenMatrix = 0xf,


        Cast5 = 0x10,
        Rijndael = 0x11,
        Noekeon = 0x12,
        RC2 = 0x13,
        RC564 = 0x14,
        RC6 = 0x15,
        Tnepres = 0x16,
        Des = 0x17,
        Aria = 0x18,
        CamelliaLight = 0x19,
        Dstu7624 = 0x1a,
        SM4 = 0x1b,
        AesLight = 0x1c,
        ThreeFish256 = 0x1d,
        Rsa = 0x1e,

        ZenMatrix2 = 0x1f
    }

    /// <summary>
    /// Extensions methods for <see cref="CipherEnum"/>
    /// </summary>
    public static class CipherEnumExtensions
    {

        public static CipherEnum[] GetCipherTypes()
        {
            List<CipherEnum> list = new List<CipherEnum>();
            foreach (string encName in Enum.GetNames(typeof(CipherEnum)))
            {
                list.Add((CipherEnum)Enum.Parse(typeof(CipherEnum), encName));
            }

            return list.ToArray();
        }

        /// <summary>
        /// Extensions method for Enum <see cref="CipherEnum"/>
        /// gets a character for each Cipher Algorithm, that is used here
        /// </summary>
        /// <param name="cipher">this <see cref="CipherEnum"/> extension</param>
        /// <returns>a <see cref="char"/>, that is a short name for the encryption</returns>
        public static char GetCipherChar(this CipherEnum cipher)
        {
            switch (cipher)
            {
                case CipherEnum.Aes:
                case CipherEnum.Rijndael: return 'A';
                case CipherEnum.AesLight: return 'L';
                case CipherEnum.Aria: return 'a';

                case CipherEnum.BlowFish: return 'b';
                case CipherEnum.Fish2: return 'f';
                case CipherEnum.Fish3: return 'F';
                case CipherEnum.ThreeFish256: return '3';

                case CipherEnum.Camellia: return 'C';
                case CipherEnum.CamelliaLight: return 'l';
                case CipherEnum.Cast5: return 'c';
                case CipherEnum.Cast6: return '6';

                case CipherEnum.Des: return '$';
                case CipherEnum.Des3: return 'D';
                case CipherEnum.Dstu7624: return 'd';

                case CipherEnum.Gost28147: return 'g';
                case CipherEnum.Idea: return 'I';
                case CipherEnum.Noekeon: return 'N';

                case CipherEnum.RC2: return '2';
                case CipherEnum.RC532: return '5';
                case CipherEnum.RC564: return 'R';
                case CipherEnum.RC6: return 'r';
                case CipherEnum.Rsa: return '%';

                case CipherEnum.Seed: return 's';
                case CipherEnum.Serpent: return 'S';
                case CipherEnum.SM4: return '4';
                case CipherEnum.SkipJack: return 'J';

                case CipherEnum.Tea: return 't';
                case CipherEnum.Tnepres: return 'T';
                case CipherEnum.XTea: return 'X';

                case CipherEnum.ZenMatrix: return 'z';
                case CipherEnum.ZenMatrix2: return 'Z';

                default: break;
            }

            return 'A';
        }


        public static string PrintChipherTypes()
        {
            string s = "";
            foreach (CipherEnum cipher in GetCipherTypes())
            {
                s += "\t" + cipher.GetCipherChar() + "\t" + cipher.ToString() + "\t" + cipher.ToString("x:2");
            }

            return s;
        }


        public static CipherEnum FromSymmCipherEnum(Symmetric.SymmCipherEnum symmCipherEnum)
        {
            return symmCipherEnum.ToCipherEnum();
        }
    }

}
