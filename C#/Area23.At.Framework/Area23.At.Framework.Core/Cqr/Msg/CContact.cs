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
using static QRCoder.Core.PayloadGenerator.SwissQrCode;

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

        public CImage? ContactImage { get; set; }

        public string NameEmail { get => string.IsNullOrEmpty(Email) ? Name : $"{Name} <{Email}>"; }

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
            SecretKey = string.Empty;
        }

        public CContact(Guid guid, string name, string email, string mobile, string address) : base()
        {
            Cuid = guid;
            Name = name;
            Email = email;
            Mobile = mobile;
            Address = address;
            SecretKey = string.Empty;            
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

        public CContact(CContact ccntct, string hash)
            : this(ccntct.ContactId, ccntct.Cuid, ccntct.Name, ccntct.Email, ccntct.Mobile, ccntct.Address, ccntct.ContactImage, hash)
        {
            CCopy(this, ccntct);
            this._hash = hash;             
            SerializedMsg = "";
            SerializedMsg = this.ToJson();
        }

        public CContact(CContact ccntct, string ChatRoomNr, string hash) : this(ccntct, hash)
        {
            CCopy(this, ccntct);
            _hash = hash;
            ContactImage = null;
            Cuid = (ccntct.Cuid == Guid.Empty) ? Guid.NewGuid() : ccntct.Cuid;            
            _message = ChatRoomNr;              
            SerializedMsg = "";
            SerializedMsg = this.ToJson();
        }

        public CContact(CContact ccntct)
        {
            CCopy(this, ccntct);            
            Cuid = (ccntct.Cuid == Guid.Empty) ? Guid.NewGuid() : ccntct.Cuid;
            SerializedMsg = "";
            SerializedMsg = this.ToJson();
        }

        public CContact(CContact ccntct, string chatRoomNr, string hash, CImage cqrImage) : this(ccntct, chatRoomNr, hash)
        {
            CCopy(this, ccntct);
            Cuid = (ccntct.Cuid == Guid.Empty) ? Guid.NewGuid() : ccntct.Cuid;
            _hash = hash;
            ContactImage = cqrImage;
            _message = chatRoomNr;
            SerializedMsg = "";
            SerializedMsg = this.ToJson();      
        }

        #endregion constructors

        public new CContact? CCopy(CContact? leftDest, CContact? rightSrc)
        {
            if (rightSrc == null)
                return null;
            if (leftDest == null)
                leftDest = new CContact(rightSrc);

            base.CCopy((CContent)leftDest, (CContent)rightSrc);

            leftDest._hash = rightSrc._hash;
            leftDest._message = rightSrc._message;
            leftDest.MsgType = rightSrc.MsgType;
            leftDest.CBytes = rightSrc.CBytes;
            leftDest.Md5Hash = rightSrc.Md5Hash;


            leftDest.ContactId = rightSrc.ContactId;
            leftDest.Cuid = rightSrc.Cuid;
            leftDest.Name = rightSrc.Name;
            leftDest.Email = rightSrc.Email;
            leftDest.Mobile = rightSrc.Mobile;
            leftDest.Address = rightSrc.Address;
            leftDest.SecretKey = rightSrc.SecretKey;
            leftDest.ContactImage = (rightSrc.ContactImage == null) ? null : new CImage(rightSrc.ContactImage.ToDrawingBitmap(), rightSrc.ContactImage.ImageFileName);
            
            leftDest.SerializedMsg = "";
            leftDest.SerializedMsg = leftDest.ToJson();
            
            return leftDest;

        }


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
                _hash = symmPipe.PipeString;
                Md5Hash = MD5Sum.HashString(String.Concat(serverKey, _hash, symmPipe.PipeString, _message), "");

                byte[] msgBytes = EnDeCodeHelper.GetBytesFromString(Message);
                byte[] cqrbytes = LibPaths.CqrEncrypt ? symmPipe.MerryGoRoundEncrpyt(msgBytes, serverKey, hash) : msgBytes;

                CBytes = cqrbytes;
                _message = Base64.ToBase64(CBytes);
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

            var ctc = this.FromJson<CContact>(serialized);
            CContact? contact = JsonConvert.DeserializeObject<CContact>(serialized); 
            
            if (contact != null && contact.Decrypt(serverKey))
            {
                contact.SerializedMsg = "";
                contact.SerializedMsg = contact.ToJson();
                CCopy(this, contact);                
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


                if (!_hash.Equals(symmPipe.PipeString))
                    throw new CqrException($"Hash: {_hash} doesn't match symmPipe.PipeString: {symmPipe.PipeString}");
                string md5Hash = MD5Sum.HashString(String.Concat(serverKey, _hash, symmPipe.PipeString, decrypted), "");
                if (!md5Hash.Equals(Md5Hash))
                    throw new CqrException($"md5Hash: {md5Hash} doesn't match property Md5Hash: {Md5Hash}");

                _message = decrypted;
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

        #region members

        public override string ToJson()
        {
            this.SerializedMsg = "";
            string jsonString = JsonConvert.SerializeObject(this, Formatting.Indented);
            this.SerializedMsg = jsonString;
            return jsonString;
        }

        public new T? FromJson<T>(string jsonText)
        {
            T? tt = JsonConvert.DeserializeObject<T>(jsonText);
            if (tt != null && tt is CContact contactJson)
            {
                if (contactJson != null && contactJson.ContactId > -1 && !string.IsNullOrEmpty(contactJson.Name))
                {
                    CCopy(this, contactJson); 
                }
            }

            return tt;
        }


        public override string ToXml()
        {
            SerializedMsg = "";
            string xmlString = Utils.SerializeToXml<CContact>(this);
            SerializedMsg = xmlString;
            return xmlString;
        }

        public new T? FromXml<T>(string xmlText)
        {
            T? cqrT = base.FromXml<T>(xmlText);
            if (cqrT is CContact cCnt)
            {
                CCopy(this, cCnt);
            }

            return (T?)cqrT;

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
