using Area23.At.Framework.Library;
using Area23.At.Framework.Library.Util;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows.Controls;
using static System.Net.Mime.MediaTypeNames;

namespace Area23.At.Framework.Library.CqrXs.CqrMsg
{


    /// <summary>
    /// Represtents a MsgContent
    /// </summary>
    [Serializable]
    public class MsgContent : ICqrMessagable
    {
        public string _hash;
        public string _message;
        public string _rawMessage;

        public MsgEnum MsgType { get; protected internal set; }

        // public bool IsMime { get => IsMimeAttachment(); }

        public string Hash { get => _hash; }

        //TODO:
        [Obsolete("TODO: remove it with hash at end", false)]
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


        /// <summary>
        /// this constructor requires a serialized or rawstring in msg
        /// </summary>
        /// <param name="serializedString">serialized string</param>
        /// <param name="msgArt">Serialization type</param>
        public MsgContent(string serializedString, MsgEnum msgArt = MsgEnum.None)
        {
            switch (msgArt)
            {
                case MsgEnum.Json:                  
                    MsgType = MsgEnum.Json;
                    MsgContent? c = GetMsgContentType(serializedString, out Type cqrType, MsgEnum.Json);
                    if (c != null)
                    {
                        _rawMessage = c._rawMessage;
                        _hash = c._hash;
                        _message = c._message;
                    }
                    break;
                case MsgEnum.Xml:
                    MsgType = MsgEnum.Xml;
                    MsgContent? cXml = GetMsgContentType(serializedString, out Type cqType, msgArt);
                    if (cXml != null)
                    {
                        _rawMessage = cXml._rawMessage;
                        _hash = cXml._hash;
                        _message = cXml._message;
                    }
                    break;
                case MsgEnum.None: //TODO
                    throw new NotImplementedException("TODO: implement reverse Reflection deserialization");

                case MsgEnum.RawWithHashAtEnd:
                default:
                    MsgType = MsgEnum.RawWithHashAtEnd;
                    _message = serializedString;
                    _rawMessage = serializedString;
                    _hash = VerificationHash(out _message);
                    break;                
            }
            
        }

