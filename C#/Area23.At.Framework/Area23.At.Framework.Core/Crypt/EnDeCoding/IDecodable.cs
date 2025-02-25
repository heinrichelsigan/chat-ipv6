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

        public virtual string EnCode(byte[] inBytes)
        {
            return Decodable.Encode(inBytes);
        }

        public virtual byte[] DeCode(string encodedString) 
        {
            return Decodable.Decode(encodedString);
        }

        public abstract byte[] Decode(string encodedString);

        public abstract string Encode(byte[] inBytes);

        public abstract bool IsValid(string encodedString);

        public bool Validate(string encodedString)
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
