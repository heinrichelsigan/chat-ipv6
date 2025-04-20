using Area23.At.Framework.Core;
using Area23.At.Framework.Core.Win32Api;
using Area23.At.Framework.Core.Static;
using Area23.At.Framework.Core.Util;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.InteropServices;
using Area23.At.Framework.Core.Cache;
using Area23.At.Framework.Core.Cqr.Msg;
using Area23.At.Framework.Core.Cqr;
using EU.CqrXs.Srv.Svc.Swashbuckle.Util;
using System.Reflection;
using Microsoft.Ajax.Utilities;
using System;
using Microsoft.AspNetCore.Authorization;

namespace EU.CqrXs.Srv.Svc.Swashbuckle.Controllers
{


    [Route("[controller]")]
    [ApiController]
    public class CqrSrvControllerBase : ControllerBase
    {

        protected internal static HashSet<CContact> _contacts;
        protected internal CContact? _contact;
        protected internal CqrFacade cqrFacade;
        // protected internal string _literalServerIPv4, _literalServerIPv6;
        protected internal string _serverKey = string.Empty;
        protected internal string _decrypted = string.Empty, _encrypted = string.Empty;
        protected internal string _responseString = string.Empty;
        protected internal string _chatRoomNumber = string.Empty;
        // protected internal ConnectionMultiplexer redis;
        // protected internal ConfigurationOptions options;
        protected internal static bool useAWSCache = false, useAppState = true;
        // protected internal string endpoint = "cqrcachecqrxseu-53g0xw.serverless.eus2.cache.amazonaws.com:6379";
        // protected internal StackExchange.Redis.IDatabase db;

        /// <summary>
        /// Persist encrypted messages in chat rooms in application state
        /// use this option only for testing, because you will you get soon an out of memory error
        /// </summary>
        public static bool PersistMsgInApplicationState
        {
            get => (PersistMsgIn.PersistMsg == PersistType.ApplicationState);
        }

        /// <summary>
        /// Use Amazon elastic cache to persist encrypted messages in chat rooms
        /// Fast option, but expensive, when we have a lot of huge size messages
        /// </summary>
        public static bool PersistMsgInAmazonElasticCache
        {
            get => (PersistMsgIn.PersistMsg == PersistType.AmazonElasticCache);
        }

        /// <summary>
        /// Use file system to encrypted messages in chat rooms
        /// Fast option, but expensive, when we have a lot of huge size messages
        /// </summary>
        public static bool PersistMsgInFileSystem
        {
            get => (PersistMsgIn.PersistMsg == PersistType.FileSystem);
        }

        /// <summary>
        /// BaseWebService
        /// </summary>
        public CqrSrvControllerBase() : base()
        {
            _contacts = new HashSet<CContact>();
            _decrypted = string.Empty;
            _responseString = string.Empty;
            _contact = null;
            _contacts = GetContacts();
            _serverKey = Constants.VALKEY_CACHE_HOST_PORT;
            // GetServerKey();
            cqrFacade = new CqrFacade(_serverKey);
            _decrypted = string.Empty;
            _responseString = string.Empty;


            if (PersistMsgInAmazonElasticCache)
            {
                string status = RedIS.ConnMux.GetStatus();

                //config = new ElastiCacheClusterConfig("cachecqrxseu-53g0xw.serverless.eus2.cache.amazonaws.com", 11211);
                //// ClusterConfigSettings clusterConfig = new ClusterConfigSettings("cachecqrxseu-53g0xw.serverless.eus2.cache.amazonaws.com", 11211);
                //memClient = new MemcachedClient(config);
            }
        }


