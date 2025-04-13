using Area23.At.Framework.Core.Cqr.Msg;
using Area23.At.Framework.Core.Crypt.Cipher.Symmetric;
using Area23.At.Framework.Core.Crypt.EnDeCoding;
using Area23.At.Framework.Core.Crypt.Hash;
using Area23.At.Framework.Core.Static;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace Area23.At.Framework.Core.Cqr.Msg
{
    [Serializable]
    public class CContact : CContent, IMsgAble
    {

        #region properties

        public int ContactId { get; set; }

        public Guid Cuid { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string Mobile { get; set; }

        public string Address { get; set; }

        public string SecretKey { get; set; }

        public CImage ContactImage { get; set; }

        public string NameEmail { get => string.IsNullOrEmpty(Email) ? Name : $"{Name} <{Email}>"; }

        #region from server given properties

        /// <summary>
        /// CRoom ChatRoom property 
        /// </summary>
        public CChatRoom CRoom { get; set; }

        
        #endregion from server given properties

        #endregion properties

        #region constructors

        /// <summary>
        /// Parameterless default constructor
        /// </summary>
        public CContact() : base()
        {
            ContactId = -1;
            Cuid = Guid.Empty;
            Name = string.Empty;
            Email = string.Empty;
            Mobile = string.Empty;
            Address = string.Empty;
            SecretKey = string.Empty;
            ContactImage = null;
            CRoom = new CChatRoom("", Guid.Empty, DateTime.MinValue, DateTime.MinValue);           
        }

        public CContact(string cs, CType msgArt = CType.Json)
        {
            FromJson<CContact>(cs);
        }

        public CContact(int cid, string name, string email, string mobile, string address) : base()
        {
            ContactId = cid;
            Name = name;
            Email = email;
            Mobile = mobile;
            Address = address;
            CRoom = new CChatRoom("", Guid.Empty, DateTime.MinValue, DateTime.MinValue);            
        }

        public CContact(Guid guid, string name, string email, string mobile, string address) : base()
        {
            Cuid = guid;
            Name = name;
            Email = email;
            Mobile = mobile;
            Address = address;

            CRoom = new CChatRoom("", Guid.Empty, DateTime.MinValue, DateTime.MinValue);
            // ClientIp = null;
        }

        public CContact(int cid, string name, string email, string mobile, string address, CImage cqrImage)
            : this(cid, name, email, mobile, address)
        {
            ContactImage = cqrImage;
        }

        public CContact(int cid, Guid cuid, string name, string email, string mobile, string address, CImage cqrImage)
            : this(cid, name, email, mobile, address)
        {
            Cuid = cuid;
            ContactImage = cqrImage;
        }

        public CContact(int cid, Guid cuid, string name, string email, string mobile, string address, CImage cqrImage, string hash)
            : this(cid, cuid, name, email, mobile, address, cqrImage)
        {
            _hash = hash;
        }

        public CContact(int cid, string name, string email, string mobile, string address, Image image)
            : this(cid, name, email, mobile, address)
        {
            ContactImage = CImage.FromDrawingImage(image);
        }

        public CContact(int cid, Guid cuid, string name, string email, string mobile, string address, Image image)
            : this(cid, name, email, mobile, address)
        {
            Cuid = cuid;
            ContactImage = CImage.FromDrawingImage(image);
        }

        public CContact(int cid, Guid cuid, string name, string email, string mobile, string address, Image image, string hash)
            : this(cid, cuid, name, email, mobile, address, image)
        {
            this._hash = hash;
        }

        public CContact(CContact c, string hash)
            : this(c.ContactId, c.Cuid, c.Name, c.Email, c.Mobile, c.Address, c.ContactImage, hash)
        {
            this._hash = hash;
            Cuid = (c.Cuid == Guid.Empty) ? Guid.NewGuid() : c.Cuid;
            CRoom = (c.CRoom == null)
                ? new CChatRoom("", Guid.Empty, DateTime.MinValue, DateTime.MinValue)
                : new CChatRoom(c.CRoom.ChatRoomNr, c.CRoom.ChatRuid, c.CRoom.LastPushed, c.CRoom.LastPushed) { TicksLong = c.CRoom.TicksLong };               
        }

        public CContact(CContact c, string ChatRoomNr, string hash) : this(c, hash)
        {
            _hash = hash;
            ContactImage = null;
            Cuid = (c.Cuid == Guid.Empty) ? Guid.NewGuid() : c.Cuid;
            CRoom = (c.CRoom == null)
                ? new CChatRoom("", Guid.Empty, DateTime.MinValue, DateTime.MinValue)
                : new CChatRoom(c.CRoom.ChatRoomNr, c.CRoom.ChatRuid, c.CRoom.LastPushed, c.CRoom.LastPushed) { TicksLong = c.CRoom.TicksLong };
        }

        public CContact(CContact c, string chatRoomNr, string hash, CImage cqrImage) : this(c, chatRoomNr, hash)
        {
            _hash = hash;
            ContactImage = cqrImage;
            ContactId = c.ContactId;
            Cuid = (c.Cuid == Guid.Empty) ? Guid.NewGuid() : c.Cuid;
            CRoom = (c.CRoom == null)
                ? new CChatRoom(chatRoomNr, Guid.NewGuid(), DateTime.MinValue, DateTime.MinValue)
                : new CChatRoom(chatRoomNr, c.CRoom.ChatRuid, c.CRoom.LastPushed, c.CRoom.LastPushed) { TicksLong = c.CRoom.TicksLong };
            
            
            // ClientIp = c.ClientIp ?? null;
        }

        #endregion constructors

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
                this.SerializedMsg = JsonConvert.SerializeObject(this);
                return this.SerializedMsg;
            }
            throw new CqrException($"EncryptToJson(string severKey) failed for CContact.");
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

        public new CContact DecryptFromJsonFromBytes(string serverKey, byte[] serializedBytes)
        {
            string serialized = Encoding.UTF8.GetString(serializedBytes);
            return DecryptFromJson(serverKey, serialized);
        }


        public new CContact DecryptFromJson(string serverKey, string serialized = "")
        {
            if (string.IsNullOrEmpty(serialized))
                serialized = this.SerializedMsg;

            CContact contact = FromJson<CContact>(serialized);
            if (contact != null && Decrypt(serverKey))
            {
                contact._message = _message;
                contact.CBytes = CBytes;
                contact.Md5Hash = Md5Hash;
                contact._hash = Hash;
                contact.Cuid = Cuid;
                contact.Email = Email;
                contact.ContactId = ContactId;
                contact.Address = Address;
                contact.ContactImage = ContactImage;
                contact.CRoom = new CChatRoom(CRoom);
                contact.Mobile = Mobile;
                contact.SerializedMsg = JsonConvert.SerializeObject(this);
                return contact;
            }
            throw new CqrException($"DecryptFromJson<T>(string severKey, string serialized) failed for CContact");
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
            // CqrContact cqrContact = new CqrContact(ContactId, Cuid, Name, Email, Mobile, Address, ContactImage);
            string jsonString = JsonConvert.SerializeObject(this, Formatting.Indented);
            this.SerializedMsg = jsonString;
            return jsonString;
        }

        public override T FromJson<T>(string jsonText)
        {
            T tt = default(T);
            try
            {
                tt = JsonConvert.DeserializeObject<T>(jsonText);
                if (tt != null && tt is CContact contactJson)
                {
                    if (contactJson != null && contactJson.ContactId > -1 && !string.IsNullOrEmpty(contactJson.Name))
                    {
                        ContactId = contactJson.ContactId;
                        Cuid = contactJson.Cuid;
                        Name = contactJson.Name;
                        Email = contactJson.Email;
                        Mobile = contactJson.Mobile;
                        Address = contactJson.Address;
                        ContactImage = contactJson.ContactImage;

                        CRoom = new CChatRoom(contactJson.CRoom);                        

                        _message = contactJson._message;
                        _hash = contactJson._hash;
                        SerializedMsg = contactJson.SerializedMsg;
                        MsgType = contactJson.MsgType;
                        Md5Hash = contactJson.Md5Hash;

                        return (T)tt;
                    }
                }
            }
            catch (Exception exJson)
            {
                SLog.Log(exJson);
            }

            return (T)tt;

        }


        public override string ToXml() => this.ToXml();

        public override T FromXml<T>(string xmlText)
        {
            T cqrT = base.FromXml<T>(xmlText);
            if (cqrT is CContact cCnt)
            {
                ContactId = cCnt.ContactId;
                Cuid = cCnt.Cuid;
                Name = cCnt.Name;
                Email = cCnt.Email;
                Mobile = cCnt.Mobile;
                Address = cCnt.Address;
                ContactImage = cCnt.ContactImage;

                CRoom = new CChatRoom(cCnt.CRoom);

                _message = cCnt._message;
                _hash = cCnt._hash ?? string.Empty;
                SerializedMsg = cCnt.SerializedMsg;
                MsgType = cCnt.MsgType;
                Md5Hash = cCnt.Md5Hash;

            }

            return cqrT;

        }


        public override string ToString()
        {
            return
                "\"ContactId\": \t\"" + ContactId + "\";" + Environment.NewLine +
                "\"Cuid\": \t\"" + Cuid + "\";" + Environment.NewLine +
                "\"Name\": \t\"" + Name + "\";" + Environment.NewLine +
                "\"Email\": \t\"" + Email ?? "" + "\";" + Environment.NewLine +
                "\"Mobile\": \t\"" + Mobile ?? "" + "\";" + Environment.NewLine +
                "\"Address\": \t\"" + Address ?? "" + "\";" + Environment.NewLine +
                "\"NameEmail\": \t\"" + NameEmail ?? "" + "\";" + Environment.NewLine +
                ((ContactImage != null) ?
                    "\"ContactImage\": \t" + Environment.NewLine +
                    "\"ContactImage.ImageFileName\": \t\"" + ContactImage.ImageFileName + "\";" + Environment.NewLine +
                    "\"ContactImage.ImageMimeType\": \t\"" + ContactImage.ImageMimeType + "\";" + Environment.NewLine +
                    "\"ContactImage.ImageBase64\": \t\"" + ContactImage.ImageBase64 + "\";" + Environment.NewLine
                    : "")
                ;
        }

        /// <summary>
        /// <see cref="object[]">RowParams</see> gets an object array of row parameters to show in <see cref="DataGridView"/>
        /// </summary>
        public object[] GetRowParams()
        {
            List<object> oList = new List<object>();
            oList.Add(ContactId);
            oList.Add(Name);
            oList.Add(Email);
            oList.Add(Mobile);
            oList.Add(Address);
            return oList.ToArray();
        }

        #endregion members

    }
}
