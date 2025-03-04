﻿using Newtonsoft.Json;
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

namespace Area23.At.Framework.Library.CqrXs.CqrMsg
{


    /// <summary>
    /// Full SrvMsg
    /// </summary>
    /// <typeparam name="TC"></typeparam>
    [Serializable]
    public class FullSrvMsg<TC> : MsgContent, ICqrMessagable where TC : class
    {

        #region properties

        public CqrContact Sender { get; set; }

        public  List<string> Emails
        {
            get
            {
                HashSet<string> mails = new HashSet<string>();
                foreach (CqrContact c in Recipients)
                {
                    if (!mails.Contains(c.Email))
                        mails.Add(c.Email);
                }
                return mails.ToList();
            }
        }

        internal List<Guid> Cuids
        {
            get
            {
                HashSet<Guid> cuids = new HashSet<Guid>();
                foreach (CqrContact c in Recipients)
                {
                    if (!cuids.Contains(c.Cuid))
                        cuids.Add(c.Cuid);
                }
                return cuids.ToList();
            }
        }



        public HashSet<CqrContact> Recipients { get; set; }

        public CqrContact Recipient
        {
            get => (Recipients == null || Recipients.Count < 1) ? null : Recipients.ElementAt(0);
            set
            {
                if (value != null && !string.IsNullOrEmpty(value.NameEmail))
                {
                    if (Recipients == null || Recipients.Count == 0)
                    {
                        Recipients = new HashSet<CqrContact>();
                        Recipients.Add(value);
                    }
                    else
                    {
                        for (int ix = 0; ix < Recipients.Count; ix++)
                        {
                            if (value.Cuid == Recipients.ElementAt(ix).Cuid &&
                                (value.Email == Recipients.ElementAt(ix).Email ||
                                (value.NameEmail == Recipients.ElementAt(ix).NameEmail ||
                                value.ContactId == Recipients.ElementAt(ix).ContactId)))
                            {
                                CqrContact toRemove = Recipients.ElementAt(ix);
                                Recipients.Remove(toRemove);
                                Recipients.Add(value);
                            }
                        }
                    }
                }
            }
        }


        public TC TContent { get; set; }

        public string ChatRoomNr { get; set; } 

        #endregion properties

        #region ctor

        public FullSrvMsg() : base()
        {
            _message = string.Empty;
            RawMessage = string.Empty;
            _hash = string.Empty;
            Sender = null;
            Recipients = new HashSet<CqrContact>();
            Recipient = null;
            TContent = null;
            ChatRoomNr = string.Empty;
        }

        public FullSrvMsg(string fm, MsgEnum msgArt = MsgEnum.Json) : base()
        {
            this.FromJson<FullSrvMsg<TC>>(fm);
        }

        [Obsolete("Always user FullSrvMsg(CqrContact sender, CqrContact to, TC tc, string hash) : base() ctor", false)]
        public FullSrvMsg(CqrContact sender, CqrContact to, TC tc) : base()
        {
            Sender = sender;
            CqrContact[] tos = (to != null) ? new CqrContact[1] { to } : new CqrContact[0];
            Recipients = new HashSet<CqrContact>(tos);
            TContent = tc;
            ChatRoomNr = string.Empty;
        }

        /// <summary>
        /// Please always use this constuctor
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="to"></param>
        /// <param name="tc"></param>
        /// <param name="hash"></param>
        public FullSrvMsg(CqrContact sender, CqrContact to, TC tc, string hash, string chatRoomNr = "") : base()
        {
            Sender = sender;
            CqrContact[] tos = (to != null) ? new CqrContact[1] { to } : new CqrContact[0];
            Recipients = new HashSet<CqrContact>(tos);
            TContent = tc;
            _hash = hash;
            ChatRoomNr = chatRoomNr;
        }


        /// <summary>
        /// Please always use this constuctor
        /// </summary>
        /// <param name="sender">CqrContact</param>
        /// <param name="tos">Array of CqrContact</param>
        /// <param name="tc"></param>
        /// <param name="hash"></param>
        public FullSrvMsg(CqrContact sender, CqrContact[] tos, TC tc, string hash, string chatRoomNr = "") : base()
        {
            Sender = sender;
            Recipients = new HashSet<CqrContact>(tos);
            TContent = tc;
            _hash = hash;
            ChatRoomNr = chatRoomNr;
        }


        #endregion ctor


        public override string ToJson()
        {
            string jsonText = JsonConvert.SerializeObject(this);
            this.RawMessage = jsonText;
            return jsonText;
        }

        public new FullSrvMsg<TC> FromJson(string jsonText) 
        {
            FullSrvMsg<TC> tc = JsonConvert.DeserializeObject<FullSrvMsg<TC>>(jsonText);
            try
            {
                if (tc != null && tc is FullSrvMsg<TC> fullSrvMsg)
                {
                    if (fullSrvMsg != null && !string.IsNullOrEmpty(fullSrvMsg.Message))
                    {
                        Sender = fullSrvMsg.Sender;
                        _hash = fullSrvMsg._hash;
                        Recipients = fullSrvMsg.Recipients;
                        TContent = fullSrvMsg.TContent;
                        ChatRoomNr = fullSrvMsg.ChatRoomNr;
                    }
                    return tc;
                }
            }
            catch (Exception exJson)
            {
                SLog.Log(exJson);
            }

            return default(FullSrvMsg<TC>);
        }

    }

}
