using Area23.At.Framework.Core.Crypt.Cipher;
using Area23.At.Framework.Core.Crypt.Cipher.Symmetric;
using Area23.At.Framework.Core.Crypt.EnDeCoding;
using Area23.At.Framework.Core.Crypt.Hash;
using Area23.At.Framework.Core.Static;
using Area23.At.Framework.Core.Util;
using Area23.At.Framework.Core.Zfx;
using Newtonsoft.Json;
using System.Text;

namespace Area23.At.Framework.Core.Cqr.Msg
{

    /// <summary>
    /// CMsg is basic message of encryptable / decryptable entities
    /// TODO: need to refactor soonly
    /// </summary>
    [Serializable]
    public class CMsg : IMsgAble
    {

        #region properties

        public SerType Cerializer { get; set; }

        public CipherPipe CryptPipe { get; set; } = new CipherPipe();

        public EncodingType EnCodingType { get => CryptPipe.EncodeType; set => CryptPipe.EncodeType = value; }

        public string Message { get; set; }

        [JsonIgnore]
        public virtual string SerializedMsg
        {
            get => Cerialize();
        }

        public string Hash { get; set; }

        public string Md5Hash { get; set; }

        [JsonIgnore]
        protected internal byte[] CBytes { get; set; }

        #endregion properties

        #region ctor

        /// <summary>
        /// Parameterless constructor CMsg
        /// </summary>
        public CMsg()
        {
            Cerializer = SerType.Json;
            Message = string.Empty;
            CryptPipe = new CipherPipe();
            // SerializedMsg = string.Empty;
            Hash = string.Empty;
            Md5Hash = string.Empty;
            CBytes = new byte[0];
            EnCodingType = EncodingType.Base64;

        }


        /// <summary>
        /// this constructor requires a serialized or rawstring in msg
        /// </summary>
        /// <param name="serializedString">serialized string</param>
        /// <param name="msgArt">Serialization type</param>
        public CMsg(string serializedString, SerType msgArt = SerType.None)
        {
            Md5Hash = MD5Sum.HashString(serializedString);
            Message = serializedString;
            // SerializedMsg = serializedString;
            CBytes = new byte[0];

            string _message = Message;
            Message = _message;

            switch (msgArt)
            {
                case SerType.Json:
                    Cerializer = SerType.Json;
                    CMsg cjson = GetMsgContentType(serializedString, out Type cqrType, SerType.Json);
                    if (cjson != null)
                    {
                        cjson.Cerializer = SerType.Json;
                        CloneCopy(cjson, this);
                    }
                    break;
                case SerType.Xml:
                    Cerializer = SerType.Xml;
                    CMsg cXml = GetMsgContentType(serializedString, out Type cqType, msgArt);
                    if (cXml != null)
                    {
                        cXml.Cerializer = SerType.Xml;
                        CloneCopy(cXml, this);
                    }
                    break;
                case SerType.None: //TODO
                    throw new NotImplementedException("TODO: implement reverse Reflection deserialization");

                case SerType.Raw:
                default:
                    Cerializer = SerType.Raw;
                    Message = serializedString;
                    // SerializedMsg = serializedString;

                    _message = Message;
                    Message = _message;

                    Md5Hash = MD5Sum.HashString(SerializedMsg);
                    break;
            }

        }

        /// <summary>
        /// this ctor requires a plainstring and serialize it in _SerializedMsg
        /// </summary>
        /// <param name="plainTextMsg">plain text message</param>
        /// <param name="hash"></param>
        /// <param name="msgArt"></param>
        public CMsg(string plainTextMsg, string hash, SerType msgArt = SerType.Raw, string md5Hash = "")
        {
            Cerializer = msgArt;
            Hash = hash;
            Message = plainTextMsg;
            // SerializedMsg = "";
            CBytes = new byte[0];
            Md5Hash = md5Hash;

            //if (msgArt == CType.Json)
            //{
            //	SerializedMsg = this.ToJson();
            //}
            //if (msgArt == CType.Xml)
            //{
            //	SerializedMsg = this.ToXml();
            //}
            if (msgArt == SerType.Raw)
            {
                if (plainTextMsg.Contains(hash) && plainTextMsg.IndexOf(hash) > plainTextMsg.Length - 10)
                {
                    Message = SerializedMsg.Substring(0, SerializedMsg.Length - Hash.Length);
                }
                //else
                //{
                // SerializedMsg = Message + "\n" + hash + "\0";
                // }
            }
            // if (msgArt == CType.None)
            // {
            //	SerializedMsg = this.ToString();
            // }
        }