        [HttpGet]
        public static void InitMethod()
        {

            if (PersistMsgInAmazonElasticCache)
            {
                string status = RedIS.ConnMux.GetStatus();

                //config = new ElastiCacheClusterConfig("cachecqrxseu-53g0xw.serverless.eus2.cache.amazonaws.com", 11211);
                //// ClusterConfigSettings clusterConfig = new ClusterConfigSettings("cachecqrxseu-53g0xw.serverless.eus2.cache.amazonaws.com", 11211);
                //memClient = new MemcachedClient(config);
            }
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

        [HttpGet]
        internal static HashSet<CContact> GetContacts()
        {
            _contacts = JsonContacts.GetContacts();
            return _contacts;
        }

        [HttpGet]
        internal static CContact AddContact(CContact ccontact)
        {
            _contacts = JsonContacts.GetContacts();
            CContact foundCt = JsonContacts.FindContactByNameEmail(_contacts, ccontact);
            if (foundCt != null)
            {
                foundCt.ContactId = ccontact.ContactId;
                if (foundCt.Cuid == null || foundCt.Cuid == Guid.Empty)
                    foundCt.Cuid = Guid.NewGuid();
                if (!string.IsNullOrEmpty(ccontact.Address))
                    foundCt.Address = ccontact.Address;
                if (ccontact.Mobile != null && ccontact.Mobile.Length > 1)
                    foundCt.Mobile = ccontact.Mobile;
                if (ccontact._message != null && !string.IsNullOrEmpty(ccontact._message))
                    foundCt._message = ccontact._message;

                foundCt.ContactImage = null;
            }
            else
            {
                if (ccontact.Cuid == null || ccontact.Cuid == Guid.Empty)
                    ccontact.Cuid = Guid.NewGuid();
                foundCt = new CContact(ccontact, ccontact._message, ccontact._hash);
                foundCt.ContactImage = null;
                foundCt._message = ccontact._message;

                _contacts.Add(foundCt);

            }

            UpdateContact(foundCt);

            return foundCt;
        }

        [HttpGet]
        internal static void UpdateContact(CContact ccontact)
        {
            CContact toAddContact = null;
            if (ccontact == null || string.IsNullOrEmpty(ccontact.Email))
                return;

            string chatRoomNr = (ccontact._message != null && !string.IsNullOrEmpty(ccontact._message)) ? ccontact._message : "";
            HashSet<CContact> contacts = new HashSet<CContact>();

            foreach (CContact ct in _contacts)
            {
                if ((ct.Cuid == ccontact.Cuid && ct.Email.Equals(ccontact.Email, StringComparison.CurrentCultureIgnoreCase) ||
                    ct.NameEmail.Equals(ccontact.NameEmail, StringComparison.CurrentCultureIgnoreCase)))
                {
                    toAddContact = new CContact(ccontact, chatRoomNr, ccontact.Hash);
                    toAddContact.Mobile = ccontact.Mobile;
                    toAddContact.ContactImage = null;
                    toAddContact.Cuid = (ccontact.Cuid != null && ccontact.Cuid != Guid.Empty) ? ccontact.Cuid : Guid.NewGuid();
                    toAddContact._message = chatRoomNr;
                    contacts.Add(toAddContact);
                }
                else
                    contacts.Add(ct);
            }
            _contacts = contacts;
            JsonContacts.SaveJsonContacts(_contacts);
        }

        [HttpGet]
        internal static void UpdateContacts(CContact contact, CSrvMsg<string> chatRoomMsg, string chatRoomNr)
        {
            bool foundCt = false;
            CContact toDelContact = null;
            if (contact == null || string.IsNullOrEmpty(contact.Email))
                return;

            if ((chatRoomMsg.Sender.Cuid == contact.Cuid && chatRoomMsg.Sender.Email.Equals(contact.Email, StringComparison.CurrentCultureIgnoreCase)) ||
                (chatRoomMsg.Sender.NameEmail.Equals(contact.NameEmail, StringComparison.CurrentCultureIgnoreCase)))
            {
                chatRoomMsg.Sender = new CContact(contact, chatRoomNr, contact.Hash);
            }
            for (int i = 0; i < chatRoomMsg.Recipients.Count; i++)
            {
                if ((chatRoomMsg.Recipients.ElementAt(i).Cuid == contact.Cuid &&
                        chatRoomMsg.Recipients.ElementAt(i).Name.Equals(contact.Name, StringComparison.CurrentCultureIgnoreCase)) ||
                    (chatRoomMsg.Recipients.ElementAt(i).Cuid == contact.Cuid &&
                        chatRoomMsg.Recipients.ElementAt(i).Email.Equals(contact.Email, StringComparison.CurrentCultureIgnoreCase)) ||
                    chatRoomMsg.Recipients.ElementAt(i).NameEmail.Equals(contact.NameEmail, StringComparison.CurrentCultureIgnoreCase))
                {
                    toDelContact = chatRoomMsg.Recipients.ElementAt(i);
                    foundCt = true;
                    break;
                }
            }
            if (foundCt)
            {
                if (chatRoomMsg.Recipients.Remove(toDelContact))
                {

                    CContact cToAdd = new CContact(contact, chatRoomNr, contact._hash);
                    cToAdd._message = contact._message;
                    chatRoomMsg.Recipients.Add(cToAdd);
                }
            }

            JsonChatRoom.SaveChatRoom(chatRoomMsg, chatRoomMsg.CRoom);
            // (new JsonChatRoom(_chatRoomNumber)).SaveJsonChatRoom(chatRoomMsg, chatRoomNr);

            foundCt = false;
            GetContacts();
            for (int j = 0; j < _contacts.Count; j++)
            {
                if ((_contacts.ElementAt(j).Cuid == contact.Cuid) &&
                    (_contacts.ElementAt(j).Name.Equals(contact.Name) ||
                        _contacts.ElementAt(j).NameEmail.Equals(contact.NameEmail)))
                {
                    toDelContact = _contacts.ElementAt(j);
                    foundCt = true;
                    break;
                }
            }
            if (foundCt)
            {
                if (_contacts.Remove(toDelContact))
                {
                    CContact c2Add = new CContact(contact, chatRoomNr, contact.Hash);
                    c2Add._message = contact._message;
                    _contacts.Add(c2Add);
                }
            }

            JsonContacts.SaveJsonContacts(_contacts);

        }

        /// <summary>
        /// Generates a chat room with a new ChatRoomNr, containing sender and recpients
        /// </summary>
        /// <param name="cSrvMsg"><see cref="CSrvMsg{string}"/></param>
        /// <returns><see cref="FullSrvMsg{string}"/></returns>
        [HttpGet]
        internal static CSrvMsg<string> InviteToChatRoom(CSrvMsg<string> cSrvMsg)
        {
            string chatRoomNr = string.Empty;
            DateTime now = DateTime.Now; // now1 = now.AddMilliseconds(10);
            List<CContact> _invited = new List<CContact>();

            string restMail = cSrvMsg.Sender.Email.Contains("@") ? (cSrvMsg.Sender.Email.Substring(0, cSrvMsg.Sender.Email.IndexOf("@"))) : cSrvMsg.Sender.Email.Trim();
            restMail = restMail.Replace("@", "_").Replace(".", "_");
            if (string.IsNullOrEmpty(chatRoomNr))
                chatRoomNr = String.Format("room_{0:MMdd}_{1}.json", DateTime.Now, restMail);

            if (string.IsNullOrEmpty(chatRoomNr))
                chatRoomNr = $"room_{DateTime.Now:MMddHHmm}.json";

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
                cSrvMsg.CRoom.TicksLong = dict.Keys.ToList();

            }

            cSrvMsg.Sender._message = chatRoomNr;


            bool addSender = true;
            foreach (CContact cr in cSrvMsg.Recipients)
            {
                cr._message = chatRoomNr;

                _invited.Add(cr);
                if ((!string.IsNullOrEmpty(cr.NameEmail) && cr.NameEmail == cSrvMsg.Sender.NameEmail) ||
                    (cr.Cuid != null && cr.Cuid != Guid.Empty && cr.Cuid == cSrvMsg.Sender.Cuid))
                    addSender = false;
            }
            if (addSender)
                _invited.Add(cSrvMsg.Sender);

            SetCachedMessageDict(chatRoomNr, dict);


            CSrvMsg<string> cChatRSrvMsg = JsonChatRoom.SaveChatRoom(cSrvMsg, cSrvMsg.CRoom);
            chatRoomNr = cChatRSrvMsg.CRoom.ChatRoomNr;
            cChatRSrvMsg._message = chatRoomNr;
            JsonChatRoom.AddJsonChatRoomToCache(chatRoomNr);

            // serialize chat room in msg later then saving
            cChatRSrvMsg.SerializedMsg = cChatRSrvMsg.ToJson();

            return cChatRSrvMsg;
        }

