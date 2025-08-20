using Area23.At.Framework.Core;
using Area23.At.Framework.Core.Cache;
using Area23.At.Framework.Core.Cqr;
using Area23.At.Framework.Core.Cqr.Msg;
using Area23.At.Framework.Core.Static;
using Area23.At.Framework.Core.Util;
using System.Runtime.InteropServices;
using EU.CqrXs.Srv.Svc.Swashbuckle.Util;
using System.Reflection;
using Microsoft.Ajax.Utilities;
using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace EU.CqrXs.Srv.Svc.Swashbuckle.Controllers
{


    [Route("[controller]")]
    [ApiController]
    public class CqrSrvControllerBase : ControllerBase
    {

        protected internal static HashSet<CContact> _contacts;
        protected internal CContact? _contact;
        protected internal CqrFacade cqrFacade;
        protected internal string _serverKey = string.Empty;
        protected internal string _decrypted = string.Empty, _encrypted = string.Empty;
        protected internal string _responseString = string.Empty;
        protected internal string _chatRoomNumber = string.Empty;
        
        protected internal static bool useAWSCache = false, useAppState = true;
        

        /// <summary>
        /// BaseWebService
        /// </summary>
        public CqrSrvControllerBase() : base()
        {            
            _contacts = new HashSet<CContact>();
            _decrypted = "";
            _responseString = string.Empty;
            _contact = null;
            _serverKey = Constants.AUTHOR_EMAIL;
            // GetServerKey();
            cqrFacade = new CqrFacade(_serverKey);
         
            InitMethod();            
        }


        [HttpGet]
        public static void InitMethod()
        {
            _contacts = JsonContacts.GetContacts();

        }

        //public virtual string GetIPAddress()
        //{
        //    throw new InvalidProgramException("HttpContext.Current doesn't exist in .Net Core");
        //}


        //[HttpGet]
        //protected static string GetServerKey()
        //{
        //    // _serverKey = Constants.AUTHOR_EMAIL;            
        //    _serverKey = Constants.VALKEY_CACHE_HOST_PORT;

        //    return _serverKey;
        //}


        /// <summary>
        /// Generates a chat room with a new ChatRoomNr, containing sender and recpients
        /// </summary>
        /// <param name="cSrvMsg"><see cref="CSrvMsg{string}"/></param>
        /// <returns><see cref="CSrvMsg{string}"/></returns>
        [HttpGet]
        internal CSrvMsg<string> InviteToChatRoom(CSrvMsg<string> cSrvMsg)
        {
            string chatRoomNr = string.Empty;
            DateTime now = DateTime.Now; // now1 = now.AddMilliseconds(10);
            List<CContact> _invited = new List<CContact>();

            string restMail = cSrvMsg.Sender.Email.Contains("@") ? (cSrvMsg.Sender.Email.Substring(0, cSrvMsg.Sender.Email.IndexOf("@"))) : cSrvMsg.Sender.Email.Trim();
            restMail = restMail.Replace("@", "_").Replace(".", "_");

            if (!string.IsNullOrEmpty(restMail))
                chatRoomNr = string.Format("room_{0:MMddHHmm}_{1}.json", DateTime.Now, restMail);
            else
                chatRoomNr = string.Format("room_{0:MMddHHmm}.json", DateTime.Now);

            Dictionary<long, string> dict = new Dictionary<long, string>();
            dict.Add(now.Ticks, "");

            if (cSrvMsg.CRoom == null)
                cSrvMsg.CRoom = new CChatRoom(chatRoomNr, Guid.NewGuid(), now, now) { TicksLong = dict.Keys.ToList() };
            else
            {
                cSrvMsg.CRoom.ChatRoomNr = chatRoomNr;
                cSrvMsg.CRoom.ChatRuid = (cSrvMsg.CRoom.ChatRuid == Guid.Empty) ? Guid.NewGuid() : cSrvMsg.CRoom.ChatRuid;
                cSrvMsg.CRoom.LastPolled = now;
                cSrvMsg.CRoom.LastPushed = now;
                if (cSrvMsg.CRoom.TicksLong == null || cSrvMsg.CRoom.TicksLong.Count == 0)
                    cSrvMsg.CRoom.TicksLong = dict.Keys.ToList();
                else
                    cSrvMsg.CRoom.TicksLong.Add(now.Ticks);
            }

            cSrvMsg.Message = (cSrvMsg.CRoom != null && !string.IsNullOrEmpty(cSrvMsg.CRoom.ChatRoomNr)) ? cSrvMsg.CRoom.ChatRoomNr : "";


            bool addSender = true;
            foreach (CContact cr in cSrvMsg.Recipients)
            {
                cr.Message = (cSrvMsg.CRoom != null && !string.IsNullOrEmpty(cSrvMsg.CRoom.ChatRoomNr)) ? cSrvMsg.CRoom.ChatRoomNr : "";

                _invited.Add(cr);
                if ((!string.IsNullOrEmpty(cr.NameEmail) && cr.NameEmail == cSrvMsg.Sender.NameEmail) ||
                    (cr.Cuid != null && cr.Cuid != Guid.Empty && cr.Cuid == cSrvMsg.Sender.Cuid))
                    addSender = false;
            }
            if (addSender)
                _invited.Add(cSrvMsg.Sender);

            SetCachedMessageDict(chatRoomNr, dict);


            CSrvMsg<string> cChatRSrvMsg = JsonChatRoom.SaveChatRoom(ref cSrvMsg);
            _chatRoomNumber = cChatRSrvMsg.CRoom.ChatRoomNr;
            cChatRSrvMsg.Message = _chatRoomNumber;
            JsonChatRoom.AddJsonChatRoomToCache(_chatRoomNumber);

            // serialize chat room in msg later then saving
            // cChatRSrvMsg.SerializedMsg = cChatRSrvMsg.ToJson();

            return cChatRSrvMsg;
        }

        //[HttpGet]
        //public CSrvMsg<string> AddLastDatePushed(CSrvMsg<string> chatRoomMsg, long tickIndex)
        //{
        //    DateTime date = new DateTime(tickIndex);
        //    chatRoomMsg.CRoom.LastPushed = date;

        //    return chatRoomMsg;
        //}




        ///// <summary>
        ///// AddLastDate adds lastPolled or lastPushed date and tickIndex to TicksLong
        ///// </summary>
        ///// <param name="chatRoomMsg"><see cref="CSrvMsg{string}"/> chat room msg to be returned to chat client app</param>
        ///// <param name="tickIndex">tick long index</param>
        ///// <param name="pushed">false for poolled, true for pushed</param>
        ///// <returns><see cref="CSrvMsg{string}"/></returns>
        //public CSrvMsg<string> AddLastDate(CSrvMsg<string> chatRoomMsg, long tickIndex, bool pushed = false)
        //{
        //    DateTime date = new DateTime(tickIndex);
        //    if (pushed)
        //    {
        //        chatRoomMsg.CRoom.LastPushed = date;
        //    }
        //    else
        //    {
        //        chatRoomMsg.CRoom.LastPolled = date;
        //        if (!chatRoomMsg.CRoom.TicksLong.Contains(tickIndex))
        //            chatRoomMsg.CRoom.TicksLong.Add(tickIndex);
        //    }

        //    return chatRoomMsg;
        //}

        /// <summary>
        /// GetCachedMessageDict returns one chat room message dictionary
        /// either from Application State in proc or from Valkey Elastic Cache on AWS
        /// </summary>
        /// <param name="chatRoomNumber">json chat room number</param>
        /// <returns>one chat room message dictionary</returns>
        [HttpGet]
        public static Dictionary<long, string> GetCachedMessageDict(string chatRoomNumber)
        {
            Dictionary<long, string> dict = new Dictionary<long, string>();

            dict = (Dictionary<long, string>)MemoryCache.CacheDict.GetValue<Dictionary<long, string>>(chatRoomNumber);

            return dict;
        }

        /// <summary>
        /// GetNewMessageIndices get all chat room indices, 
        /// which are newer than last <see cref="CContact.LastPolled">polling date of user</see>
        /// or user hasn't read and that are not in list <see cref="CqrContact.TicksLong"></see>
        /// </summary>
        /// <param name="dictKeys"><see cref="DateTime.Ticks"/> as index key of chat room message dictionary</param>
        /// <param name="cSrvMsg"><see cref=CSrvMsg{string}"/></param>
        /// <returns><see cref="List{long}">key indices of messages, that are new and not already polled</see></returns>
        [HttpGet]
        public static List<long> GetNewMessageIndices(List<long> dictKeys, CSrvMsg<string> cSrvMsg)
        {

            List<long> pollKeys = new List<long>();
            foreach (long tickIndex in dictKeys)
            {
                // if (tickIndex > sender.LastPolled.Ticks)
                if (!cSrvMsg.CRoom.TicksLong.Contains(tickIndex))
                    pollKeys.Add(tickIndex);
            }

            return pollKeys;
        }

        /// <summary>
        /// SetCachedMessageDict saves the mesage dictionary for chat room in 
        /// either application state in proc or Amazon Valkey Elastic cache
        /// </summary>
        /// <param name="chatRoomNumber">json chat room number</param>
        /// <param name="dict">the mesage dictionary for chat room </param>
        [HttpGet]
        public static void SetCachedMessageDict(string chatRoomNumber, Dictionary<long, string> dict)
        {

            MemoryCache.CacheDict.SetValue<Dictionary<long, string>>(chatRoomNumber, dict);
            return;
        }


        [HttpGet]
        public static string GetDateNow()
        {
            return DateTime.Now.ToString("yyyy-MM-dd_HH:mm:ss.fff");
        }
    }
}