        public CMsg(CMsg srcToClone)
        {
            CloneCopy(srcToClone, this);
        }

        #endregion ctor

        #region members

        #region EnDeCrypt+DeSerialize


        public virtual string EncryptToJson(string serverKey, EncodingType encoder = EncodingType.Base64, 
            ZipType zipType = ZipType.None, KeyHash kh = KeyHash.Hex)
        {
            if (Encrypt(serverKey, encoder, zipType))
            {
                string serializedJson = Cerialize();
                return serializedJson;
            }
            throw new CqrException($"EncryptToJson(string severKey failed");
        }

        public virtual bool Encrypt(string serverKey, EncodingType encoder = EncodingType.Base64, ZipType zipType = ZipType.None)
        {
            string pipeString = "", encrypted = "", keyHash = EnDeCodeHelper.KeyToHex(serverKey);
            try
            {
                CipherPipe symmPipe = new CipherPipe(serverKey);
                pipeString = symmPipe.PipeString;
                encrypted = Encoding.UTF8.GetString(new CipherPipe(serverKey).EncryptEncodeBytes(
                    Encoding.UTF8.GetBytes(Message), serverKey, keyHash, encoder, zipType, KeyHash.Hex));
                // encrypted = CipherPipe.EncrpytToString(Message, serverKey, out pipeString, encoder, zipType);
                Hash = pipeString;
                Md5Hash = MD5Sum.HashString(string.Concat(serverKey, keyHash, pipeString, Message), "");

                Message = encrypted;
            }
            catch (Exception exCrypt)
            {
                CqrException.SetLastException(exCrypt);
                throw;
            }
            return true;
        }


        public virtual CMsg? DecryptFromJson(string serverKey, string serialized = "",
            EncodingType decoder = EncodingType.Base64, ZipType zipType = ZipType.None)
        {
            if (string.IsNullOrEmpty(serialized))
                serialized = SerializedMsg;

            CMsg? cc = Cerializer.DeCerialize<CMsg>(serialized);
            if (cc != null && cc.Decrypt(serverKey, decoder, zipType))
            {
                CloneCopy(cc, this);
                return cc;
            }
            throw new CqrException($"DecryptFromJson<T>(string severKey, string serialized) failed");
        }

        public virtual bool Decrypt(string serverKey, EncodingType decoder = EncodingType.Base64, ZipType zipType = ZipType.None)
        {
            string pipeString = "", keyHash = EnDeCodeHelper.KeyToHex(serverKey);
            try
            {
                CipherPipe symmPipe = new CipherPipe(serverKey);
                string hashIv = KeyHash.Hex.Hash(serverKey);
                pipeString = symmPipe.PipeString;

                string decrypted = Encoding.UTF8.GetString(symmPipe.DecodeDecrpytBytes(
                    Encoding.UTF8.GetBytes(Message), serverKey, hashIv, decoder, zipType, KeyHash.Hex));
                // string decrypted = CipherPipe.DecrpytToString(Message, serverKey, out pipeString, EncodingType.Base64, ZipType.None);

                if (!Hash.Equals(pipeString))
                    throw new CqrException($"CMsg.Hash={Hash} doesn't match PipeString={pipeString}");

                string md5Hash = MD5Sum.HashString(string.Concat(serverKey, keyHash, pipeString, decrypted), "");
                if (!md5Hash.Equals(Md5Hash))
                    throw new CqrException($"CMsg.Md5Hash={Md5Hash} doesn't match md5Hash={md5Hash}.");

                Message = decrypted;
                CBytes = new byte[0];
            }
            catch (Exception exCrypt)
            {
                CqrException.SetLastException(exCrypt);
                throw;
            }
            return true;
        }


        #endregion EnDeCrypt+DeSerialize


        #region serialization / deserialization

        /// <summary>
        /// Serialize all CC classes to json
        /// </summary>
        /// <returns>json serialized string</returns>
        public virtual string Cerialize() => Cerializer.Cerialize(this);

        public virtual T? DeCerialize<T>(string jsonText)
        {
            if (string.IsNullOrEmpty(jsonText))
                jsonText = SerializedMsg;

            T? t = Cerializer.DeCerialize<T>(jsonText);
            if (t != null)
            {
                if (t is CMsg cc)
                    cc.CCopy(this, cc);
                if (t is CContact cct)
                    cct.CCopy(this, cct);
                if (t is CFile cfile)
                    cfile.CCopy(this, cfile);
                else if (t is CImage cimg)
                    cimg.CCopy(this, cimg);
                else if (t is CSrvMsg<object> csrvmsg)
                    csrvmsg.CCopy(this, csrvmsg);
            }

            return t;
        }



