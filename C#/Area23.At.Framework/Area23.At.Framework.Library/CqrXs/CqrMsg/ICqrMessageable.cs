using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Area23.At.Framework.Core.CqrXs.CqrMsg
{
    public interface ICqrMessagable
    {
        public MsgEnum MsgType { get; }
        public string Hash { get; }
        public string Message { get; }
        public string RawMessage { get; }

        public string ToJson();
        public T? FromJson<T>(string jsonText);
        public string ToXml();
        public T? FromXml<T>(string xmlText);
      
    }
}