        /// <summary>
        /// ChatRoomCheckPermission
        /// Validates, if a user has permission to poll or to push messages in the chat room by the following steps:
        /// 1. chat room number in encrypted, now decrypted msg from webservice must be ident with chat room number from json file
        /// 2. sender email from  encrypted, now decrypted msg from webservice must much
        /// 2.a. either creator invitor of chat room
        /// 2.b. on of the invited persons in invitation
        /// </summary>
        /// <param name="cSrvMsg"><see cref="CSrvMsg{string}"/> decoded from <see cref="CqrService.CqrService"/> Webservice</param>
        /// <param name="chatRoomMsg"><see cref="CSrvMsg{string}"/> generated from chat room json</param>
        /// <param name="chatRoomNr"><see cref="string"/> chat room number of chat room</param>
        /// <param name="isClosingRequest">default false, only on close, where only creator can close chat room</param>
        /// <returns>true, if person is allowed to push or receive msg from / to chat room</returns>        
        [HttpGet]
        public static bool ChatRoomCheckPermission(CSrvMsg<string> cSrvMsg, string chatRoomNr, bool isClosingRequest = false)
        {
            CContact? aContact = null;
            bool isValid = false;
            if (chatRoomNr.Equals(cSrvMsg.CRoom.ChatRoomNr))
            {
                foreach (var invitedC in cSrvMsg.CRoom.InvitedEmails)
                {
                    if (invitedC == null)
                        continue;
                    if (cSrvMsg.Sender.NameEmail.Contains(invitedC, StringComparison.CurrentCultureIgnoreCase) ||
                        cSrvMsg.Sender.Email.Equals(invitedC))
                    {
                        aContact = cSrvMsg.Sender;
                        isValid = true;
                        return isValid;
                    }
                }
                
                if (!isClosingRequest)
                {
                    foreach (string? iEmail in cSrvMsg.CRoom.InvitedEmails)
                    {
                        if (iEmail == null) continue;
                        if (cSrvMsg.Sender.NameEmail.Contains(iEmail, StringComparison.CurrentCultureIgnoreCase) ||                            
                            cSrvMsg.Sender.Name.Equals(iEmail, StringComparison.CurrentCultureIgnoreCase))
                        {
                            aContact = cSrvMsg.Sender;
                            isValid = true;
                            return isValid;
                        }
                    }
                }
            }

            return isValid;
        }



