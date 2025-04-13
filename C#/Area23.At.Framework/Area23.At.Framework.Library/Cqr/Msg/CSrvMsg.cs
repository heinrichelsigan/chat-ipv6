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

        #region chatroom properties
        
        public CChatRoom CRoom { get; set; }

        #endregion chatroom properties

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
            _message = string.Empty;
            SerializedMsg = string.Empty;
            _hash = string.Empty;
            Sender = null;
            Recipients = new HashSet<CContact>();
            TContent = null;
            CRoom = new CChatRoom();            
        }

        public CSrvMsg(string fm, CType msgArt = CType.Json) : base()
        {
            this.FromJson<CSrvMsg<TC>>(fm);
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
        /// Please always use this constuctor
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
            _hash = hash;
            CRoom = new CChatRoom(chatRoomNr, Guid.NewGuid(), DateTime.MinValue, DateTime.MinValue);            
            string allMsg = this.ToJson();
            _message = allMsg;
            SerializedMsg = allMsg;
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
            _hash = hash;
            CRoom = new CChatRoom(chatRoomNr, Guid.NewGuid(), DateTime.MinValue, DateTime.MinValue);            
            if (tc is string || tc is int || tc is long || tc is byte || tc is short || tc is uint || tc is ulong || tc is ushort || tc is sbyte || tc is float || tc is double) 
                _message = tc.ToString();
            else
                _message = JsonConvert.SerializeObject(tc);                    
            string allMsg = this.ToJson();
            SerializedMsg = allMsg;
        }


        /// <summary>
        /// CSrvMsg ctor with instance passed to
        /// </summary>
        /// <param name="cSrvMsg">instance of <see cref="CSrvMsg{TC}"/></param>
        public CSrvMsg(CSrvMsg<TC> cSrvMsg) : this() 
        { 
            if (cSrvMsg != null)
            {
                Sender = cSrvMsg.Sender;
                Recipients = cSrvMsg.Recipients;
                TContent = cSrvMsg.TContent;
                _hash = cSrvMsg.Hash;
                CRoom = new CChatRoom(cSrvMsg.CRoom);
                _message = cSrvMsg.Message;
                Md5Hash = cSrvMsg.Md5Hash;
            }
        }

        #endregion ctor


        #region EnDeCrypt+DeSerialize


        public override byte[] EncryptToJsonToBytes(string serverKey)
        {
            string serialized = EncryptToJson(serverKey);
            return Encoding.UTF8.GetBytes(serialized);
        }

        public override string EncryptToJson(string serverKey)
        {
            if (Encrypt(serverKey))
            {
                string serializedJson = ToJson();
                return serializedJson;
            }
            throw new CqrException($"EncryptToJson(string severKey failed");
        }

        public override bool Encrypt(string serverKey)
        {
            try
            {
                string hash = EnDeCodeHelper.KeyToHex(serverKey);
                SymmCipherPipe symmPipe = new SymmCipherPipe(serverKey, hash);
                this.Md5Hash = MD5Sum.HashString(_message);
                _hash = symmPipe.PipeString;
                byte[] msgBytes = EnDeCodeHelper.GetBytesFromString(Message);

                byte[] cqrbytes = LibPaths.CqrEncrypt ? symmPipe.MerryGoRoundEncrpyt(msgBytes, serverKey, hash) : msgBytes;
                CBytes = cqrbytes;
            }
            catch (Exception exCrypt)
            {
                CqrException.SetLastException(exCrypt);
                throw;
            }
            return true;
        }

        public new CSrvMsg<TC> DecryptFromJsonFromBytes(string serverKey, byte[] serializedBytes)  
        {
            string serialized = Encoding.UTF8.GetString(serializedBytes);
            return DecryptFromJson(serverKey, serialized);
        }


        public new CSrvMsg<TC> DecryptFromJson(string serverKey, string serialized = "") 
        {
            if (string.IsNullOrEmpty(serialized))
                serialized = this.SerializedMsg;

            CSrvMsg<TC> csrvmsg = FromJson<CSrvMsg<TC>>(serialized);
            if (csrvmsg != null && Decrypt(serverKey))
            {
                csrvmsg._message = _message;
                csrvmsg.CBytes = CBytes;
                csrvmsg.Md5Hash = Md5Hash;
                csrvmsg._hash = Hash;
                csrvmsg.TContent = TContent;
                csrvmsg.Sender = Sender;
                csrvmsg.Recipients = Recipients;
                return csrvmsg;
            }
            throw new CqrException($"DecryptFromJson<T>(string severKey, string serialized) failed");
        }

        public override bool Decrypt(string serverKey)
        {
            try
            {
                string hash = EnDeCodeHelper.KeyToHex(serverKey);
                SymmCipherPipe symmPipe = new SymmCipherPipe(serverKey, hash);

                byte[] cipherBytes = CBytes;
                byte[] unroundedMerryBytes = LibPaths.CqrEncrypt ? symmPipe.DecrpytRoundGoMerry(cipherBytes, serverKey, hash) : cipherBytes;
                string decrypted = EnDeCodeHelper.GetString(unroundedMerryBytes); //DeEnCoder.GetStringFromBytesTrimNulls(unroundedMerryBytes);
                while (decrypted[decrypted.Length - 1] == '\0')
                    decrypted = decrypted.Substring(0, decrypted.Length - 1);

                string md5Hash = MD5Sum.HashString(decrypted);
                if (!_hash.Equals(symmPipe.PipeString))
                    throw new CqrException($"Hash: {_hash} doesn't match symmPipe.PipeString: {symmPipe.PipeString}");
                if (!md5Hash.Equals(Md5Hash))
                    throw new CqrException($"md5Hash: {md5Hash} doesn't match property Md5Hash: {Md5Hash}");

                _message = decrypted;
                CBytes = null;
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
            this.SerializedMsg = "";
            string jsonText = JsonConvert.SerializeObject(this);
            this.SerializedMsg = jsonText;
            return jsonText;
        }

        public new CSrvMsg<TC> FromJson(string jsonText) 
        {
            CSrvMsg<TC> cMsg = JsonConvert.DeserializeObject<CSrvMsg<TC>>(jsonText);
            try
            {
                if (cMsg != null && cMsg is CSrvMsg<TC> cSrvMsg && cSrvMsg != null)
                {                    
                    Sender = cSrvMsg.Sender;
                    Recipients = cSrvMsg.Recipients;
                    TContent = cSrvMsg.TContent;
                    CRoom = new CChatRoom(cSrvMsg.CRoom);
                    _hash = cSrvMsg._hash;
                    Md5Hash = cSrvMsg.Md5Hash;
                    _message = cSrvMsg._message;
                    SerializedMsg = jsonText;
                    
                    return cMsg;
                }
            }
            catch (Exception exJson)
            {
                SLog.Log(exJson);
            }

            return default(CSrvMsg<TC>);
        }

        public string[] GetEmails() => this.Emails.ToArray();

        #endregion members

        #region static_members

        public static CSrvMsg<TC> FromJsonDecrypt(string serverKey, string serialized)
        {
            if (string.IsNullOrEmpty(serialized))
                throw new CqrException("static CSrvMsg<TC> FromJsonDecrypt(string serverKey, string serialized): serialized is null or empty.");

            CSrvMsg<TC> csrvmsg = new CSrvMsg<TC>(serialized, CType.Json);
            csrvmsg = DecryptSrvMsg(serverKey, csrvmsg);
            if (csrvmsg != null)
                return csrvmsg;
            throw new CqrException($"DecryptFromJson<T>(string severKey, string serialized) failed");
        }

        public static CSrvMsg<TC> DecryptSrvMsg(string serverKey, CSrvMsg<TC> cSrvMsg)
        {
            try
            {
                string hash = EnDeCodeHelper.KeyToHex(serverKey);
                SymmCipherPipe symmPipe = new SymmCipherPipe(serverKey, hash);

                byte[] cipherBytes = cSrvMsg.CBytes;
                byte[] unroundedMerryBytes = LibPaths.CqrEncrypt ? symmPipe.DecrpytRoundGoMerry(cipherBytes, serverKey, hash) : cipherBytes;
                string decrypted = EnDeCodeHelper.GetString(unroundedMerryBytes); //DeEnCoder.GetStringFromBytesTrimNulls(unroundedMerryBytes);
                while (decrypted[decrypted.Length - 1] == '\0')
                    decrypted = decrypted.Substring(0, decrypted.Length - 1);

                string md5Hash = MD5Sum.HashString(decrypted, "");
                if (!cSrvMsg._hash.Equals(symmPipe.PipeString))
                    throw new CqrException($"Hash: {cSrvMsg._hash} doesn't match symmPipe.PipeString: {symmPipe.PipeString}");
                if (!md5Hash.Equals(cSrvMsg.Md5Hash))
                    throw new CqrException($"md5Hash: {md5Hash} doesn't match property Md5Hash: {cSrvMsg.Md5Hash}");

                cSrvMsg._message = decrypted;
                cSrvMsg.CBytes = null;
            }
            catch (Exception exCrypt)
            {
                CqrException.SetLastException(exCrypt);
                throw;
            }

            return cSrvMsg;
        }

        #endregion static_members

    }

}
