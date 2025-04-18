﻿using Area23.At.Framework.Library.Cqr;
using Area23.At.Framework.Library.Cqr.Msg;
using Area23.At.Framework.Library.Static;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace Area23.At.Framework.Library.Cqr.Msg
{
    [Serializable]
    public class CChatRoom : CContent, IMsgAble
    {
        public Guid ChatRuid { get; set; }

        public string ChatRoomNr { get; set; }

        public List<long> TicksLong { get; set; }

        public DateTime LastPushed { get; set; }

        public DateTime LastPolled { get; set; }

        #region ICqrMessagable interface

        public new CType MsgType => CType.Json;

        //public new string RawMessage { get => ToJson(); set; }

        public new string Hash => _hash;

        // public new string Md5Hash { get => MD5Sum.HashString(_message); set; }

        #endregion ICqrMessagable interface

        public CChatRoom() : base()
        {
            ChatRuid = Guid.NewGuid();
            ChatRoomNr = "";
            _message = "";
            TicksLong = new List<long>();
            LastPushed = DateTime.MinValue;
            LastPolled = DateTime.MinValue;
            _hash = "";
        }


        public CChatRoom(string chatRoomNr, Guid chatRuid, DateTime lastPushed, DateTime lastPolled)  : this()
        {
            ChatRoomNr = chatRoomNr;
            ChatRuid = (chatRuid == Guid.Empty) ? Guid.NewGuid() : chatRuid;
            lastPushed = LastPushed;
            LastPolled = lastPolled;
            TicksLong = new List<long>();
        }



        public CChatRoom(CChatRoom chatRoom) : this()
        {
            if (chatRoom != null)
            {
                ChatRoomNr = chatRoom.ChatRoomNr;
                ChatRuid = chatRoom.ChatRuid;
                TicksLong = chatRoom.TicksLong;
                LastPolled = chatRoom.LastPolled;
                LastPushed = chatRoom.LastPushed; 
            }            
        }

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
                if (tt != null && tt is CChatRoom chatRoom)
                {
                    if (chatRoom != null && chatRoom.ChatRuid != Guid.Empty && !string.IsNullOrEmpty(chatRoom.ChatRoomNr))
                    {
                        ChatRuid = chatRoom.ChatRuid;
                        ChatRoomNr = chatRoom.ChatRoomNr;
                        TicksLong = chatRoom.TicksLong;
                        LastPushed = chatRoom.LastPushed;
                        LastPolled = chatRoom.LastPolled;
                        _message = chatRoom._message;
                        _hash = chatRoom._hash;

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
            if (cqrT is CChatRoom chatRoom && chatRoom != null && chatRoom.ChatRuid != Guid.Empty && !string.IsNullOrEmpty(chatRoom.ChatRoomNr))
            {
                ChatRuid = chatRoom.ChatRuid;
                ChatRoomNr = chatRoom.ChatRoomNr;
                TicksLong = chatRoom.TicksLong;
                LastPushed = chatRoom.LastPushed;
                LastPolled = chatRoom.LastPolled;
                _message = chatRoom._message;
                _hash = chatRoom._hash;


                return (T)cqrT;
            }

            return cqrT;

        }


        public override string ToString()
        {

            return
                "\"ChatRuid\": \t\"" + ChatRuid + "\";" + Environment.NewLine +
                "\"ChatRoomNr\": \t\"" + ChatRoomNr + "\";" + Environment.NewLine +
                "\"TicksLong\": \t\"" + String.Join(",", TicksLong) + "\";" + Environment.NewLine +
                "\"LastPushed\": \t\"" + LastPushed ?? "" + "\";" + Environment.NewLine +
                "\"LastPolled\": \t\"" + LastPolled ?? "" + "\";" + Environment.NewLine +
                "\"_message\": \t\"" + _message ?? "" + "\";" + Environment.NewLine +
                "\"_hash\": \t\"" + _hash ?? "" + "\";" + Environment.NewLine +
                "\"SerializedMsg\": \t\"" + SerializedMsg ?? "" + "\";" + Environment.NewLine +
                "\"Md5Hash\": \t\"" + Md5Hash ?? "" + "\";" + Environment.NewLine;
        }

        #endregion members
    }
}
