using Area23.At.Framework.Core.Util;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlTypes;
using System.Linq;
using System.Net.Mail;
using System.Runtime.InteropServices.JavaScript;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows.Controls;
using static System.Net.Mime.MediaTypeNames;

namespace Area23.At.Framework.Core.CqrXs.CqrMsg
{


    /// <summary>
    /// Represtents a MsgContent
    /// </summary>
    [JsonObject]
    [Serializable]
    public class MsgContent : ICqrMessagable
    {
        public string _hash;
        public string _message;
        public string _rawMessage;

        public MsgEnum MsgType { get; protected internal set; }

        // public bool IsMime { get => IsMimeAttachment(); }

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
                case MsgEnum.None:
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
                _rawMessage = Ext.SerializeToXml<MsgContent>(this);

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
                this._message = mc.Message;
                this._rawMessage = mc.RawMessage;
            }
            return t;
        }

        public virtual string ToXml() => Ext.SerializeToXml<MsgContent>(this);
       
        public virtual T? FromXml<T>(string xmlText)
        {
            T? cqrT = Ext.DeserializeFromXml<T>(xmlText);
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

            if (_hash.Length > 4 && _rawMessage.Substring(_rawMessage.Length - _hash.Length).Equals(_hash, StringComparison.InvariantCulture))
                msg = _rawMessage.Substring(0, _rawMessage.Length - _hash.Length);

            return _hash ?? string.Empty;
        }

        public override string ToString()
        {
            return $"\"_message\": {_message}\n" +
                $"\"_hash\": {_hash}\n" +
                $"\"_rawMessage\": {_rawMessage}\n" +
                $"\"MsgType\": {this.MsgType}\n";

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
                Area23Log.LogStatic(exCqrFile);
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
                        //        Ext.DeserializeFromXml<ClientSrvMsg<FullSrvMsg<string>, FullSrvMsg<string>>>(serString);                            
                        //}
                        if (serString.Contains("Sender") && serString.Contains("Recipients") && serString.Contains("TContent"))
                        {
                            outType = typeof(FullSrvMsg<string>);
                            return (FullSrvMsg<string>)Ext.DeserializeFromXml<FullSrvMsg<string>>(serString);
                        }
                        if (serString.Contains("CqrFileName") && serString.Contains("Base64Type"))
                        {
                            outType = typeof(CqrFile);
                            return (CqrFile)Ext.DeserializeFromXml<CqrFile>(serString);
                        }
                        if (serString.Contains("ImageFileName") && serString.Contains("ImageMimeType"))
                        {
                            outType = typeof(CqrImage);
                            return (CqrImage)Ext.DeserializeFromXml<CqrImage>(serString);
                        }
                        if (serString.Contains("ContactId") && serString.Contains("Cuid") && serString.Contains("Email"))
                        {
                            outType = typeof(CqrContact);
                            return (CqrContact)Ext.DeserializeFromXml<CqrContact>(serString);
                        }

                        outType = typeof(MsgContent);
                        return (MsgContent)Ext.DeserializeFromXml<MsgContent>(serString);
                    }
                    break;
                case MsgEnum.RawWithHashAtEnd:
                case MsgEnum.None:
                default: throw new NotImplementedException("GetMsgContentType(...): case MsgEnum.RawWithHashAtEnd and MsgEnum.None not implemented");
            }

            return null;
        }


        public static MsgContent SetMessageContent(string plainMsg)
        {
            MsgContent msgContent = new MsgContent(plainMsg);
            return msgContent;
        }

        #endregion static members

    }


}
