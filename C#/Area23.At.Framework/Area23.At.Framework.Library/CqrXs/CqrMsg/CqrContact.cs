using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Net;
using Area23.At.Framework.Library.Util;
using Area23.At.Framework.Library;
using System.Drawing;
using Area23.At.Framework.Library.CqrMsg;
using Area23.At.Framework.Library.Static;

namespace Area23.At.Framework.Library.CqrXs.CqrMsg
{


    /// <summary>
    /// CqrContact is a contact for CqrJd
    /// </summary>
    [Serializable]
    public class CqrContact : MsgContent, ICqrMessagable
    {

        #region properties

        public int ContactId { get; set; }

        public Guid Cuid { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string Mobile { get; set; }

        public string Address { get; set; }

        public string SecretKey { get; set; }

        public CqrImage ContactImage { get; set; }

        public string NameEmail { get => string.IsNullOrEmpty(Email) ? Name : $"{Name} <{Email}>"; }

        #region from server given properties


        [Newtonsoft.Json.JsonIgnore]
        internal IPAddress ClientIp { get; set; }


        public string ChatRoomId { get; set; }

        public List<DateTime> PolledMsgDates  { get; set; }

        public DateTime LastPolled { get; set; }

        #endregion from server given properties

        #endregion properties

        #region constructors

        public CqrContact() : base()
        {
            ContactId = -1;
            Cuid = Guid.Empty;
            Name = string.Empty;
            Email = string.Empty;
            Mobile = string.Empty;
            Address = string.Empty;
            SecretKey = string.Empty;
            ContactImage = null;
            ChatRoomId = string.Empty;
            LastPolled = DateTime.MinValue;
            PolledMsgDates = new List<DateTime>();
        }

        public CqrContact(string cs, MsgEnum msgArt = MsgEnum.Json)
        {
            FromJson<CqrContact>(cs);
        }

        public CqrContact(int cid, string name, string email, string mobile, string address) : base()
        {
            ContactId = cid;
            Name = name;
            Email = email;
            Mobile = mobile;
            Address = address;
            LastPolled = DateTime.MinValue;
            PolledMsgDates = new List<DateTime>();
            ChatRoomId = string.Empty;
            ClientIp = null;
        }

        public CqrContact(Guid guid, string name, string email, string mobile, string address) : base()
        {
            Cuid = guid;
            Name = name;
            Email = email;
            Mobile = mobile;
            Address = address;
            ChatRoomId = string.Empty;
            LastPolled = DateTime.MinValue;
            PolledMsgDates = new List<DateTime>();
            ClientIp = null;
        }

        public CqrContact(int cid, string name, string email, string mobile, string address, CqrImage cqrImage)
            : this(cid, name, email, mobile, address)
        {
            ContactImage = cqrImage;
        }

        public CqrContact(int cid, Guid cuid, string name, string email, string mobile, string address, CqrImage cqrImage)
            : this(cid, name, email, mobile, address)
        {
            Cuid = cuid;
            ContactImage = cqrImage;
        }

        public CqrContact(int cid, Guid cuid, string name, string email, string mobile, string address, CqrImage cqrImage, string hash)
            : this(cid, cuid, name, email, mobile, address, cqrImage)
        {
            _hash = hash;
        }

        public CqrContact(int cid, string name, string email, string mobile, string address, Image image)
            : this(cid, name, email, mobile, address)
        {
            ContactImage = CqrImage.FromDrawingImage(image);
        }

        public CqrContact(int cid, Guid cuid, string name, string email, string mobile, string address, Image image)
            : this(cid, name, email, mobile, address)
        {
            Cuid = cuid;
            ContactImage = CqrImage.FromDrawingImage(image);
        }

        public CqrContact(int cid, Guid cuid, string name, string email, string mobile, string address, Image image, string hash)
            : this(cid, cuid, name, email, mobile, address, image)
        {
            this._hash = hash;
        }

        public CqrContact(CqrContact c, string hash)
            : this(c.ContactId, c.Cuid, c.Name, c.Email, c.Mobile, c.Address, c.ContactImage, hash)
        {
            this._hash = hash;
            ChatRoomId = c.ChatRoomId;
            LastPolled = c.LastPolled;
            PolledMsgDates = new List<DateTime>((IEnumerable<DateTime>)c.PolledMsgDates);
            ClientIp = c.ClientIp ?? null;
        }

        public CqrContact(CqrContact c, string chatRoomId, string hash) : this(c, hash)
        {
            _hash = hash;
            ContactImage = null;
            Cuid = c.Cuid;
            ChatRoomId = chatRoomId;
            LastPolled = c.LastPolled;
            PolledMsgDates = new List<DateTime>((IEnumerable<DateTime>)c.PolledMsgDates);
            ClientIp = c.ClientIp ?? null;
        }

        public CqrContact(CqrContact c, string chatRoomId, DateTime lastPolled, string hash) : this(c, chatRoomId, hash)
        {
            _hash = hash;
            ContactImage = null;
            Cuid = c.Cuid;
            ChatRoomId = chatRoomId;
            LastPolled = lastPolled;
            PolledMsgDates = new List<DateTime>((IEnumerable<DateTime>)c.PolledMsgDates);
            if (!PolledMsgDates.Contains(LastPolled))
                PolledMsgDates.Add(LastPolled);
            ClientIp = c.ClientIp ?? null;
        }



        #endregion constructors

        #region members

        public override string ToJson()
        {
            // CqrContact cqrContact = new CqrContact(ContactId, Cuid, Name, Email, Mobile, Address, ContactImage);
            string jsonString = JsonConvert.SerializeObject(this, Formatting.Indented);
            this.RawMessage = jsonString;
            return jsonString;
        }

        public override T FromJson<T>(string jsonText)
        {
            T tt = default(T);
            try
            {
                tt = JsonConvert.DeserializeObject<T>(jsonText);
                if (tt != null && tt is CqrContact cqrContactJson)
                {
                    if (cqrContactJson != null && cqrContactJson.ContactId > -1 && !string.IsNullOrEmpty(cqrContactJson.Name))
                    {
                        ContactId = cqrContactJson.ContactId;
                        Cuid = cqrContactJson.Cuid;
                        Name = cqrContactJson.Name;
                        Email = cqrContactJson.Email;
                        Mobile = cqrContactJson.Mobile;
                        Address = cqrContactJson.Address;
                        ContactImage = cqrContactJson.ContactImage;
                        _message = cqrContactJson._message;
                        _hash = cqrContactJson._hash;
                        RawMessage = cqrContactJson.RawMessage;
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