        #endregion serialization / deserialization

        public virtual CMsg CCopy(CMsg leftDest, CMsg rightSrc)
        {
            return CloneCopy(rightSrc, leftDest);
        }



        public virtual bool IsCFile()
        {
            if (this is CFile cf && string.IsNullOrEmpty(cf.FileName) && cf.Data != null)
                return true;

            if (string.IsNullOrEmpty(SerializedMsg))
            {
                // if (MsgType == null || MsgType == CType.Json || MsgType == CType.Json)
                // SerializedMsg = this.ToJson();
                // else if (MsgType == CType.Xml)
                // SerializedMsg = this.ToXml();
            }
            if (SerializedMsg.IsValidJson() && SerializedMsg.Contains("FileName") && SerializedMsg.Contains("Base64Type") ||
                SerializedMsg.IsValidXml() && SerializedMsg.Contains("FileName") && SerializedMsg.Contains("Base64Type"))
                return true;

            return false;
        }


        public virtual CFile ToCFile()
        {
            if (this is CFile cf && string.IsNullOrEmpty(cf.FileName) && cf.Data != null)
                return cf;

            if (string.IsNullOrEmpty(SerializedMsg))
            {
                //if (MsgType == null || MsgType == CType.Json || MsgType == CType.Json)
                //	SerializedMsg = this.ToJson();
                //else if (MsgType == CType.Xml)
                //	SerializedMsg = this.ToXml();
            }
            if (SerializedMsg.IsValidJson() && SerializedMsg.Contains("FileName") && SerializedMsg.Contains("Base64Type"))
                return JsonConvert.DeserializeObject<CFile>(SerializedMsg);
            else if (SerializedMsg.IsValidXml() && SerializedMsg.Contains("CqrFileName") && SerializedMsg.Contains("Base64Type"))
                return Utils.DeserializeFromXml<CFile>(SerializedMsg);

            return null;
        }

        #endregion members

        #region static members

        public static CMsg GetMsgContentType(string serString, out Type outType, SerType msgType = SerType.None)
        {
            outType = typeof(CMsg);
            switch (msgType)
            {
                case SerType.Json:
                    if (serString.IsValidJson())
                    {
                        //if (serString.Contains("ServerMsg") && serString.Contains("ClientMsg") && serString.Contains("ServerMsgString") && serString.Contains("ClientMsgString"))
                        //{
                        //    outType = typeof(ClientSrvMsg<CryptMsg<string>, CryptMsg<string>>);
                        //    return (ClientSrvMsg<CryptMsg<string>, CryptMsg<string>>)
                        //        JsonConvert.DeserializeObject<CryptMsg<CryptMsg<string>, CryptMsg<string>>>(serString);
                        //}
                        if (serString.Contains("Sender") && serString.Contains("Recipients") && serString.Contains("TContent"))
                        {
                            outType = typeof(CSrvMsg<string>);
                            return (CSrvMsg<string>)JsonConvert.DeserializeObject<CSrvMsg<string>>(serString);
                        }

                        if (serString.Contains("FileName") && serString.Contains("Base64Type"))
                        {
                            outType = typeof(CFile);
                            CFile cFile = JsonConvert.DeserializeObject<CFile>(serString);
                            // cFile.SerializedMsg = serString;
                            return cFile;
                        }
                        if (serString.Contains("ImageFileName") && serString.Contains("ImageMimeType"))
                        {
                            outType = typeof(CImage);
                            return JsonConvert.DeserializeObject<CImage>(serString);
                        }
                        if (serString.Contains("ContactId") && serString.Contains("Cuid") && serString.Contains("Email"))
                        {
                            outType = typeof(CContact);
                            return JsonConvert.DeserializeObject<CContact>(serString);
                        }

                        outType = typeof(CMsg);
                        return JsonConvert.DeserializeObject<CMsg>(serString);
                    }
                    break;
                case SerType.Xml:
                    if (serString.IsValidXml())
                    {
                        //if (serString.Contains("ServerMsg") && serString.Contains("ClientMsg") && serString.Contains("ServerMsgString") && serString.Contains("ClientMsgString"))
                        //{
                        //    outType = typeof(ClientSrvMsg<CryptMsg<string>, CryptMsg<string>>);
                        //    return (ClientSrvMsg<CryptMsg<string>, CryptMsg<string>>)
                        //        Utils.DeserializeFromXml<ClientSrvMsg<CryptMsg<string>, CryptMsg<string>>>(serString);                            
                        //}
                        if (serString.Contains("Sender") && serString.Contains("Recipients") && serString.Contains("TContent"))
                        {
                            outType = typeof(CSrvMsg<string>);
                            return (CSrvMsg<string>)Utils.DeserializeFromXml<CSrvMsg<string>>(serString);
                        }
                        if (serString.Contains("FileName") && serString.Contains("Base64Type"))
                        {
                            outType = typeof(CFile);
                            return Utils.DeserializeFromXml<CFile>(serString);
                        }
                        if (serString.Contains("ImageFileName") && serString.Contains("ImageMimeType"))
                        {
                            outType = typeof(CImage);
                            return Utils.DeserializeFromXml<CImage>(serString);
                        }
                        if (serString.Contains("ContactId") && serString.Contains("Cuid") && serString.Contains("Email"))
                        {
                            outType = typeof(CContact);
                            return Utils.DeserializeFromXml<CContact>(serString);
                        }

                        outType = typeof(CMsg);
                        return Utils.DeserializeFromXml<CMsg>(serString);
                    }
                    break;
                case SerType.Raw:
                case SerType.None:
                default: throw new NotImplementedException("GetMsgContentType(...): case MsgEnum.RawWithHashAtEnd and MsgEnum.None not implemented");
            }

            return null;
        }


