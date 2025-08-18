using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Area23.At.Framework.Library.Util;
using Area23.At.Framework.Library;
using Area23.At.Framework.Library.Static;
using Area23.At.Framework.Library.Crypt.Cipher.Symmetric;
using Area23.At.Framework.Library.Crypt.EnDeCoding;
using Area23.At.Framework.Library.Crypt.Hash;
using System.Runtime.Remoting.Contexts;

namespace Area23.At.Framework.Library.Cqr.Msg
{


    /// <summary>
    /// CSrvMsg
    /// </summary>
    /// <typeparam name="TC"></typeparam>
    [Serializable]
    public class CSrvMsg<TC> : CContent, IMsgAble where TC : class
    {

        #region properties

        public CContact Sender { get; set; }

        public HashSet<CContact> Recipients { get; set; }

        public TC TContent { get; set; }

        public CChatRoom CRoom { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        protected internal List<string> Emails
        {
            get
            {
                HashSet<string> mails = new HashSet<string>();
                foreach (CContact c in Recipients)
                {
                    if (!mails.Contains(c.Email))
                        mails.Add(c.Email);
                }
                return mails.ToList();
            }
        }

        [Newtonsoft.Json.JsonIgnore]
        protected internal List<Guid> Cuids
        {
            get
            {
                HashSet<Guid> cuids = new HashSet<Guid>();
                foreach (CContact c in Recipients)
                {
                    if (!cuids.Contains(c.Cuid))
                        cuids.Add(c.Cuid);
                }
                return cuids.ToList();
            }
        }

        [Newtonsoft.Json.JsonIgnore]
        protected internal CContact Recipient
        {
            get => (Recipients == null || Recipients.Count < 1) ? null : Recipients.ElementAt(0);
            set
            {
                if (value != null && !string.IsNullOrEmpty(value.NameEmail))
                {
                    if (Recipients == null || Recipients.Count == 0)
                    {
                        Recipients = new HashSet<CContact>();
                        Recipients.Add(value);
                    }
                    else
                    {
                        CContact toRemove = null;
                        for (int ix = 0; ix < Recipients.Count; ix++)
                        {
                            if ((value.Cuid != null && value.Cuid != Guid.Empty && value.Cuid == Recipients.ElementAt(ix).Cuid) &&
                                ((value.Email == Recipients.ElementAt(ix).Email) ||
                                    (value.NameEmail == Recipients.ElementAt(ix).NameEmail) ||
                                    (value.Mobile == Recipients.ElementAt(ix).Mobile)))
                            {
                                toRemove = Recipients.ElementAt(ix);
                                break;
                            }
                        }
                        if (toRemove != null)
                        {
                            Recipients.Remove(toRemove);
                            Recipients.Add(value);
                        }

                    }
                }
            }
        }

        #endregion properties

        #region ctor

        public CSrvMsg() : base()
        {
            Message = string.Empty;
            // SerializedMsg = string.Empty;
            Hash = string.Empty;
            Sender = null;
            Recipients = new HashSet<CContact>();
            TContent = null;
            CBytes = null;
            CRoom = new CChatRoom();
        }

        public CSrvMsg(string serializedString, CType msgArt = CType.Json) : base()
        {
            CSrvMsg<TC> deserializedSrvMsg = null;
            if (string.IsNullOrEmpty(serializedString))
                throw new CqrException("Can not deserialize null or empty serializedString.");

            if (msgArt == CType.Json)
            {
                deserializedSrvMsg = this.FromJson<CSrvMsg<TC>>(serializedString);
                deserializedSrvMsg.MsgType = CType.Json;
            }
            else if (msgArt == CType.Xml)
            {
                deserializedSrvMsg = this.FromXml<CSrvMsg<TC>>(serializedString);
                deserializedSrvMsg.MsgType = CType.Xml;
            }

            if (deserializedSrvMsg == null)
                throw new CqrException("Can not deserialize serializedString to CSrvMsg<TC>.");

            this.Sender = deserializedSrvMsg.Sender;
            this.Recipients = deserializedSrvMsg.Recipients;
            this.TContent = deserializedSrvMsg.TContent;
            this.CRoom = deserializedSrvMsg.CRoom;
            this.Hash = deserializedSrvMsg.Hash;
            this.Message = deserializedSrvMsg.Message;
            this.CBytes = deserializedSrvMsg.CBytes;
            this.Md5Hash = deserializedSrvMsg.Md5Hash;
            this.MsgType = deserializedSrvMsg.MsgType;
        }


        [Obsolete("Always user CSrvMsg(CContact sender, CContact to, TC tc, string hash) : base() ctor", false)]
        public CSrvMsg(CContact sender, CContact to, TC tc) : base()
        {
            Sender = sender;
            CContact[] tos = (to != null) ? new CContact[1] { to } : new CContact[0];
            Recipients = new HashSet<CContact>(tos);
            TContent = tc;
        }


        /// <summary>
        /// constuctor
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="to"></param>
        /// <param name="tc"></param>
        /// <param name="hash"></param>
        public CSrvMsg(CContact sender, CContact to, TC tc, string hash, string chatRoomNr = "") : base()
        {
            Sender = sender;
            CContact[] tos = (to != null) ? new CContact[1] { to } : new CContact[0];
            Recipients = new HashSet<CContact>(tos);
            TContent = tc;
            Hash = hash;
            CRoom = new CChatRoom(chatRoomNr, Guid.NewGuid(), DateTime.MinValue, DateTime.MinValue);
            if (tc is string || tc is int || tc is long || tc is byte || tc is short || tc is uint || tc is ulong || tc is ushort || tc is sbyte || tc is float || tc is double)
                Message = tc.ToString();
            else
                Message = JsonConvert.SerializeObject(tc);
            // string allMsg = this.ToJson();
            // SerializedMsg = allMsg;
        }



        /// <summary>
        /// constuctor
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="to"></param>
        /// <param name="tc"></param>
        /// <param name="hash"></param>
        public CSrvMsg(CContact sender, CContact to, TC tc, string hash, CChatRoom chatRoom) : base()
        {
            Sender = sender;
            CContact[] tos = (to != null) ? new CContact[1] { to } : new CContact[0];
            Recipients = new HashSet<CContact>(tos);
            TContent = tc;
            Hash = hash;
            CRoom = new CChatRoom(chatRoom) { TicksLong = new List<long>(chatRoom.TicksLong) };
            if (tc is string || tc is int || tc is long || tc is byte || tc is short || tc is uint || tc is ulong || tc is ushort || tc is sbyte || tc is float || tc is double)
                Message = tc.ToString();
            else
                Message = JsonConvert.SerializeObject(tc);
            // string allMsg = this.ToJson();
            // SerializedMsg = allMsg;
        }

        /// <summary>
        /// Please always use this constuctor
        /// </summary>
        /// <param name="sender">CqrContact</param>
        /// <param name="tos">Array of CqrContact</param>
        /// <param name="tc"></param>
        /// <param name="hash"></param>
        /// <param name="chatRoom">chatroom</param>
        public CSrvMsg(CContact sender, CContact[] toMany, TC tc, string hash, CChatRoom chatRoom) : base()
        {
            Sender = sender;
            Recipients = new HashSet<CContact>(toMany);
            TContent = tc;
            Hash = hash;
            CRoom = new CChatRoom(chatRoom) { TicksLong = new List<long>(chatRoom.TicksLong) };
            if (tc is string || tc is int || tc is long || tc is byte || tc is short || tc is uint || tc is ulong || tc is ushort || tc is sbyte || tc is float || tc is double)
                Message = tc.ToString();
            else
                Message = JsonConvert.SerializeObject(tc);
            // string allMsg = this.ToJson();
            // SerializedMsg = allMsg;
        }


        /// <summary>
        /// Please always use this constuctor
        /// </summary>
        /// <param name="sender">CqrContact</param>
        /// <param name="tos">Array of CqrContact</param>
        /// <param name="tc"></param>
        /// <param name="hash"></param>
        public CSrvMsg(CContact sender, CContact[] tos, TC tc, string hash, string chatRoomNr = "") : base()
        {
            Sender = sender;
            Recipients = new HashSet<CContact>(tos);
            TContent = tc;
            Hash = hash;
            CRoom = new CChatRoom(chatRoomNr, Guid.NewGuid(), DateTime.MinValue, DateTime.MinValue);
            if (tc is string || tc is int || tc is long || tc is byte || tc is short || tc is uint || tc is ulong || tc is ushort || tc is sbyte || tc is float || tc is double)
                Message = tc.ToString();
            else
                Message = JsonConvert.SerializeObject(tc);
            // string allMsg = this.ToJson();
            // SerializedMsg = allMsg;
        }


        /// <summary>
        /// CSrvMsg ctor with instance passed to
        /// </summary>
        /// <param name="cSrvMsg">instance of <see cref="CSrvMsg{TC}"/></param>
        public CSrvMsg(CSrvMsg<TC> cSrvMsg) : this()
        {
            if (cSrvMsg != null)
            {
                CloneCopy(cSrvMsg, this);
            }
        }


        #endregion ctor

        public new CSrvMsg<TC> CCopy(CSrvMsg<TC> leftDest, CSrvMsg<TC> rightSrc)
        {
            return CloneCopy(rightSrc, leftDest);
        }

        #region EnDeCrypt+DeSerialize

        /// <summary>
        /// Serialize <see cref="CSrvMsg{TC}"/> to Json Stting
        /// </summary>
        /// <returns>json serialized string</returns>
        public override string EncryptToJson(string serverKey, EncodingType encoder = EncodingType.Base64, Zfx.ZipType zipType = Zfx.ZipType.None)
        {
            if (Encrypt(serverKey, encoder, zipType))
            {
                string serializedJson = ToJson();
                return serializedJson;
            }
            throw new CqrException($"EncryptToJson(string severKey failed");
        }

        public override bool Encrypt(string serverKey, EncodingType encoder = EncodingType.Base64, Zfx.ZipType zipType = Zfx.ZipType.None)
        {
            string pipeString = "", encrypted = "", keyHash = EnDeCodeHelper.KeyToHex(serverKey);
            try
            {
                if (TContent != null)
                {                    
                    Message = JsonConvert.SerializeObject(TContent);
                }
                pipeString = (new SymmCipherPipe(serverKey, keyHash)).PipeString;
                Hash = pipeString;
                Md5Hash = MD5Sum.HashString(String.Concat(serverKey, keyHash, pipeString, Message), "");

                encrypted = SymmCipherPipe.EncrpytToString(Message, serverKey, out pipeString, encoder, zipType);

                Message = encrypted;
            }
            catch (Exception exCrypt)
            {
                CqrException.SetLastException(exCrypt);
                throw;
            }
            return true;
        }

        public new CSrvMsg<TC> DecryptFromJson(string serverKey, string serialized = "",
            EncodingType decoder = EncodingType.Base64, Zfx.ZipType zipType = Zfx.ZipType.None)
        {
            if (string.IsNullOrEmpty(serialized))
                serialized = this.SerializedMsg;

            CSrvMsg<TC> csrvmsg = FromJson<CSrvMsg<TC>>(serialized);
            if (csrvmsg != null && Decrypt(serverKey, decoder, zipType))
            {
                csrvmsg.Message = Message;
                csrvmsg.CBytes = CBytes;
                csrvmsg.MsgType = MsgType;
                csrvmsg.Md5Hash = Md5Hash;
                csrvmsg.Hash = Hash;
                csrvmsg.TContent = TContent;
                csrvmsg.Sender = Sender;
                csrvmsg.Recipients = Recipients;
                // csrvmsg.SerializedMsg = "";
                // csrvmsg.SerializedMsg = csrvmsg.ToJson();
                return csrvmsg;
            }
            throw new CqrException($"DecryptFromJson<T>(string severKey, string serialized) failed");
        }

        public override bool Decrypt(string serverKey, EncodingType decoder = EncodingType.Base64, Zfx.ZipType zipType = Zfx.ZipType.None)
        {
            string pipeString = "", decrypted = "", keyHash = EnDeCodeHelper.KeyToHex(serverKey);
            try
            {
                pipeString = (new SymmCipherPipe(serverKey, keyHash)).PipeString;

                decrypted = SymmCipherPipe.DecrpytToString(Message, serverKey, out pipeString, decoder, zipType);

                if (!Hash.Equals(pipeString))
                {
                    string errMsg = $"CSrvMsg.Hash={Hash} doesn't match pipeString={pipeString}";
                    Area23Log.Log(errMsg);
                    // throw new CqrException(errMsg);
                }

                string md5Hash = MD5Sum.HashString(String.Concat(serverKey, keyHash, pipeString, decrypted), "");
                if (!md5Hash.Equals(Md5Hash))
                {
                    string md5ErrMsg = $"CSrvMsg.Md5Hash={Md5Hash} doesn't match md5Hash={md5Hash}.";
                    Area23Log.Log(md5ErrMsg);
                    // throw new CqrException(md5ErrMsg);
                }

                Message = decrypted;
                TContent = Newtonsoft.Json.JsonConvert.DeserializeObject<TC>(decrypted);

            }
            catch (Exception exCrypt)
            {
                CqrException.SetLastException(exCrypt);
                throw;
            }
            return true;
        }


        #endregion EnDeCrypt+DeSerialize

        #region members

        public override string ToJson()
        {
            // this.SerializedMsg = "";
            string jsonText = JsonConvert.SerializeObject(this);
            // this.SerializedMsg = jsonText;
            return jsonText;
        }

        public new CSrvMsg<TC> FromJson(string jsonText)
        {
            CSrvMsg<TC> cMsg = JsonConvert.DeserializeObject<CSrvMsg<TC>>(jsonText);
            try
            {
                if (cMsg != null && cMsg is CSrvMsg<TC> cSrvMsg)
                {
                    if (cSrvMsg != null && !string.IsNullOrEmpty(cSrvMsg.SerializedMsg))
                    {
                        Sender = cSrvMsg.Sender;
                        Recipients = cSrvMsg.Recipients;
                        TContent = cSrvMsg.TContent;
                        CRoom = new CChatRoom(cSrvMsg.CRoom);
                        Hash = cSrvMsg.Hash;
                        Md5Hash = cSrvMsg.Md5Hash;
                        Message = cSrvMsg.Message;
                        MsgType = CType.Json;
                        // SerializedMsg = jsonText;
                    }
                    return cMsg;
                }
            }
            catch (Exception exJson)
            {
                Area23Log.LogOriginMsgEx("CSrvMsg", "FromJson", exJson);
            }

            return default(CSrvMsg<TC>);
        }

        public override string ToXml()
        {
            // SerializedMsg = "";
            string xmlString = Utils.SerializeToXml<CSrvMsg<TC>>(this);
            // SerializedMsg = xmlString;
            return xmlString;
        }

        public new T FromXml<T>(string xmlText)
        {
            T cqrT = Utils.DeserializeFromXml<T>(xmlText);
            if (cqrT is CSrvMsg<TC> cSrvMsg)
            {
                // this.SerializedMsg = xmlText;
                this.MsgType = CType.Xml;
                this.Md5Hash = cSrvMsg.Md5Hash;
                this.Hash = cSrvMsg.Hash;
                this.Message = cSrvMsg.Message;
                this.CBytes = cSrvMsg.CBytes;
                this.TContent = cSrvMsg.TContent;
                this.CRoom = cSrvMsg.CRoom;
                this.Sender = cSrvMsg.Sender;
                this.Recipients = cSrvMsg.Recipients;
            }

            return cqrT;
        }

        public string[] GetEmails() => this.Emails.ToArray();

        #endregion members


        #region static members 

        #region static members ToJsonEncrypt EncryptSrvMsg FromJsonDecrypt DecryptSrvMsg

        /// <summary>
        /// Serialize <see cref="CSrvMsg{TC}"/> to Json Stting
        /// </summary>
        /// <returns>json serialized string</returns>
        public static string ToJsonEncrypt(string serverKey, CSrvMsg<TC> cSrvMsg,
            EncodingType encoder = EncodingType.Base64, Zfx.ZipType zipType = Zfx.ZipType.None)
        {
            if (EncryptSrvMsg(serverKey, ref cSrvMsg, encoder, zipType))
            {
                string serializedJson = cSrvMsg.ToJson();
                return serializedJson;
            }
            throw new CqrException($"EncryptToJson(string severKey, CSrvMsg<TC> cSrvMsg) failed");
        }

        public static bool EncryptSrvMsg(string serverKey, ref CSrvMsg<TC> cSrvMsg,
            EncodingType encoder = EncodingType.Base64, Zfx.ZipType zipType = Zfx.ZipType.None)
        {
            string encrypted = "", pipeString = "", keyHash = EnDeCodeHelper.KeyToHex(serverKey);
            try
            {
                if (cSrvMsg.TContent != null)
                {                    
                    cSrvMsg.Message = JsonConvert.SerializeObject(cSrvMsg.TContent);
                }
                pipeString = (new SymmCipherPipe(serverKey, keyHash)).PipeString;
                cSrvMsg.Hash = pipeString;
                cSrvMsg.Md5Hash = MD5Sum.HashString(String.Concat(serverKey, keyHash, pipeString, cSrvMsg.Message), "");

                encrypted = SymmCipherPipe.EncrpytToString(cSrvMsg.Message, serverKey, out pipeString, encoder, zipType);
                cSrvMsg.Message = encrypted;
            }
            catch (Exception exCrypt)
            {
                CqrException.SetLastException(exCrypt);
                throw;
            }
            return true;
        }

        public static CSrvMsg<TC> FromJsonDecrypt(string serverKey, string serialized,
             EncodingType decoder = EncodingType.Base64, Zfx.ZipType zipType = Zfx.ZipType.None)
        {
            if (string.IsNullOrEmpty(serialized))
                throw new CqrException("static CSrvMsg<TC> FromJsonDecrypt(string serverKey, string serialized): serialized is null or empty.");

            CSrvMsg<TC> cSrvMsgDeserialized = Newtonsoft.Json.JsonConvert.DeserializeObject<CSrvMsg<TC>>(serialized);
            CSrvMsg<TC> cSrvMsgDecrypted = DecryptSrvMsg(serverKey, ref cSrvMsgDeserialized, decoder, zipType);

            if (cSrvMsgDecrypted != null)
            {
                cSrvMsgDecrypted.Recipients = cSrvMsgDeserialized.Recipients;
                cSrvMsgDecrypted.Sender = cSrvMsgDeserialized.Sender;
                cSrvMsgDecrypted.CRoom = cSrvMsgDeserialized.CRoom;
                return cSrvMsgDecrypted;
            }

            throw new CqrException($"DecryptFromJson<T>(string severKey, string serialized) failed");
        }

        public static CSrvMsg<TC> DecryptSrvMsg(string serverKey, ref CSrvMsg<TC> cSrvMsg,
            EncodingType decoder = EncodingType.Base64, Zfx.ZipType zipType = Zfx.ZipType.None)
        {
            string pipeString = "", decrypted = "", keyHash = EnDeCodeHelper.KeyToHex(serverKey);
            try
            {
                pipeString = (new SymmCipherPipe(serverKey, keyHash)).PipeString;

                decrypted = SymmCipherPipe.DecrpytToString(cSrvMsg.Message, serverKey, out pipeString, decoder, zipType);

                if (!cSrvMsg.Hash.Equals(pipeString))
                {
                    string errMsg = $"cSrvMsg.Hash={cSrvMsg.Hash} doesn't match pipeString={pipeString}";
                    Area23Log.Log(errMsg);
                    // throw new CqrException(errMsg);
                    ;
                }
                string md5Hash = MD5Sum.HashString(String.Concat(serverKey, cSrvMsg.Hash, pipeString, decrypted), "");
                if (!md5Hash.Equals(cSrvMsg.Md5Hash))
                {
                    string md5ErrExcMsg = $"CSrvMsg-Md5Hash={cSrvMsg.Md5Hash} doesn't match md5Hash={md5Hash}";
                    Area23Log.Log(md5ErrExcMsg);
                    // throw new CqrException(md5ErrExcMsg);
                    ;
                }

                cSrvMsg.Message = decrypted;
                cSrvMsg.TContent = Newtonsoft.Json.JsonConvert.DeserializeObject<TC>(decrypted);            
            }
            catch (Exception exCrypt)
            {
                CqrException.SetLastException(exCrypt);
                throw;
            }

            return cSrvMsg;
        }

        #endregion static members ToJsonEncrypt EncryptSrvMsg FromJsonDecrypt DecryptSrvMsg

        public new static CSrvMsg<TC> CloneCopy(CSrvMsg<TC> source, CSrvMsg<TC> destination)
        {
            if (source == null)
                return null;
            if (source == null)
                destination = new CSrvMsg<TC>(source);

            destination.Hash = source.Hash;
            destination.Message = source.Message;
            destination.MsgType = source.MsgType;
            destination.CBytes = source.CBytes;
            destination.Md5Hash = source.Md5Hash;

            destination.Sender = source.Sender;
            destination.Recipients = source.Recipients;
            destination.TContent = source.TContent;
            destination.CRoom = source.CRoom;

            // destination.SerializedMsg = "";
            // destination.SerializedMsg = destination.ToJson();

            return destination;
        }

        #endregion static members 

    }

}
