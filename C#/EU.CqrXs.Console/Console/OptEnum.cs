using Area23.At.Framework.Core.Crypt.Cipher;
using Area23.At.Framework.Core.Crypt.EnDeCoding;
using Area23.At.Framework.Core.Crypt.Hash;
using System;
using System.Collections.Generic;
using System.Text;

namespace EU.CqrXs.Console
{

    /// <summary>
    /// OptEnum different option types
    /// </summary>
    public enum OptEnum
    {
        Usage = 0x0,
        InParam = 0x1,
        Key = 0x2,
        Hash = 0x3,
        Zip = 0x4,
        CipherAlgos = 0x5,
        Encode = 0x6,
        OutP = 0x7,
        DeCrypt = 0x8,
        SymmCipher = 0x9,
        Mode = 0xa,
        Verbose = 0xe,
        Help = 0xf
    }

    public static class OptEnumExtensions
    {

        /// <summary>
        /// Gets an option by argument
        /// </summary>
        /// <param name="argument">cmd line argument</param>
        /// <param name="optEnum"><see cref="OptEnum">OptEnum cmd arg option enum</see></param>
        /// <returns>optArg</returns>
        public static string GetOption(this string argument, out OptEnum optEnum)
        {
            string optArg = "";
            if (string.IsNullOrEmpty(argument) || argument.Length < 2 || argument[0] != '-')
            {
                optEnum = OptEnum.Usage;
                if (argument[0] != '/')
                    return optArg;
            }
            optArg = argument;
            string arg = argument.TrimStart("-/".ToCharArray());

            if (arg.Contains("="))
                // optArg = arg.GetSubStringByPattern("=", true, "", " ", true, StringComparison.CurrentCultureIgnoreCase);
                optArg = arg.Split("=".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)[1];

            switch (arg[0])
            {
                case 'I':
                case 'i':
                    optEnum = OptEnum.InParam;
                    return optArg;
                case 'O':
                case 'o':
                    optEnum = OptEnum.OutP;
                    return optArg;
                case 'Z':
                case 'z':
                    optEnum = OptEnum.Zip;
                    return optArg;
                case 'E':
                case 'e':
                    optEnum = OptEnum.Encode;
                    return optArg;
                case 'C':
                case 'c':
                    optEnum = OptEnum.CipherAlgos;
                    return optArg;
                case 'k':
                case 'K':
                    optEnum = OptEnum.Key;
                    return optArg;
                case 'm':
                case 'M':
                    optEnum = OptEnum.Mode;
                    return optArg;
                case 'h':
                case 'H':
                    optEnum = OptEnum.Hash;
                    return optArg;
                case 'S':
                    optEnum = OptEnum.SymmCipher;
                    return optArg;
                case 'D':
                    optEnum = OptEnum.DeCrypt;
                    return optArg;
                case 'v':
                case 'V':
                    optEnum = OptEnum.Verbose;
                    return optArg;
                case 'g':
                case 'G':
                case '?':
                    optEnum = OptEnum.Help;
                    optArg = "";
                    break;
                default:
                    optEnum = OptEnum.Usage;
                    optArg = $"unrecognized option: {argument}.";
                    break;
            }

            return optArg;
        }

    }
}