        /// <summary>
        /// this ctor requires a plainstring and serialize it in _rawMessage
        /// </summary>
        /// <param name="plainTextMsg">plain text message</param>
        /// <param name="hash"></param>
        /// <param name="msgArt"></param>
        public MsgContent(string plainTextMsg, string hash, MsgEnum msgArt = MsgEnum.RawWithHashAtEnd)
        {
            MsgType = msgArt;
            if (msgArt == MsgEnum.Json)
            {
                _message = plainTextMsg;
                _hash = hash;
                _rawMessage = this.ToJson();
            }
            if (msgArt == MsgEnum.Xml)
            {
                _message = plainTextMsg;
                _hash = hash;
                _rawMessage = Utils.SerializeToXml<MsgContent>(this);

            }
            if (msgArt == MsgEnum.RawWithHashAtEnd)
            {
                _hash = hash;
                if (plainTextMsg.Contains(hash) && plainTextMsg.IndexOf(hash) > (plainTextMsg.Length - 10))
                {
                    _rawMessage = plainTextMsg;
                    _message = _rawMessage.Substring(0, _rawMessage.Length - _hash.Length);
                }
                else
                {
                    _message = plainTextMsg;
                    _rawMessage = _message + "\n" + hash + "\0";
                }
            }
            if (msgArt == MsgEnum.None)
            {
                _hash = hash;
                _message = plainTextMsg;
                _rawMessage = this.ToString();
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

        public virtual string ToJson() => Newtonsoft.Json.JsonConvert.SerializeObject(this);

        public virtual T? FromJson<T>(string jsonText)
        {
            T? t = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(jsonText);
            if (t != null && t is MsgContent mc)
            {
                this._hash = mc.Hash;
                this._message = mc._message;
                this._rawMessage = mc.RawMessage;
            }
            return t;
        }

        public virtual string ToXml() => Utils.SerializeToXml<MsgContent>(this);
       
        public virtual T? FromXml<T>(string xmlText)
        {
            T? cqrT = Utils.DeserializeFromXml<T>(xmlText);
            if (cqrT is MsgContent mc)
            {
                this._hash = mc._hash;
                this._rawMessage = mc._rawMessage;
                this._message = mc._message;
            }
            
            return cqrT;
        }

        public virtual string VerificationHash(out string msg)
        {
            msg = _message;
            if (!string.IsNullOrEmpty(_hash))
            {                
                return _hash;
            }

            if (IsCqrFile())
            {
                CqrFile? cqFile = ToCqrFile();
                // CqrFile? cfile = IsTo<CqrFile>(out CqrFile? t);
                if (cqFile != null && !string.IsNullOrEmpty(cqFile.Hash))
                {
                    _hash = cqFile._hash;
                    if (!string.IsNullOrEmpty(cqFile._message))
                        msg = cqFile._message;

                    return _hash;
                }

            }

            if (_rawMessage.Length > 9) 
            {
                // if (_message.Contains('\n') && _message.LastIndexOf('\n') < _message.Length)
                string tmp = _rawMessage.Substring(_rawMessage.Length - 10);
                if (tmp.Contains('\n') && tmp.IndexOf('\n') < 9)
                {
                    _hash = tmp.Substring(tmp.LastIndexOf('\n') + 1);
                    if (_hash.Contains("\0"))
                        _hash = _hash.Substring(0, _hash.LastIndexOf("\0"));
                }
            }
            else
            {
                _hash = _rawMessage;
            }        


            if (_hash.Length > 4 && _rawMessage.Substring(_rawMessage.Length - _hash.Length).Equals(_hash, StringComparison.InvariantCulture))
                msg = _rawMessage.Substring(0, _rawMessage.Length - _hash.Length);

            return _hash ?? string.Empty;
        }

        public override string ToString()
        {
            string s = this.GetType().ToString() + "\n";
            var fields = Static.Utils.GetAllFields(this.GetType());
            foreach (var field in fields)           
                s += field.Name + " \t\"" + field.GetRawConstantValue()?.ToString() + "\"\n";            
            var props = Static.Utils.GetAllProperties(this.GetType());
            foreach (var prp in props)
                s += prp.Name + " \t\"" + prp.GetRawConstantValue()?.ToString() + "\"\n";

            return s;
        }


        public virtual bool IsCqrFile()
        {
            if (this is CqrFile cf && string.IsNullOrEmpty(cf.CqrFileName) && cf.Data != null)
                return true;

            CqrFile? cq = null;
            try
            {
                cq = JsonConvert.DeserializeObject<CqrFile>(_rawMessage);
                if (cq != null && !string.IsNullOrEmpty(cq.CqrFileName) && cq.Data != null)
                    return true;
            }
            catch (Exception exCqrFile)
            {
                SLog.Log(exCqrFile);
            }

            return false;
        }

        public virtual CqrFile? ToCqrFile()
        {
            if (this is CqrFile cf && string.IsNullOrEmpty(cf.CqrFileName) && cf.Data != null)
                return cf;
            
            if (_rawMessage.IsValidJson() && _rawMessage.Contains("CqrFileName") && _rawMessage.Contains("Base64Type"))
            {
                return (CqrFile)JsonConvert.DeserializeObject<CqrFile>(_rawMessage);
            }

            return null;
        }


        #region static members
       
        public static MsgContent GetMsgContentType(string serString, out Type outType, MsgEnum msgType = MsgEnum.None)
        {
            outType = typeof(MsgContent);
            switch (msgType)
            {
                case MsgEnum.Json:
                    if (serString.IsValidJson())
                    {
                        //if (serString.Contains("ServerMsg") && serString.Contains("ClientMsg") && serString.Contains("ServerMsgString") && serString.Contains("ClientMsgString"))
                        //{
                        //    outType = typeof(ClientSrvMsg<FullSrvMsg<string>, FullSrvMsg<string>>);
                        //    return (ClientSrvMsg<FullSrvMsg<string>, FullSrvMsg<string>>)
                        //        JsonConvert.DeserializeObject<ClientSrvMsg<FullSrvMsg<string>, FullSrvMsg<string>>>(serString);
                        //}
                        if (serString.Contains("Sender") && serString.Contains("Recipients") && serString.Contains("TContent"))
                        {
                            outType = typeof(FullSrvMsg<string>);
                            return (FullSrvMsg<string>)JsonConvert.DeserializeObject<FullSrvMsg<string>>(serString);
                        }

                        if (serString.Contains("CqrFileName") && serString.Contains("Base64Type"))
                        {
                            outType = typeof(CqrFile);
                            CqrFile cqrFile = (CqrFile)JsonConvert.DeserializeObject<CqrFile>(serString);
                            cqrFile._rawMessage = serString;
                            return cqrFile;
                        }
                        if (serString.Contains("ImageFileName") && serString.Contains("ImageMimeType"))
                        {
                            outType = typeof(CqrImage);
                            return (CqrImage)JsonConvert.DeserializeObject<CqrImage>(serString);
                        }
                        if (serString.Contains("ContactId") && serString.Contains("Cuid") && serString.Contains("Email"))
                        {
                            outType = typeof(CqrContact);
                            return (CqrContact)JsonConvert.DeserializeObject<CqrContact>(serString);
                        }

                        outType = typeof(MsgContent);
                        return (MsgContent)JsonConvert.DeserializeObject<MsgContent>(serString);
                    }
                    break;
                case MsgEnum.Xml:
                    if (serString.IsValidXml())
                    {
                        //if (serString.Contains("ServerMsg") && serString.Contains("ClientMsg") && serString.Contains("ServerMsgString") && serString.Contains("ClientMsgString"))
                        //{
                        //    outType = typeof(ClientSrvMsg<FullSrvMsg<string>, FullSrvMsg<string>>);
                        //    return (ClientSrvMsg<FullSrvMsg<string>, FullSrvMsg<string>>)
                        //        Utils.DeserializeFromXml<ClientSrvMsg<FullSrvMsg<string>, FullSrvMsg<string>>>(serString);                            
                        //}
                        if (serString.Contains("Sender") && serString.Contains("Recipients") && serString.Contains("TContent"))
                        {
                            outType = typeof(FullSrvMsg<string>);
                            return (FullSrvMsg<string>)Utils.DeserializeFromXml<FullSrvMsg<string>>(serString);
                        }
                        if (serString.Contains("CqrFileName") && serString.Contains("Base64Type"))
                        {
                            outType = typeof(CqrFile);
                            return (CqrFile)Utils.DeserializeFromXml<CqrFile>(serString);
                        }
                        if (serString.Contains("ImageFileName") && serString.Contains("ImageMimeType"))
                        {
                            outType = typeof(CqrImage);
                            return (CqrImage)Utils.DeserializeFromXml<CqrImage>(serString);
                        }
                        if (serString.Contains("ContactId") && serString.Contains("Cuid") && serString.Contains("Email"))
                        {
                            outType = typeof(CqrContact);
                            return (CqrContact)Utils.DeserializeFromXml<CqrContact>(serString);
                        }

                        outType = typeof(MsgContent);
                        return (MsgContent)Utils.DeserializeFromXml<MsgContent>(serString);
                    }
                    break;
                case MsgEnum.RawWithHashAtEnd:
                case MsgEnum.None:
                default: throw new NotImplementedException("GetMsgContentType(...): case MsgEnum.RawWithHashAtEnd and MsgEnum.None not implemented");
            }

            return null;
        }

        #endregion static members

    }


}

/*

using Area23.At.Framework.Library.Crypt.Cipher.Symmetric;
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

*/

