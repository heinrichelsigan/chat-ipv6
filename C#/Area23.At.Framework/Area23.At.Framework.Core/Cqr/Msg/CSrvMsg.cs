﻿using Area23.At.Framework.Core.Crypt.Cipher.Symmetric;
using Area23.At.Framework.Core.Crypt.EnDeCoding;
using Area23.At.Framework.Core.Crypt.Hash;
using Area23.At.Framework.Core.Static;
using Area23.At.Framework.Core.Util;
using Area23.At.Framework.Core.Zfx;
using Newtonsoft.Json;
using System.Security.Policy;

namespace Area23.At.Framework.Core.Cqr.Msg
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

        public CChatRoom? CRoom { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public string Emails
        {
            get
            {
                HashSet<string> mails = new HashSet<string>();
                mails.Add(Sender.Email);
                foreach (CContact c in Recipients)
                {
                    if (!mails.Contains(c.Email))
                        mails.Add(c.Email);
                }

                return string.Join("; ", mails.ToArray());
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
        public CContact Recipient
        {
            get => (Recipients == null || Recipients.Count < 1) ? null : Recipients.ElementAt(0);
            protected internal set
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

        public CSrvMsg(string serializedString, SerType msgArt = SerType.Json) : base()
        {
            CSrvMsg<TC> deserializedSrvMsg = null;
            if (string.IsNullOrEmpty(serializedString))
                throw new CqrException("Can not deserialize null or empty serializedString.");

            if (msgArt == SerType.Json)
            {
                deserializedSrvMsg = this.FromJson<CSrvMsg<TC>>(serializedString);
                deserializedSrvMsg.MsgType = SerType.Json;
            }
            else if (msgArt == SerType.Xml)
            {
                deserializedSrvMsg = this.FromXml<CSrvMsg<TC>>(serializedString);
                deserializedSrvMsg.MsgType = SerType.Xml;
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
            CRoom = new CChatRoom(chatRoom) { MsgDict = new Dictionary<long, string>(chatRoom.MsgDict) };
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
			CRoom = new CChatRoom(chatRoom) { MsgDict = new Dictionary<long, string>(chatRoom.MsgDict) };
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

        public CSrvMsg<TC> CCopy(CSrvMsg<TC> leftDest, CSrvMsg<TC> rightSrc)
        {
            return CloneCopy(rightSrc, leftDest);
        }

        #region EnDeCrypt+DeSerialize

        /// <summary>
        /// Serialize <see cref="CSrvMsg{TC}"/> to Json Stting
        /// </summary>
        /// <returns>json serialized string</returns>
        public override string EncryptToJson(string serverKey, EncodingType encoder = EncodingType.Base64, ZipType zipType = ZipType.None, KeyHash kHash = KeyHash.Hex)
        {
            if (Encrypt(serverKey, encoder, zipType, kHash))
            {
                string serializedJson = ToJson();
                return serializedJson;
            }
            throw new CqrException($"EncryptToJson(string severKey failed");
        }

        public override bool Encrypt(string serverKey, EncodingType encoder = EncodingType.Base64, ZipType zipType = ZipType.None, KeyHash kHash = KeyHash.Hex)
        {
            string keyHash = kHash.Hash(serverKey);
            try
            {                
                if (TContent != null)
                {
                    Message = JsonConvert.SerializeObject(TContent);
                }
                string pipeString = (new SymmCipherPipe(serverKey, keyHash)).PipeString;
                Hash = pipeString;
                Md5Hash = MD5Sum.HashString(String.Concat(serverKey, keyHash, pipeString, Message), "");

                string encrypted = SymmCipherPipe.EncrpytToString(Message, serverKey, out pipeString, encoder, zipType, kHash);

                Message = encrypted;
                TContent = null;
            }
            catch (Exception exCrypt)
            {
                CqrException.SetLastException(exCrypt);
                throw;
            }
            return true;
        }

        public new CSrvMsg<TC> DecryptFromJson(string serverKey, string serialized = "",
            EncodingType decoder = EncodingType.Base64, ZipType zipType = ZipType.None, KeyHash kHash = KeyHash.Hex) 
        {
            if (string.IsNullOrEmpty(serialized))
                serialized = this.SerializedMsg;

            CSrvMsg<TC> csrvmsg = FromJson<CSrvMsg<TC>>(serialized);
            if (csrvmsg != null && Decrypt(serverKey, decoder, zipType, kHash))
            {
                csrvmsg.Message = Message;
                csrvmsg.CBytes = CBytes;
                csrvmsg.MsgType = MsgType;
                csrvmsg.Md5Hash = Md5Hash;
                csrvmsg.Hash = Hash;
                csrvmsg.KHash = KHash;
                csrvmsg.ZType = ZType;
                csrvmsg.TContent = TContent;
                csrvmsg.Sender = Sender;
                csrvmsg.Recipients = Recipients;
                return csrvmsg;
            }
            throw new CqrException($"DecryptFromJson<T>(string severKey, string serialized) failed");
        }

        public override bool Decrypt(string serverKey, EncodingType decoder = EncodingType.Base64, ZipType zipType = ZipType.None, KeyHash kHash = KeyHash.Hex)
        {
            string  pipeString = "", decrypted = "", keyHash = kHash.Hash(serverKey);
            try
            {
                pipeString = (new SymmCipherPipe(serverKey, keyHash)).PipeString;

                decrypted = SymmCipherPipe.DecrpytToString(Message, serverKey, out pipeString, decoder, zipType, kHash);

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

        public override T? FromJson<T>(string jsonText) where T : default
        {            
            CSrvMsg<TC> cMsg = JsonConvert.DeserializeObject<CSrvMsg<TC>>(jsonText);
            try
            {
                if (this is T t && cMsg is T && cMsg != null)
                {
                    Sender = new CContact(cMsg.Sender);
                    Recipients = cMsg.Recipients;
                    TContent = cMsg.TContent;
                    CRoom = new CChatRoom(cMsg.CRoom);
                    Hash = cMsg.Hash;
                    Md5Hash = cMsg.Md5Hash;
                    Message = cMsg.Message;
                    MsgType = SerType.Json;

                    return t;
                }
            }
            catch (Exception exJson)
            {
                Area23Log.LogOriginMsgEx("CSrvMsg", "FromJson", exJson);
            }
            
            return base.FromJson<T>(jsonText);
        }

        public override string ToXml() => Utils.SerializeToXml<CSrvMsg<TC>>(this);

        public override T FromXml<T>(string xmlText)
        {
            CSrvMsg<TC> cMsg = Utils.DeserializeFromXml<CSrvMsg<TC>>(xmlText);
            try
            {
                if (this is T t && cMsg is T && cMsg != null)
                {
                    Sender = new CContact(cMsg.Sender);
                    Recipients = cMsg.Recipients;
                    TContent = cMsg.TContent;
                    CRoom = new CChatRoom(cMsg.CRoom);
                    Hash = cMsg.Hash;
                    Md5Hash = cMsg.Md5Hash;
                    Message = cMsg.Message;
                    MsgType = SerType.Xml;

                    return t;
                }
            }
            catch (Exception exJson)
            {
                Area23Log.LogOriginMsgEx("CSrvMsg", "FromXml", exJson);
            }

            return base.FromXml<T>(xmlText);
        }

        public string[] GetEmails() => this.Emails.Split(";".ToCharArray());

        #endregion members


        #region static members 

        #region static members Encrypt2Json Json2Decrypt

        /// <summary>
        /// Serialize <see cref="CSrvMsg{TC}"/> to Json Stting
        /// </summary>
        /// <returns>json serialized string</returns>
        public static string Encrypt2Json(string serverKey, CSrvMsg<TC> cSrvMsg,
            EncodingType encoder = EncodingType.Base64, ZipType zipType = ZipType.None, KeyHash kHash = KeyHash.Hex)
        {               
            string keyHash = kHash.Hash(serverKey);
            try
            {
                if (cSrvMsg.TContent != null)
                {
                    cSrvMsg.Message = JsonConvert.SerializeObject(cSrvMsg.TContent);
                }
                string pipeString = (new SymmCipherPipe(serverKey, keyHash)).PipeString;
                cSrvMsg.Hash = pipeString;
                cSrvMsg.Md5Hash = MD5Sum.HashString(String.Concat(serverKey, keyHash, pipeString, cSrvMsg.Message), "");

                string encrypted = SymmCipherPipe.EncrpytToString(cSrvMsg.Message, serverKey, out pipeString, encoder, zipType, kHash);
                cSrvMsg.Message = encrypted;
                cSrvMsg.TContent = null;                     
            }
            catch (Exception exCrypt)
            {
                CqrException.SetLastException(exCrypt);
                throw;
            }
            string serializedJson = JsonConvert.SerializeObject(cSrvMsg, Newtonsoft.Json.Formatting.Indented);
            if (string.IsNullOrEmpty(serializedJson))            
                throw new CqrException($"Encrypt2Json(string severKey, CSrvMsg<TC> cSrvMsg) failed");

            return serializedJson;
        }

        public static new CSrvMsg<TC> Json2Decrypt(string serverKey, string serialized,
             EncodingType decoder = EncodingType.Base64, ZipType zipType = ZipType.None, KeyHash kHash = KeyHash.Hex)
        {
            if (string.IsNullOrEmpty(serialized))
                throw new CqrException("static CSrvMsg<TC> FromJsonDecrypt(string serverKey, string serialized): serialized is null or empty.");

            CSrvMsg<TC> cSrvMsg = Newtonsoft.Json.JsonConvert.DeserializeObject<CSrvMsg<TC>>(serialized);

            string keyHash = kHash.Hash(serverKey);
            try
            {
                string pipeString = (new SymmCipherPipe(serverKey, keyHash)).PipeString;

                string decrypted = SymmCipherPipe.DecrpytToString(cSrvMsg.Message, serverKey, out pipeString, decoder, zipType, kHash);

                if (!cSrvMsg.Hash.Equals(pipeString))
                {
                    string errMsg = $"cSrvMsg.Hash={cSrvMsg.Hash} doesn't match pipeString={pipeString}";
                    Area23Log.Log(errMsg);
                    // throw new CqrException(errMsg);
                    ;
                }
                string md5Hash = MD5Sum.HashString(String.Concat(serverKey, keyHash, pipeString, decrypted), "");
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

        #endregion static members Encrypt2Json Json2Decrypt

        public static CSrvMsg<TC>? CloneCopy(CSrvMsg<TC> source, CSrvMsg<TC> destination)
        {
            if (source == null)
                return null;
            if (destination == null)
            {
                destination = new CSrvMsg<TC>(source);
                return destination;
            }

            destination.Hash = source.Hash;
            destination.Message = source.Message;
            destination.MsgType = source.MsgType;
            destination.CBytes = source.CBytes;
            destination.Md5Hash = source.Md5Hash;
            destination.KHash = source.KHash;
            destination.ZType = source.ZType;

            destination.Sender = source.Sender;
            destination.Recipients = source.Recipients;                        
            destination.CRoom = (source.CRoom != null) ? new CChatRoom(source.CRoom) : source.CRoom;

            destination.TContent = source.TContent;

            return destination;
        }

        #endregion static members 

    }

}