        /// <summary>
        /// AddLastDate adds lastPolled or lastPushed date and tickIndex to TicksLong
        /// </summary>
        /// <param name="chatRoomMsg"><see cref="CSrvMsg{string}"/> chat room msg to be returned to chat client app</param>
        /// <param name="tickIndex">tick long index</param>
        /// <param name="pushed">false for poolled, true for pushed</param>
        /// <returns><see cref="CSrvMsg{string}"/></returns>
        [HttpGet]
        public static CSrvMsg<string> AddLastDate(CSrvMsg<string> chatRoomMsg, long tickIndex, bool pushed = false)
        {
            DateTime date = new DateTime(tickIndex);
            if (pushed)
            {
                chatRoomMsg.CRoom.LastPushed = date;                
            }
            else
            {
                chatRoomMsg.CRoom.LastPolled = date;
                if (!chatRoomMsg.CRoom.TicksLong.Contains(tickIndex))
                    chatRoomMsg.CRoom.TicksLong.Add(tickIndex);                
            }

            return chatRoomMsg;
        }

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

            // ApplicationState as Cache
            if (PersistMsgInApplicationState && (CacheHashDict.GetValue<Dictionary<long, string>>(chatRoomNumber) != null))
                dict = (Dictionary<long, string>)CacheHashDict.GetValue<Dictionary<long, string>>(chatRoomNumber);

            // Amazon Redis Valkey Cache
            if (PersistMsgInAmazonElasticCache)
            {
                dict = (Dictionary<long, string>)RedIS.ValKey.GetKey<Dictionary<long, string>>(chatRoomNumber);
            }

            // TODO: implement filesystem 

            return dict;

        }

        /// <summary>
        /// GetNewMessageIndices get all chat room indices, 
        /// which are newer than last <see cref="CContact.LastPolled">polling date of user</see>
        /// or user hasn't read and that are not in list <see cref="CContact.TicksLong"></see>
        /// </summary>
        /// <param name="dictKeys"><see cref="DateTime.Ticks"/> as index key of chat room message dictionary</param>
        /// <param name="cSrvMsg"><see cref="CSrvMsg{string}"/></param>
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

            if (CqrSrvControllerBase.PersistMsgInApplicationState)
                CacheHashDict.SetValue<Dictionary<long, string>>(chatRoomNumber, dict);                 
            if (CqrSrvControllerBase.PersistMsgInAmazonElasticCache)
            {
                RedIS.ValKey.SetKey<Dictionary<long, string>>(chatRoomNumber, dict);
            }

            return;
        }

        [HttpGet]
        public static string GetDateNow()
        {
            return DateTime.Now.ToString("yyyy-MM-dd_HH:mm:ss.fff");
        }
    }
}
