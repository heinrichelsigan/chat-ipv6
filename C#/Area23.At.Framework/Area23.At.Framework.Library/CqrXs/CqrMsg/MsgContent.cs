﻿using Area23.At.Framework.Library.Crypt.Cipher.Symmetric;
using Area23.At.Framework.Library.Util;
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
    /// [DataContract(Name = "MsgContent")]
    /// </summary>    
    [JsonObject]
    [Serializable]
    public class MsgContent
    {
        protected internal Nullable<bool> _isMime;
        internal string _hash;
        internal string _message;
        protected internal string _rawMessage;

        public MsgEnum MsgType { get; protected internal set; }

        public bool IsMime { get => IsMimeAttachment(); }

        public string Hash { get => _hash; }

        public string Message
        {
            get
            {
                if (_message.EndsWith("\n" + _hash + "\0"))
                    _message = _message.Substring(0, _message.LastIndexOf("\n" + _hash + "\0"));
                else if (_message.EndsWith("\n" + _hash))
                    _message = _message.Substring(0, _message.LastIndexOf("\n" + _hash));
                else if (_message.EndsWith(_hash + "\0"))
                    _message = _message.Substring(0, _message.LastIndexOf(_hash + "\0"));
                // if (_message.EndsWith(_hash))
                //     _message = _message.Substring(0, _message.IndexOf(_hash));

                return _message;
            }
        }



        public string RawMessage { get => _rawMessage; }


        #region ctor

        public MsgContent()
        {
            MsgType = MsgEnum.None;
            _message = string.Empty;
            _rawMessage = string.Empty;
            _hash = string.Empty;
        }

        public MsgContent(string msg, MsgEnum msgArt = MsgEnum.None)
        {
            switch (msgArt)
            {
                case MsgEnum.JsonSerialized:
                    MsgType = MsgEnum.JsonSerialized;
                    MsgContent c = this.FromJson<MsgContent>(msg);
                    c._rawMessage = msg;
                    break;
                case MsgEnum.JsonDeserialized:
                    MsgType = MsgEnum.JsonDeserialized;
                    _rawMessage = msg;
                    _hash = VerificationHash(out _message);
                    _rawMessage = JsonConvert.SerializeObject(this, Formatting.Indented);
                    break;
                case MsgEnum.None:
                case MsgEnum.RawWithHashAtEnd:
                default:
                    MsgType = MsgEnum.RawWithHashAtEnd;
                    _message = msg;
                    _rawMessage = msg;
                    _hash = VerificationHash(out _message);
                    break;
            }

        }

        public MsgContent(string msg, string hash, MsgEnum msgArt = MsgEnum.RawWithHashAtEnd)
        {
            MsgType = msgArt;
            if (msgArt == MsgEnum.JsonSerialized || msgArt == MsgEnum.JsonDeserialized)
            {
                _message = msg;
                _hash = hash;
                _rawMessage = this.ToJson();
            }
            if (msgArt == MsgEnum.RawWithHashAtEnd || msgArt == MsgEnum.None)
            {
                _hash = hash;
                if (msg.Contains(hash) && msg.IndexOf(hash) > (msg.Length - 10))
                {
                    _rawMessage = msg;
                    _message = _rawMessage.Substring(0, _rawMessage.Length - _hash.Length);
                }
                else
                {
                    _message = msg;
                    _rawMessage = _message + "\n" + hash + "\0";
                }                
            }
        }

        #endregion ctor

        public MsgContent SetMsgContent(string plainMsg)
        {
            MsgContent msgContent = new MsgContent(plainMsg);
            _message = msgContent.Message;
            _rawMessage = msgContent.RawMessage;
            _hash = msgContent._hash;

            return (MsgContent)this;
        }

        public virtual bool IsMimeAttachment()
        {
            if (_isMime.HasValue)
                return _isMime.Value;

            _isMime = false;
            if (_rawMessage.StartsWith("Content-Type: ") && _rawMessage.Contains("Content-Verification: ") && _rawMessage.Contains("Content-Length: "))
            {
                string checkContentLength = _rawMessage.GetSubStringByPattern("Content-Length: ", true, "", ";\n", false);
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

        public virtual TType FromJson<TType>(string jsonText)
        {
            TType t = JsonConvert.DeserializeObject<TType>(jsonText);
            if (t != null && t is MsgContent mc)
            {
                this._hash = mc.Hash;
                this._message = mc.Message;
                this._rawMessage = mc.RawMessage;
            }
            return t;
        }


        public virtual string VerificationHash(out string msg)
        {
            msg = _message;
            if (!string.IsNullOrEmpty(_hash))
            {
                return _hash;
            }

            if (_rawMessage.Length > 8)
            {
                // if (_message.Contains('\n') && _message.LastIndexOf('\n') < _message.Length)
                string tmp = _rawMessage.Substring(_rawMessage.Length - 9);
                if (tmp.Contains('\n') && tmp.IndexOf('\n') < 9)
                    _hash = tmp.Substring(tmp.LastIndexOf('\n') + 1);
            }
            else
            {
                _hash = _rawMessage;
            }

            if (IsMimeAttachment())
            {
                _hash = _rawMessage.GetSubStringByPattern("Content-Verification: ", true, "", ";\n", false);
                // _hash = _rawMessage.Substring(0, _rawMessage.IndexOf(";\n"));
            }

            try
            {
                if (_hash.Length > 4 && _rawMessage.Substring(_rawMessage.Length - _hash.Length).Equals(_hash, StringComparison.InvariantCulture))
                    msg = _rawMessage.Substring(0, _rawMessage.Length - _hash.Length);
            } catch (Exception exHash)
            {
                Area23Log.LogStatic("Couldn't verify hash, it's most an encryption bug in Aes pipe cycle: " + exHash.Message);
                throw new FishRequiresAesEngineException("hash verification isn't possible, because of 1st bug, wrong crypto engine", exHash);
            }

            return _hash ?? string.Empty;
        }

        public virtual MimeAttachment ToMimeAttachment()
        {
            if (!IsMimeAttachment())
                throw new InvalidCastException($"MsgContent Message={_rawMessage} isn't a mime attachment!");

            MimeAttachment mimeAttachment = MimeAttachment.GetBase64Attachment(_rawMessage);
            return mimeAttachment;
        }



        public override string ToString()
        {
            return _message.ToString();
        }

        public static MsgContent SetMessageContent(string plainMsg)
        {
            MsgContent msgContent = new MsgContent(plainMsg);
            return msgContent;
        }

    }


}
