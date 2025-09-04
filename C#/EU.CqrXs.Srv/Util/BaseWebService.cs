using Area23.At.Framework.Library.Cache;
using Area23.At.Framework.Library.Cqr;
using Area23.At.Framework.Library.Cqr.Msg;
using Area23.At.Framework.Library.Crypt.Cipher.Symmetric;
using Area23.At.Framework.Library.Static;
using Area23.At.Framework.Library.Util;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Services;

namespace EU.CqrXs.Srv.Util
{

    /// <summary>
    /// BaseWebService
    /// </summary>
    [WebService(Namespace = "https://srv.cqrxs.eu/v1.6/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class BaseWebService : WebService
    {

        protected internal static HashSet<CContact> _contacts;

        protected internal bool _isValid = false;
        protected internal string _serverKey = "", _responseString = "", _chatRoomNumber = "", _decrypted = "";
        protected internal CContact _contact;
        protected internal SymmCipherPipe _symmPipe;


        /// <summary>
        /// BaseWebService
        /// </summary>
        public BaseWebService()
        {
            _contacts = new HashSet<CContact>();
            InitMethod();
        }


        /// <summary>
        /// InitMethod inits all global service variables
        /// </summary>
        public virtual void InitMethod()
        {
            _contacts = JsonContacts.GetContacts(true);
            _contact = null;
            _isValid = false;

            _serverKey = GetServerKey();            
            _decrypted = "";
            _responseString = "";
            _chatRoomNumber = "";

            _symmPipe = new SymmCipherPipe(_serverKey);
            
        }


        /// <summary>
        /// GetServerKey gets server key for encryption: <see cref="HttpRequest.UserHostAddress">UserHostAddress</see> + <see cref="Constants.APP_NAME">APP_NAME</see>
        /// <see cref="HttpRequest.UserHostAddress">UserHostAddress</see> can be overwritten for debugging with Web.Config key <see cref="Constants.EXTERNAL_CLIENT_IP">"ExternalClientIP"</see>
        /// </summary>
        /// <returns><see cref="string">string server key</see> to en-/decrypt client messages</returns>
        protected string GetServerKey()
        {
            string serverKey = "";

            if (ConfigurationManager.AppSettings[Constants.EXTERNAL_CLIENT_IP] != null)
                serverKey = (string)ConfigurationManager.AppSettings[Constants.EXTERNAL_CLIENT_IP];
            else
                serverKey = HttpContext.Current.Request.UserHostAddress;

            serverKey += Constants.APP_NAME;

            return serverKey;
        }

        /// <summary>
        /// Generates a chat room with a new ChatRoomNr, containing sender and recpients
        /// </summary>
        /// <param name="cSrvMsg"><see cref="CSrvMsg{string}"/></param>
        /// <returns><see cref="CSrvMsg{string}"/></returns>
        internal CSrvMsg<string> InviteToChatRoom(CSrvMsg<string> cSrvMsg)
        {
            string chatRoomNr = string.Empty;
            DateTime now = DateTime.Now; // now1 = now.AddMilliseconds(10);
            List<CContact> _invited = new List<CContact>();

            string restMail = cSrvMsg.Sender.Email.Contains("@") ? (cSrvMsg.Sender.Email.Substring(0, cSrvMsg.Sender.Email.IndexOf("@"))) : cSrvMsg.Sender.Email.Trim();
            restMail = restMail.Replace("@", "_").Replace(".", "_");

            if (!string.IsNullOrEmpty(restMail))
                chatRoomNr = String.Format("room_{0:MMddHHmm}_{1}.json", DateTime.Now, restMail);
            else
                chatRoomNr = $"room_{DateTime.Now:MMddHHmm}.json";

            Dictionary<long, string> msgDict = new Dictionary<long, string>();
            msgDict.Add(now.Ticks, cSrvMsg.Sender.NameEmail);

            if (cSrvMsg.CRoom == null)
                cSrvMsg.CRoom = new CChatRoom(chatRoomNr, Guid.NewGuid(), now, now) { MsgDict = msgDict };
            else
            {
                cSrvMsg.CRoom.ChatRoomNr = chatRoomNr;
                cSrvMsg.CRoom.ChatRuid = (cSrvMsg.CRoom.ChatRuid == Guid.Empty) ? Guid.NewGuid() : cSrvMsg.CRoom.ChatRuid;
                cSrvMsg.CRoom.LastPolled = now;
                cSrvMsg.CRoom.LastPushed = now;
                if (cSrvMsg.CRoom.MsgDict == null || cSrvMsg.CRoom.MsgDict.Count == 0)
                    cSrvMsg.CRoom.MsgDict = msgDict;
                else
                    cSrvMsg.CRoom.MsgDict.Add(now.Ticks, cSrvMsg.Sender.NameEmail);
            }

            MemoryCache.CacheDict.SetValue<string>(now.Ticks.ToString(), "");   // add empty key to ticks index
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

            foreach (CContact cr in _invited)
            {
                if (cr != null && !string.IsNullOrEmpty(cr.Email))
                    cSrvMsg.CRoom.InvitedEmails.Add(cr.Email);
            }

            // SetCachedChatRoom(chatRoomNr, cSrvMsg.CRoom);

            CSrvMsg<string> cChatRSrvMsg = JsonChatRoom.SaveChatRoom(ref cSrvMsg);
            // string chatRoomNumber = cChatRSrvMsg.CRoom.ChatRoomNr;
            // cChatRSrvMsg.Message = chatRoomNumber;
            // JsonChatRoom.AddJsonChatRoomToCache(chatRoomNumber);

            // serialize chat room in msg later then saving
            // cChatRSrvMsg.SerializedMsg = cChatRSrvMsg.ToJson();

            return cChatRSrvMsg;
        }

        [Obsolete("Use JsonChatRoom.GetCachedChatRoom(string chatRoomNumber) instead", true)]
        public static CChatRoom GetCachedChatRoom(string chatRoomNumber)
        {
            CChatRoom chatRoomt = (CChatRoom)JsonChatRoom.GetCachedChatRoom(chatRoomNumber);

            // TODO: implement filesystem 

            return chatRoomt;

        }

        /// <summary>
        /// GetNewMessageIndices get all chat room indices, 
        /// which are newer than last <see cref="CContact.LastPolled">polling date of user</see>
        /// or user hasn't read and that are not in list <see cref="CContact.TicksLong"></see>
        /// </summary>
        /// <param name="dictKeys"><see cref="DateTime.Ticks"/> as index key of chat room message dictionary</param>
        /// <param name="cSrvMsg"><see cref="CSrvMsg{string}"/></param>
        /// <returns><see cref="List{long}">key indices of messages, that are new and not already polled</see></returns>
        public static List<long> GetNewMessageIndices(List<long> dictKeys, CSrvMsg<string> cSrvMsg)
        {

            Area23Log.LogOriginMsg("CqrService.asmx", $"GetNewMessageIndices(...) " +
                        $"cSrvMsg.CRoom.TicksLong.Count = {cSrvMsg.CRoom.TicksLong.Count}.\r\n");

            List<long> pollKeys = new List<long>();
            foreach (long tickIndex in dictKeys)
            {
                // if (tickIndex > sender.LastPolled.Ticks)
                if (!cSrvMsg.CRoom.TicksLong.Contains(tickIndex))
                {
                    pollKeys.Add(tickIndex);
                    Area23Log.LogOriginMsg("CqrService.asmx",
                        $"GetNewMessageIndices(...) added {tickIndex} to pollKeys count = {pollKeys.Count}.\r\n");
                }

            }

            return pollKeys;
        }

        /// <summary>
        /// SetCachedMessageDict saves the mesage dictionary for chat room in 
        /// either application state in proc or Amazon Valkey Elastic cache
        /// </summary>
        /// <param name="chatRoomNumber">json chat room number</param>
        /// <param name="dict">the mesage dictionary for chat room </param>
        [Obsolete("SetCachedChatRoom is obsolete", true)]
        public static void SetCachedChatRoom(string chatRoomNumber, CChatRoom chatRoom)
        {
            JsonChatRoom.SetCachedChatRoom(chatRoomNumber, chatRoom);
            return;
        }


        /// <summary>
        /// AddLastDate adds lastPolled or lastPushed date and tickIndex to TicksLong
        /// </summary>
        /// <param name="chatRoomMsg"><see cref="CSrvMsg{string}"/> chat room msg to be returned to chat client app</param>
        /// <param name="tickIndex">tick long index</param>
        /// <param name="pushed">false for poolled, true for pushed</param>
        /// <returns><see cref="CSrvMsg{string}"/></returns>
        public CSrvMsg<string> AddLastDate(CSrvMsg<string> chatRoomMsg, long tickIndex, bool pushed = false)
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
        public virtual string GetKey(string key)
        {
            string vlKey = "";
            string testReport = $"{DateTime.Now.Area23DateTimeMilliseconds()}: GetKey({key}) => ";
            try
            {
                InitMethod();
                vlKey = MemoryCache.CacheDict.GetString(key);
                testReport += vlKey;
            }
            catch (Exception ex1)
            {
                testReport += $"\n{DateTime.Now.Area23DateTimeMilliseconds()}: Exception {ex1.GetType()}: {ex1.Message}\n\t{ex1}\n";
            }            

            return string.IsNullOrEmpty(vlKey) ? testReport : vlKey;
        }

        [WebMethod]
        public virtual string TestCache()
        {
            string[] allKeys;
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

            testReport += $"{DateTime.Now.Area23DateTimeMilliseconds()}: Persistence in {PersistInCache.CacheType.ToString()}\n";


            Dictionary<Guid, CContact> dictCacheTest = new Dictionary<Guid, CContact>();
            foreach (CContact c in _contacts)
            {
                if (c != null && c.Cuid != null && c.Cuid != Guid.Empty &&
                    !dictCacheTest.Keys.Contains(c.Cuid))
                {
                    dictCacheTest.Add(c.Cuid, c);
                }
            }
            testReport += $"{DateTime.Now.Area23DateTimeMilliseconds()}: Added {dictCacheTest.Count} count contacts to Dictionary<Guid, CqrContact>...\n";

            try
            {
                testReport += $"{DateTime.Now.Area23DateTimeMilliseconds()}: Getting MemoryCache.CacheDict.AllKeys" + Environment.NewLine;

                allKeys = MemoryCache.CacheDict.AllKeys;
                if (allKeys == null)
                    testReport += $"{DateTime.Now.Area23DateTimeMilliseconds()}: Got null (NULL) keys" + Environment.NewLine;
                else
                {
                    testReport += $"{DateTime.Now.Area23DateTimeMilliseconds()}: Got null {allKeys.Length} keys" + Environment.NewLine;
                    testReport += $"{DateTime.Now.Area23DateTimeMilliseconds()}: AllKeys = [ {string.Join(" ,", allKeys)} ]" + Environment.NewLine;
                }
                testReport += $"{DateTime.Now.Area23DateTimeMilliseconds()}: Preparing to set Dictionary<Guid, CqrContact> in cache." + "\r\n";
                MemoryCache.CacheDict.SetValue<Dictionary<Guid, CContact>>("TestCache", dictCacheTest);
                testReport += $"{DateTime.Now.Area23DateTimeMilliseconds()}: Added serialized json string to cache." + Environment.NewLine;

                Dictionary<Guid, CContact> outdict = (Dictionary<Guid, CContact>)MemoryCache.CacheDict.GetValue<Dictionary<Guid, CContact>>("TestCache");
                testReport += $"{DateTime.Now.Area23DateTimeMilliseconds()}: Got Dictionary<Guid, CqrContact> from cache with {outdict.Keys.Count} keys." + "\r\n";
                foreach (CContact contact in outdict.Values)
                {
                    testReport += $"{DateTime.Now.Area23DateTimeMilliseconds()}: Contact Cuid={contact.Cuid} NameEmail={contact.NameEmail} Mobile={contact.Mobile}." + "\r\n";
                }
                testReport += $"{DateTime.Now.Area23DateTimeMilliseconds()}: Preparing to delete key \"TestCache\":" + "\r\n";
                MemoryCache.CacheDict.RemoveKey("TestCache");
                testReport += $"{DateTime.Now.Area23DateTimeMilliseconds()}: Deleted key \"TestCache\"." + "\r\n";
            }
            catch (Exception ex2)
            {
                testReport += $"{DateTime.Now.Area23DateTimeMilliseconds()}: Exception {ex2.GetType()}: {ex2.Message}\n\t{ex2}\n";
            }
            try
            {
                List<string> chatRooms = MemoryCache.CacheDict.GetValue<List<string>>(Constants.CHATROOMS) ?? new List<string>();
                string[] chatRoomArray = new string[chatRooms.Count];
                Array.Copy(chatRooms.ToArray(), 0, chatRoomArray, 0, chatRooms.Count);

                testReport += $"{DateTime.Now.Area23DateTimeMilliseconds()}:Found {chatRooms.Count} chat room keys in cache." + Environment.NewLine;
                foreach (string room in chatRoomArray)
                {
                    try
                    {
                        CChatRoom ccr = JsonChatRoom.GetCachedChatRoom(room);
                        Dictionary<long, string> dicTest = ccr.MsgDict;
                        if (dicTest != null)
                        {
                            testReport += $"{DateTime.Now.Area23DateTimeMilliseconds()}: chat room {room} with keys {dicTest.Keys.Count} messages." + Environment.NewLine;
                        }
                    }
                    catch (Exception exChatRoom)
                    {
                        testReport += $"{DateTime.Now.Area23DateTimeMilliseconds()}: loading chat room {room} failed. Exception: {exChatRoom.Message}." + Environment.NewLine;
                        Area23Log.LogOriginMsgEx("CqrService", $"Loading chat room {room} failed. ", exChatRoom);
                    }
                }
                MemoryCache.CacheDict.SetValue<List<string>>(Constants.CHATROOMS, chatRooms);
            }
            catch (Exception ex2)
            {
                testReport += $"{DateTime.Now.Area23DateTimeMilliseconds()}: Exception {ex2.GetType()}: {ex2.Message}\n\t{ex2}\n";
            }


            return testReport;
        }

        [WebMethod]
        public virtual string ResetCache()
        {
            string testReport = $"{DateTime.Now.Area23DateTimeMilliseconds()}:ResetCache() started.\n";
            try
            {
                InitMethod();
            }
            catch (Exception ex1)
            {
                testReport += $"{DateTime.Now.Area23DateTimeMilliseconds()}: Exception {ex1.GetType()}: {ex1.Message}\n\t{ex1}\n";
            }

            testReport += $"{DateTime.Now.Area23DateTimeMilliseconds()}: InitMethod() completed.\n";

            testReport += $"{DateTime.Now.Area23DateTimeMilliseconds()}: Persistence in {PersistInCache.CacheType.ToString()}\n";

            try
            {
                if (PersistInCache.CacheType == PersistType.RedisValkey)
                {
                    testReport += $"{DateTime.Now.Area23DateTimeMilliseconds()}: Ready to connect to {ConfigurationManager.AppSettings[Constants.VALKEY_CACHE_HOST_PORT_KEY]}\n";
                    string status = RedisValkeyCache.ValKeyInstance.Status;
                    testReport += $"{DateTime.Now.Area23DateTimeMilliseconds()}: ConnectionMulitplexer.Status = {status}" + Environment.NewLine;
                }
                testReport += $"{DateTime.Now.Area23DateTimeMilliseconds()}: Getting MemoryCache.CacheDict.AllKeys" + Environment.NewLine;

                string[] allKeys = MemoryCache.CacheDict.AllKeys;
                HashSet<string> newKeys = new HashSet<string>(allKeys);

                List<string> chatRooms = MemoryCache.CacheDict.GetValue<List<string>>(Constants.CHATROOMS);
                string[] chatRoomArray = new string[chatRooms.Count];
                Array.Copy(chatRooms.ToArray(), 0, chatRoomArray, 0, chatRooms.Count);

                foreach (string chatRoom in chatRoomArray)
                {
                    Dictionary<long, string> dict = new Dictionary<long, string>();

                    dict = (Dictionary<long, string>)MemoryCache.CacheDict.GetValue<Dictionary<long, string>>(chatRoom);
                    if (dict != null || dict.Count == 0)
                    {
                        chatRooms.Remove(chatRoom);
                        if (newKeys.Contains(chatRoom))
                            newKeys.Remove(chatRoom);
                    }
                    else
                    {
                        if (!newKeys.Contains(chatRoom))
                        {
                            newKeys.Add(chatRoom);
                            MemoryCache.CacheDict.SetValue<Dictionary<long, string>>(chatRoom, dict);
                        }
                    }
                }
                MemoryCache.CacheDict.SetValue<List<string>>(Constants.CHATROOMS, chatRooms);

                allKeys = MemoryCache.CacheDict.AllKeys;
                if (allKeys == null)
                    testReport += $"{DateTime.Now.Area23DateTimeMilliseconds()}: Got null (NULL) keys" + Environment.NewLine;
                else
                {
                    testReport += $"{DateTime.Now.Area23DateTimeMilliseconds()}: Got null {allKeys.Length} keys" + Environment.NewLine;
                    testReport += $"{DateTime.Now.Area23DateTimeMilliseconds()}: AllKeys = [ {string.Join(" ,", allKeys)} ]" + Environment.NewLine;
                }
                foreach (string aKey in allKeys)
                {
                    if (aKey.Equals(Constants.ALL_KEYS, StringComparison.CurrentCultureIgnoreCase) || aKey.Equals(Constants.CHATROOMS, StringComparison.CurrentCultureIgnoreCase) ||
                        (aKey.StartsWith("room", StringComparison.CurrentCultureIgnoreCase) &&
                            aKey.EndsWith(".json", StringComparison.CurrentCultureIgnoreCase) &&
                            chatRooms.Contains(aKey)))
                    {
                        testReport += $"{DateTime.Now.Area23DateTimeMilliseconds()}: Keeping key \"" + aKey + "\":" + "\r\n";
                    }
                    else
                    {
                        testReport += $"{DateTime.Now.Area23DateTimeMilliseconds()}: Preparing to delete key \"" + aKey + "\":" + "\r\n";
                        MemoryCache.CacheDict.RemoveKey(aKey);
                    }
                }

                allKeys = MemoryCache.CacheDict.AllKeys;
                if (allKeys == null)
                    testReport += $"{DateTime.Now.Area23DateTimeMilliseconds()}: Got null (NULL) keys" + Environment.NewLine;
                else
                {
                    testReport += $"{DateTime.Now.Area23DateTimeMilliseconds()}: Got null {allKeys.Length} keys" + Environment.NewLine;
                    testReport += $"{DateTime.Now.Area23DateTimeMilliseconds()}: AllKeys = [ {string.Join(" ,", allKeys)} ]" + Environment.NewLine;
                }
            }
            catch (Exception ex2)
            {
                testReport += $"{DateTime.Now.Area23DateTimeMilliseconds()}: Exception {ex2.GetType()}: {ex2.Message}\n\t{ex2}\n";
            }

            testReport += $"{DateTime.Now.Area23DateTimeMilliseconds()}:ResetCache() finished.\n";

            return testReport;
        }


    }

}