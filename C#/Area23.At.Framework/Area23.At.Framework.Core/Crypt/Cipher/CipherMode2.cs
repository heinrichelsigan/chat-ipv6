using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Security.Cryptography;
using System.Text;

namespace Area23.At.Framework.Core.Crypt.Cipher
{

    [Serializable]
    [DefaultValue("ECB")]
    public enum CipherMode2 : byte
    {
        CBC = 0x0,
        CCM = 0x1,
        CFB = 0x2,
        CTS = 0x3,
        EAX = 0x4,
        ECB = 0x5,
        GOFB = 0x6
    }


    /// <summary>
    /// <see cref="System.Security.Cryptography.CipherMode"/>
    /// </summary>
    [Serializable]
    public struct CiffreMode
    {
        public static CipherMode CMode { get; internal set; }

        public static CipherMode2 CMode2 { get; internal set; }

        public CiffreMode()
        {
            CMode = CipherMode.ECB;
            CMode2 = CipherMode2.ECB;
        }

        public CiffreMode(CipherMode2 cipherMode2)
        {
            CMode2 = cipherMode2;
            CMode = cipherMode2.ToCipherMode();
        }

    }

    public static partial class CipherModeExtensions
    {

        //public static CipherMode ToCipherMode(this CipherMode2 cipherMode2)
        //{
        //    return cipherMode2 switch
        //    {
        //        CipherMode2.CBC => CipherMode.CBC,
        //        CipherMode2.CFB => CipherMode.CFB,
        //        CipherMode2.CTS => CipherMode.CTS,
        //        CipherMode2.ECB => CipherMode.ECB,
        //        _ => throw new NotSupportedException($"CipherMode2 '{cipherMode2}' is not supported in System.Security.Cryptography.CipherMode"),
        //    };
        //}

        public static CipherMode ToCipherMode(this CipherMode2 mode)
        {
            return mode switch
            {
                CipherMode2.CBC => CipherMode.CBC,
                CipherMode2.CCM => CipherMode.CBC,
                CipherMode2.CFB => CipherMode.CFB,
                CipherMode2.CTS => CipherMode.CTS,
                CipherMode2.EAX => CipherMode.CBC,
                CipherMode2.ECB => CipherMode.ECB,
                CipherMode2.GOFB => CipherMode.CBC,
                _ => CipherMode.ECB,
            };
        }

        public static CipherMode2 FromCipherMode(this CipherMode mode)
        {
            return mode switch
            {
                CipherMode.CBC => CipherMode2.CBC,
                CipherMode.CFB => CipherMode2.CFB,
                CipherMode.CTS => CipherMode2.CTS,
                CipherMode.ECB => CipherMode2.ECB,
                _ => CipherMode2.ECB,
            };
        }

        public static CipherMode[] GetCipherModes()
        {
            List<CipherMode> list = new List<CipherMode>();
            foreach (string encName in Enum.GetNames(typeof(CipherMode)))
            {
                list.Add((CipherMode)Enum.Parse(typeof(CipherMode), encName));
            }

            return list.ToArray();
        }



        public static CipherMode2[] GetCipherModes2()
        {
            List<CipherMode2> list = new List<CipherMode2>();
            foreach (string encName in Enum.GetNames(typeof(CipherMode2)))
            {
                list.Add((CipherMode2)Enum.Parse(typeof(CipherMode2), encName));
            }

            return list.ToArray();
        }



        /// <summary>
        /// parses pipe semicolon separated pipe string to CipherList
        /// </summary>
        /// <param name="text">semicolon separated pipe string to CipherList </param>
        /// <returns><see cref="T:CipherMode"/> array of ciphers for the pipe</returns>
        public static CipherMode2 ParseText(string text)
        {
            CipherMode2 cipherMode = CipherMode2.ECB;
            List<CipherMode2> cipherList = new List<CipherMode2>();
            text = text ?? "";

            if (!Enum.TryParse<CipherMode2>(text, out cipherMode))
                cipherMode = CipherMode2.ECB;

            return cipherMode;
        }

    }

}
