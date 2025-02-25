using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Area23.At.Framework.Core.Crypt.EnDeCoding
{
 
    public interface IDecodable
    {
        IDecodable Decodable { get; }

        public static HashSet<char>? ValidCharList { get; private set; } 

        public string Encode(byte[] inBytes)
        {
            return Decodable.Encode(inBytes);
        }

        public byte[] Decode(string encodedString)
        {
            return Decodable.Decode(encodedString);
        }

        public abstract static byte[] DeCode(string encodedString);


        public abstract static string EnCode(byte[] inBytes);

        public bool IsValid(string encodedString)
        {
            if (ValidCharList != null)
            {
                foreach (char ch in encodedString)
                {
                    if (!ValidCharList.Contains(ch))
                        return false;
                }
            }
            return true;
        }

    }
}
