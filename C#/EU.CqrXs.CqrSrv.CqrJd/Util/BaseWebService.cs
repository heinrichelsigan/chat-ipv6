using Area23.At.Framework.Library.Cache;
using Area23.At.Framework.Library.CqrXs.CqrMsg;
using Area23.At.Framework.Library.Net.WebHttp;
using Area23.At.Framework.Library.Static;
using Area23.At.Framework.Library.Util;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Web;
using System.Web.Services;

namespace EU.CqrXs.CqrSrv.CqrJd.Util
{

    /// <summary>
    /// BaseWebService
    /// </summary>
    [WebService(Namespace = "https://cqrjd.eu/cqrsrv/cqrjd/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class BaseWebService : WebService
    {

        protected internal static HashSet<CqrContact> _contacts;
        protected internal CqrContact _contact;
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
        public BaseWebService()
        {
            InitMethod();
        }

        public virtual void InitMethod()
        {
            _contacts = GetContacts();
            GetServerKey();
            _decrypted = string.Empty;
            _responseString = string.Empty;
            _contact = null;


            if (PersistMsgInAmazonElasticCache)
            {
                string status = RedIs.ConnMux.GetStatus();

                //config = new ElastiCacheClusterConfig("cachecqrxseu-53g0xw.serverless.eus2.cache.amazonaws.com", 11211);
                //// ClusterConfigSettings clusterConfig = new ClusterConfigSettings("cachecqrxseu-53g0xw.serverless.eus2.cache.amazonaws.com", 11211);
                //memClient = new MemcachedClient(config);
            }
        }

        [WebMethod]
        public virtual string TestService()
        {

            string ret = Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().GetName().FullName) +
                Assembly.GetExecutingAssembly().GetName().Version.ToString();

            object[] sattributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
            if (sattributes.Length > 0)
            {
                AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)sattributes[0];
                if (titleAttribute.Title != "")
                    ret = titleAttribute.Title;
            }
            ret += "\n" + Assembly.GetExecutingAssembly().GetName().Version.ToString();

            object[] oattributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
            if (oattributes.Length > 0)
                ret += "\n" + ((AssemblyDescriptionAttribute)oattributes[0]).Description;

            object[] pattributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
            if (pattributes.Length > 0)
                ret += "\n" + ((AssemblyProductAttribute)pattributes[0]).Product;

            object[] crattributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
            if (crattributes.Length > 0)
                ret += "\n" + ((AssemblyCopyrightAttribute)crattributes[0]).Copyright;

            object[] cpattributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
            if (cpattributes.Length > 0)
                ret += "\n" + ((AssemblyCompanyAttribute)cpattributes[0]).Company;


            return ret;

        }

        [WebMethod]
        public virtual string GetIPAddress()
        {
            string userHostAddr = HttpContext.Current.Request.UserHostAddress;
            return userHostAddr;
        }

        [WebMethod]
        public virtual string TestCache()
        {
            string testReport = $"{DateTime.Now.Area23DateTimeMilliseconds()}:TestCache() started.\n";
            try
            {
                InitMethod();
            }
            catch (Exception ex1)
            {
                testReport += $"{DateTime.Now.Area23DateTimeMilliseconds()}: Exception {ex1.GetType()}: {ex1.Message}\n\t{ex1}\n";
            }

            testReport += $"{DateTime.Now.Area23DateTimeMilliseconds()}: InitMethod() completed.\n";

            testReport += $"{DateTime.Now.Area23DateTimeMilliseconds()}: Persistence in {PersistMsgIn.PersistMsg.ToString()}\n";

            Dictionary<Guid, CqrContact> dictCacheTest = new Dictionary<Guid, CqrContact>();
            foreach (CqrContact c in _contacts)
            {
                if (c != null && c.Cuid != null && c.Cuid != Guid.Empty &&
                    !dictCacheTest.Keys.Contains(c.Cuid))
                    dictCacheTest.Add(c.Cuid, c);
            }
            testReport += $"{DateTime.Now.Area23DateTimeMilliseconds()}: Added {dictCacheTest.Count} count contacts to Dictionary<Guid, CqrContact>...\n";
            if (PersistMsgInAmazonElasticCache)
            {
                try
                {
                    testReport += $"{DateTime.Now.Area23DateTimeMilliseconds()}: Ready to connect to {ConfigurationManager.AppSettings[Constants.VALKEY_CACHE_HOST_PORT_KEY]}\n";
                    string status = RedIs.ConnMux.GetStatus();
                    testReport += $"{DateTime.Now.Area23DateTimeMilliseconds()}: ConnectionMulitplexer.Status = {status}" + Environment.NewLine;

                    testReport += $"{DateTime.Now.Area23DateTimeMilliseconds()}: Preparing to set Dictionary<Guid, CqrContact> in cache."+ Environment.NewLine;
                    RedIs.ValKey.SetKey<Dictionary<Guid, CqrContact>>("TestCache", dictCacheTest);                                        
                    testReport += $"{DateTime.Now.Area23DateTimeMilliseconds()}: Added serialized json string to cache." + Environment.NewLine;

                    Dictionary<Guid, CqrContact> outdict = (Dictionary<Guid, CqrContact>)RedIs.ValKey.GetKey<Dictionary<Guid, CqrContact>>("TestCache");                   
                    testReport += $"{DateTime.Now.Area23DateTimeMilliseconds()}: Got Dictionary<Guid, CqrContact> from cache with {outdict.Keys.Count} keys." + Environment.NewLine;
                    foreach (CqrContact contact in outdict.Values)
                    {
                        testReport += $"{DateTime.Now.Area23DateTimeMilliseconds()}: Contact Cuid={contact.Cuid} NameEmail={contact.NameEmail} Mobile={contact.Mobile}" + Environment.NewLine;
                    }

                    List<string> chatRooms = JsonChatRoom.GetJsonChatRoomsFromCache();
                    testReport += $"{DateTime.Now.Area23DateTimeMilliseconds()}:Found {chatRooms.Count} chat room keys in cache." + Environment.NewLine;
                    foreach (string room in chatRooms)
                    {
                        try
                        {
                            Dictionary<long, string> dicTest = GetCachedMessageDict(room);
                            testReport += $"{DateTime.Now.Area23DateTimeMilliseconds()}: chat room {room} with keys {dicTest.Keys.Count} messages." + Environment.NewLine;
                        }
                        catch (Exception exChatRoom)
                        {
                            testReport += $"{DateTime.Now.Area23DateTimeMilliseconds()}: loading chat room {room} failed. Exception: {exChatRoom.Message}." + Environment.NewLine;
                            Area23Log.LogStatic($"Loading chat room {room} failed. ", exChatRoom, "");
                        }
                    }
                }
                catch (Exception ex2)
                {
                    testReport += $"{DateTime.Now.Area23DateTimeMilliseconds()}: Exception {ex2.GetType()}: {ex2.Message}\n\t{ex2}\n";
                }
            }

            return testReport;
        }

        protected string GetServerKey()
        {
            // _serverKey = Constants.AUTHOR_EMAIL;            

            if (ConfigurationManager.AppSettings["ExternalClientIP"] != null)
            {
                _serverKey = (string)ConfigurationManager.AppSettings["ExternalClientIP"];
            }                
            else
                _serverKey = HttpContext.Current.Request.UserHostAddress;
            _serverKey += Constants.APP_NAME;

            return _serverKey;
        }

        internal HashSet<CqrContact> GetContacts()
        {
            _contacts = JsonContacts.GetContacts();
            return _contacts;
        }

        internal CqrContact AddContact(CqrContact cqrContact)
        {
            _contacts = JsonContacts.GetContacts();
            CqrContact foundCt = JsonContacts.FindContactByNameEmail(_contacts, cqrContact);
            if (foundCt != null)
            {
                foundCt.ContactId = cqrContact.ContactId;
                if (foundCt.Cuid == null || foundCt.Cuid == Guid.Empty)
                    foundCt.Cuid = Guid.NewGuid();
                if (!string.IsNullOrEmpty(cqrContact.Address))
                    foundCt.Address = cqrContact.Address;
                if (cqrContact.Mobile != null && cqrContact.Mobile.Length > 1)
                    foundCt.Mobile = cqrContact.Mobile;
                if (!string.IsNullOrEmpty(cqrContact.ChatRoomNr))
                    foundCt.ChatRoomNr = cqrContact.ChatRoomNr;
                if (cqrContact.LastPolled != null && cqrContact.LastPolled > DateTime.MinValue)
                    foundCt.LastPolled = cqrContact.LastPolled;

                //if (cqrContact != null && cqrContact.ContactImage != null &&
                //    !string.IsNullOrEmpty(cqrContact.ContactImage.ImageFileName) &&
                //    cqrContact.ContactImage.ImageBase64 != null &&
                //    !string.IsNullOrEmpty(cqrContact.ContactImage.ImageBase64))
                foundCt.ContactImage = null;
            }
            else
            {
                if (cqrContact.Cuid == null || cqrContact.Cuid == Guid.Empty)
                    cqrContact.Cuid = Guid.NewGuid();
                _contacts.Add(cqrContact);
                foundCt = cqrContact;
                foundCt.ContactImage = null;

            }

            UpdateContact(foundCt);

            return foundCt;
        }

        internal void UpdateContact(CqrContact cqrContact)
        {
            CqrContact toAddContact = null;
            if (cqrContact == null || string.IsNullOrEmpty(cqrContact.Email))
                return;
            HashSet<CqrContact> contacts = new HashSet<CqrContact>();

            foreach (CqrContact ct in _contacts)
            {
                if ((ct.Cuid == cqrContact.Cuid && ct.Email == cqrContact.Email) ||
                    (ct.NameEmail == cqrContact.NameEmail))
                {
                    toAddContact = new CqrContact(cqrContact, cqrContact.ChatRoomNr, cqrContact.Hash);
                    toAddContact.Mobile = cqrContact.Mobile;
                    toAddContact.ContactImage = null;
                    toAddContact.Cuid = (cqrContact.Cuid != null && cqrContact.Cuid != Guid.Empty) ? cqrContact.Cuid : Guid.NewGuid();
                    contacts.Add(toAddContact);
                }
                else
                    contacts.Add(ct);
            }
            _contacts = contacts;
            JsonContacts.SaveJsonContacts(_contacts);
        }

        internal void UpdateContacts(CqrContact contact, FullSrvMsg<string> chatRoomMsg, string chatRoomNr)
        {
            bool foundCt = false;
            CqrContact toDelContact = null;
            if (contact == null || string.IsNullOrEmpty(contact.Email))
                return;

            if ((chatRoomMsg.Sender.Cuid == contact.Cuid && chatRoomMsg.Sender.Email == contact.Email) ||
                (chatRoomMsg.Sender.NameEmail == contact.NameEmail))
            {
                chatRoomMsg.Sender = new CqrContact(contact, chatRoomNr, contact.Hash);
            }
            for (int i = 0; i < chatRoomMsg.Recipients.Count; i++)
            {
                if ((chatRoomMsg.Recipients.ElementAt(i).Cuid == contact.Cuid) &&
                    (chatRoomMsg.Recipients.ElementAt(i).Name.Equals(contact.Name) ||
                    chatRoomMsg.Recipients.ElementAt(i).NameEmail.Equals(contact.NameEmail)))
                {
                    toDelContact = chatRoomMsg.Recipients.ElementAt(i);
                    foundCt = true;
                    break;
                }
            }
            if (foundCt)
                if (chatRoomMsg.Recipients.Remove(toDelContact))
                    chatRoomMsg.Recipients.Add(new CqrContact(contact, chatRoomNr, contact._hash));

            (new JsonChatRoom(_chatRoomNumber)).SaveJsonChatRoom(chatRoomMsg, chatRoomNr);

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
                if (_contacts.Remove(toDelContact))
                    _contacts.Add(new CqrContact(contact, chatRoomNr, contact.Hash));

            JsonContacts.SaveJsonContacts(_contacts);

        }

        /// <summary>
        /// Generates a chat room with a new ChatRoomNr, containing sender and recpients
        /// </summary>
        /// <param name="fullSrvMsg"><see cref="FullSrvMsg{string}"/></param>
        /// <returns><see cref="FullSrvMsg{string}"/></returns>
        internal FullSrvMsg<string> InviteToChatRoom(FullSrvMsg<string> fullSrvMsg)
        {
            string ChatRoomNr = string.Empty;
            DateTime now = DateTime.Now; // now1 = now.AddMilliseconds(10);
            List<CqrContact> _invited = new List<CqrContact>();

            string restMail = fullSrvMsg.Sender.Email.Contains("@") ? (fullSrvMsg.Sender.Email.Substring(0, fullSrvMsg.Sender.Email.IndexOf("@"))) : fullSrvMsg.Sender.Email.Trim();
            restMail = restMail.Replace("@", "_").Replace(".", "_");
            if (string.IsNullOrEmpty(ChatRoomNr))
                ChatRoomNr = String.Format("room_{0:MMdd}_{1}.json", DateTime.Now, restMail);

            if (string.IsNullOrEmpty(ChatRoomNr))
                ChatRoomNr = $"room_{DateTime.Now:MMddHHmm}.json";

            fullSrvMsg.ChatRuid = (fullSrvMsg.ChatRuid == Guid.Empty) ? Guid.NewGuid() : fullSrvMsg.ChatRuid;

            fullSrvMsg.ChatRoomNr = ChatRoomNr;
            fullSrvMsg.ChatRuid = Guid.NewGuid();
            fullSrvMsg.Sender.ChatRoomNr = ChatRoomNr;
            fullSrvMsg.Sender.ChatRuid = fullSrvMsg.ChatRuid;
            fullSrvMsg.LastPushed = now;
            fullSrvMsg.LastPolled = now;


            Dictionary<long, string> dict = new Dictionary<long, string>();
            dict.Add(now.Ticks, "");

            fullSrvMsg.Sender.LastPolled = now;
            fullSrvMsg.Sender.LastPushed = now;
            fullSrvMsg.Sender.TicksLong = dict.Keys.ToList();

            bool addSender = true;
            foreach (CqrContact c in fullSrvMsg.Recipients)
            {
                c.ChatRoomNr = ChatRoomNr;
                c.ChatRuid = fullSrvMsg.ChatRuid;

                _invited.Add(c);
                if ((!string.IsNullOrEmpty(c.NameEmail) && c.NameEmail == fullSrvMsg.Sender.NameEmail) ||
                    (c.Cuid != null && c.Cuid != Guid.Empty && c.Cuid == fullSrvMsg.Sender.Cuid))
                    addSender = false;
            }
            if (addSender)
                _invited.Add(fullSrvMsg.Sender);

            SetCachedMessageDict(ChatRoomNr, dict);

            FullSrvMsg<string> chatRSrvMsg = new FullSrvMsg<string>();
            chatRSrvMsg.Sender = new CqrContact(fullSrvMsg.Sender, fullSrvMsg._hash);
            chatRSrvMsg.Recipients = new HashSet<CqrContact>(_invited);
            chatRSrvMsg._hash = fullSrvMsg._hash;
            chatRSrvMsg.ChatRoomNr = ChatRoomNr;
            chatRSrvMsg.ChatRuid = fullSrvMsg.ChatRuid;
            chatRSrvMsg.LastPushed = now;
            chatRSrvMsg.LastPushed = now;
            chatRSrvMsg.TicksLong = dict.Keys.ToList();
            chatRSrvMsg.MsgType = MsgEnum.Json;            

            chatRSrvMsg = (new JsonChatRoom(ChatRoomNr)).SaveJsonChatRoom(chatRSrvMsg, ChatRoomNr);
            _chatRoomNumber = chatRSrvMsg.ChatRoomNr;
            JsonChatRoom.AddJsonChatRoomToCache(_chatRoomNumber);

            // serialize chat room in msg later then saving
            chatRSrvMsg.RawMessage = chatRSrvMsg.ToJson();
            chatRSrvMsg._message = chatRSrvMsg.ToJson();

            return chatRSrvMsg;
        }

        /// <summary>
        /// ChatRoomCheckPermission
        /// Validates, if a user has permission to poll or to push messages in the chat room by the following steps:
        /// 1. chat room number in encrypted, now decrypted msg from webservice must be ident with chat room number from json file
        /// 2. sender email from  encrypted, now decrypted msg from webservice must much
        /// 2.a. either creator invitor of chat room
        /// 2.b. on of the invited persons in invitation
        /// </summary>
        /// <param name="fullSrvMsg"><see cref="FullSrvMsg{string}"/> decoded from <see cref="CqrService.CqrService"/> Webservice</param>
        /// <param name="chatRoomMsg"><see cref="FullSrvMsg{string}"/> generated from chat room json</param>
        /// <param name="ChatRoomNr"><see cref="string"/> chat room number of chat room</param>
        /// <param name="isClosingRequest">default false, only on close, where only creator can close chat room</param>
        /// <returns>true, if person is allowed to push or receive msg from / to chat room</returns>        
        public bool ChatRoomCheckPermission(FullSrvMsg<string> fullSrvMsg, FullSrvMsg<string> chatRoomMsg, string ChatRoomNr, bool isClosingRequest = false)
        {

            bool isValid = false;
            if (ChatRoomNr.Equals(chatRoomMsg.ChatRoomNr))
            {
                if ((fullSrvMsg.Sender.NameEmail == chatRoomMsg.Sender.NameEmail) ||
                    (!string.IsNullOrEmpty(fullSrvMsg.Sender.Email) && fullSrvMsg.Sender.Email == chatRoomMsg.Sender.Email))
                {
                    _contact = chatRoomMsg.Sender;
                    isValid = true;
                    return isValid;
                }
                if (!isClosingRequest)
                {
                    foreach (CqrContact c in chatRoomMsg.Recipients)
                    {
                        if (fullSrvMsg.Sender.NameEmail == c.NameEmail ||
                            fullSrvMsg.Sender.NameEmail.ToLower().Equals(c.NameEmail.ToLower()) ||
                            fullSrvMsg.Sender.Email == c.Email ||
                            fullSrvMsg.Sender.Email.ToLower().Equals(c.Email.ToLower()))
                        {
                            _contact = c;
                            isValid = true;
                            return isValid;
                        }
                    }
                }
            }

            return isValid;
        }

        /// <summary>
        /// Add LastPolled to contact and also to <see cref="CqrContact.PolledMsgDates"/>
        /// reduces <see cref="CqrContact.PolledMsgDates"/>, if contact wears a to huge amount of polling history
        /// TODO: implement ring buffer here
        /// </summary>
        /// <param name="contact"><see cref="CqrContact"/> to modify</param>
        /// <param name="date"></param>
        /// <returns>modified <see cref="CqrContact"/></returns>
        public CqrContact AddPollDate(CqrContact contact, DateTime date, bool pushed = false)
        {
            if (pushed)
                contact.LastPushed = date;
            else
                contact.LastPolled = date;

            return contact;
        }

        /// <summary>
        /// AddLastDate adds lastPolled or lastPushed date and tickIndex to TicksLong
        /// </summary>
        /// <param name="chatRoomMsg"><see cref="FullSrvMsg{string}"/> chat room msg to be returned to chat client app</param>
        /// <param name="tickIndex">tick long index</param>
        /// <param name="pushed">false for poolled, true for pushed</param>
        /// <returns><see cref="FullSrvMsg{string}"/></returns>
        public FullSrvMsg<string> AddLastDate(FullSrvMsg<string> chatRoomMsg, long tickIndex, bool pushed = false)
        {
            DateTime date = new DateTime(tickIndex);
            if (pushed)
            {
                chatRoomMsg.LastPushed = date;
                // TODO should we add tickindex, when pushing
                chatRoomMsg.Sender.LastPushed = date;
            }
            else
            {
                chatRoomMsg.LastPolled = date;
                if (!chatRoomMsg.TicksLong.Contains(tickIndex))
                    chatRoomMsg.TicksLong.Add(tickIndex);
                chatRoomMsg.Sender.LastPushed = date;
                if (!chatRoomMsg.Sender.TicksLong.Contains(tickIndex))
                    chatRoomMsg.Sender.TicksLong.Add(tickIndex);                
            }

            return chatRoomMsg;
        }

        /// <summary>
        /// GetCachedMessageDict returns one chat room message dictionary
        /// either from Application State in proc or from Valkey Elastic Cache on AWS
        /// </summary>
        /// <param name="chatRoomNumber">json chat room number</param>
        /// <returns>one chat room message dictionary</returns>
        public static Dictionary<long, string> GetCachedMessageDict(string chatRoomNumber)
        {
            Dictionary<long, string> dict = new Dictionary<long, string>();

            // ApplicationState as Cache
            if (PersistMsgInApplicationState && (HttpContext.Current.Application[chatRoomNumber] != null))
                dict = (Dictionary<long, string>)HttpContext.Current.Application[chatRoomNumber];

            // Amazon Redis Valkey Cache
            if (PersistMsgInAmazonElasticCache)
            {
                dict = (Dictionary<long, string>)RedIs.ValKey.GetKey<Dictionary<long, string>>(chatRoomNumber);
            }

            // TODO: implement filesystem 

            return dict;

        }

        /// <summary>
        /// GetNewMessageIndices get all chat room indices, 
        /// which are newer than last <see cref="CqrContact.LastPolled">polling date of user</see>
        /// or user hasn't read and that are not in list <see cref="CqrContact.TicksLong"></see>
        /// </summary>
        /// <param name="dictKeys"><see cref="DateTime.Ticks"/> as index key of chat room message dictionary</param>
        /// <param name="sender"><see cref="CqrContact"/></param>
        /// <returns><see cref="List{long}">key indices of messages, that are new and not already polled</see></returns>
        public static List<long> GetNewMessageIndices(List<long> dictKeys, CqrContact sender)
        {
            
            List<long> pollKeys = new List<long>();
            foreach (long tickIndex in dictKeys)
            {
                // if (tickIndex > sender.LastPolled.Ticks)
                if (!sender.TicksLong.Contains(tickIndex))
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
        public static void SetCachedMessageDict(string chatRoomNumber, Dictionary<long, string> dict)
        {

            if (BaseWebService.PersistMsgInApplicationState)
                HttpContext.Current.Application[chatRoomNumber] = dict;
            if (BaseWebService.PersistMsgInAmazonElasticCache)
            {
                RedIs.ValKey.SetKey<Dictionary<long, string>>(chatRoomNumber, dict);
            }

            return;
        }

    }

}