        public static string Encrypt(string serverKey, ref CMsg CMsg, EncodingType encType = EncodingType.Base64, ZipType zipType = ZipType.None)
        {
            // CMsg.SerializedMsg = "";
            CMsg.Md5Hash = "";
            string pipeString = "", encryptedMsg = "", keyHash = EnDeCodeHelper.KeyToHex(serverKey);

            try
            {
                CipherPipe symmPipe = new CipherPipe(serverKey);
                pipeString = symmPipe.PipeString;

                CMsg.Hash = pipeString;
                CMsg.Md5Hash = MD5Sum.HashString(string.Concat(serverKey, EnDeCodeHelper.KeyToHex(serverKey), pipeString, CMsg.Message), "");


                encryptedMsg = Encoding.UTF8.GetString(new CipherPipe(serverKey).EncryptEncodeBytes(
                    Encoding.UTF8.GetBytes(CMsg.Message), serverKey, keyHash, encType, zipType, KeyHash.Hex));
                // encryptedMsg = CipherPipe.EncrpytToString(CMsg.Message, serverKey, out pipeString, encType, zipType);                    
                CMsg.Message = encryptedMsg;

            }
            catch (Exception exCrypt)
            {
                CqrException.SetLastException(exCrypt);
                throw;
            }

            return encryptedMsg;
        }

        public static CMsg? Decrypt(ref CMsg CMsg, string serverKey, EncodingType encType = EncodingType.Base64)
        {
            string pipeString = "", keyHash = EnDeCodeHelper.KeyToHex(serverKey);
            try
            {
                CipherPipe symmPipe = new CipherPipe(serverKey);
                pipeString = symmPipe.PipeString;

                string decrypted = Encoding.UTF8.GetString(symmPipe.DecodeDecrpytBytes(
                    Encoding.UTF8.GetBytes(CMsg.Message), serverKey, keyHash, EncodingType.Base64, ZipType.None, KeyHash.Hex));
                // string decrypted = CipherPipe.DecrpytToString(CMsg.Message, serverKey, out pipeString, EncodingType.Base64, ZipType.None);

                if (!CMsg.Hash.Equals(pipeString))
                    throw new CqrException($"CMsg.Hash={CMsg.Hash} doesn't match PipeString={pipeString}");

                string md5Hash = MD5Sum.HashString(string.Concat(serverKey, EnDeCodeHelper.KeyToHex(serverKey), pipeString, decrypted), "");
                if (!md5Hash.Equals(CMsg.Md5Hash))
                    throw new CqrException($"CMsg.Md5Hash={CMsg.Md5Hash} doesn't match md5Hash={md5Hash}.");

                CMsg.Message = decrypted;
                CMsg.CBytes = new byte[0];
            }
            catch (Exception exCrypt)
            {
                CqrException.SetLastException(exCrypt);
                throw;
            }
            return CMsg;
        }


        public static CMsg CloneCopy(CMsg source, CMsg destination)
        {
            if (source == null)
                return null;
            if (destination == null)
                destination = new CMsg(source);

            destination.Hash = source.Hash;
            destination.Message = source.Message;
            destination.Cerializer = source.Cerializer;
            destination.CBytes = source.CBytes;
            destination.Md5Hash = source.Md5Hash;
            destination.EnCodingType = source.EnCodingType;

            return destination;
        }


        #endregion static members

    }

}
