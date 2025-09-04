﻿namespace Area23.At.Framework.Core.Crypt.EnDeCoding
{
    /// <summary>
    /// Base64 mime standard encoding
    /// </summary>
    public class Base64 : IDecodable
    {

        public const char ZERO_WIDTH_NO_BREAK_SPACE = (char)0xfeff;
        public static readonly char[] SPECIAL_CHAR_ARRAY = { ZERO_WIDTH_NO_BREAK_SPACE, ' ', '\t', '\r', '\n' };
        public static readonly string SPECIAL_CHARS = new string(SPECIAL_CHAR_ARRAY);
        public const string VALID_CHARS = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz+/=";

        #region common interface, interfaces for static members appear in C# 7.3 or later

        public IDecodable Decodable => this;

        public static HashSet<char>? ValidCharList { get; private set; } = new HashSet<char>(VALID_CHARS.ToCharArray());        

        /// <summary>
        /// Encodes byte[] to valid encode formatted string
        /// </summary>
        /// <param name="inBytes">byte array to encode</param>
        /// <returns>encoded string</returns>
        public string Encode(byte[] inBytes) => Base64.ToBase64(inBytes);        

        /// <summary>
        /// Decodes an encoded string to byte[]
        /// </summary>
        /// <param name="encodedString">encoded string</param>
        /// <returns>byte array</returns>
        public byte[] Decode(string encodedString) => Base64.FromBase64(encodedString);

        public bool IsValid(string encodedStr) => Base64.IsValidBase64(encodedStr, out _);

        public bool IsValidShowError(string encodedString, out string error) => Base64.IsValidBase64(encodedString, out error);
        

        #endregion common interface, interfaces for static members appear in C# 7.3 or later


        public static string ToBase64(byte[] inBytes)
        {
            string os = Convert.ToBase64String(inBytes, 0, inBytes.Length, Base64FormattingOptions.None);
            return os;
        }

        public static byte[] FromBase64(string inString)
        {
            byte[] outBytes = Convert.FromBase64String(inString);
            return outBytes;
        }

       
        public static bool IsValidBase64(string inString, out string error)
        {
            bool valid = true;
            error = "";
            foreach (char ch in inString)
            {
                if (!ValidCharList.Contains(ch))
                {
                    error += ch;
                    valid = false;
                }
            }
            return valid;
        }

    }

}