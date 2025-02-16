using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Area23.At.Framework.Library.CqrXs.CqrMsg
{


    /// <summary>
    /// Represtents a MsgContent
    /// </summary>
    [DataContract(Name = "MsgContent")]
    [Description("cqrxs.eu msgcontent")]
    public class MsgContent
    {
        protected internal Nullable<bool> _isMime;
        protected internal string _hash;
        protected internal string _message;
        protected internal string _rawMessage;

        public bool IsMime { get => IsMimeAttachment(); }

        public string Hash { get => VerificationHash(); }

        public string Message { get => _message; }

        public string RawMessage { get => _rawMessage; }


        #region ctor

        public MsgContent()
        {
            _message = string.Empty;
            _rawMessage = string.Empty;
            _hash = string.Empty;
        }

        public MsgContent(string msg)
        {
            _message = msg;            
        }

        public MsgContent(string msg, string hash)
        {
            _message = msg;
            _hash = hash;
        }

        #endregion ctor

        public MsgContent SetMsgContent(string plainMsg)
        {
            MsgContent msgContent = new MsgContent(plainMsg);
            _message = msgContent.Message;

            return (MsgContent)this;
        }

        public virtual bool IsMimeAttachment()
        {
            if (_isMime.HasValue)
                return _isMime.Value;

            _isMime = false;
            if (_message.StartsWith("Content-Type: ") && _message.Contains("Content-Verification: ") && _message.Contains("Content-Length: "))
            {
                string checkContentLength = _message.Substring(_message.IndexOf("Content-Length: ") + "Content-Length: ".Length);
                checkContentLength = checkContentLength.Substring(0, checkContentLength.IndexOf(";\n"));
                int contentLen = 0;
                if (!Int32.TryParse(checkContentLength, out contentLen))
                    contentLen = -1;

                _isMime = (contentLen > 0);
            }

            return _isMime.Value;
        }

        public virtual string ToJson()
        {
            string jsonText = JsonConvert.SerializeObject(this);
            return jsonText;
        }

        public virtual T FromJson<T>(string jsonText)
        {
            T t = JsonConvert.DeserializeObject<T>(jsonText);
            if (t != null && t is MsgContent mc)
            {
                this._hash = mc.Hash;
                this._message = mc.Message;
                this._rawMessage = mc.RawMessage;
            }
            return t;
        }
        

        public virtual string VerificationHash()
        {
            if (!string.IsNullOrEmpty(_hash))
                return _hash;

            _hash = (_message.Length > 8) ? _message.Substring(_message.Length - 8) : _message;
            if (IsMimeAttachment())
            {
                string verification = _message.Substring(_message.IndexOf("Content-Verification: ") + "Content-Verification: ".Length);
                _hash = _message.Substring(0, _message.IndexOf(";\n"));
            }

            if (_hash.Length > 6 && _message.Substring(_message.Length - 8).Equals(_hash, StringComparison.InvariantCulture))
                _message = _message.Substring(0, _message.Length - 8);

            return _hash;
        }

        public virtual MimeAttachment ToMimeAttachment()
        {
            if (!IsMimeAttachment())
                throw new InvalidCastException($"MsgContent Message={_message} isn't a mime attachment!");

            MimeAttachment mimeAttachment = MimeAttachment.GetBase64Attachment(_message);
            return mimeAttachment;
        }



        public override string ToString()
        {
            return _message.ToString();
        }

        public static MsgContent GetMessageContent(string plainMsg)
        {
            MsgContent msgContent = new MsgContent(plainMsg);
            return msgContent;
        }


    }

}
