using Area23.At.Framework.Library;
using Area23.At.Framework.Library.Cqr;
using Area23.At.Framework.Library.Cqr.Msg;
using Area23.At.Framework.Library.Crypt.EnDeCoding;
using Area23.At.Framework.Library.Net.IpSocket;
using Area23.At.Framework.Library.Static;
using Area23.At.Framework.Library.Util;
using EU.CqrXs.Service.Util;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Web;
using System.Web.Services;
using System.Net;
using Newtonsoft.Json;
using System.Net.NetworkInformation;
using Area23.At.Framework.Library.Cache;
using System.Net.Http.Headers;
using Area23.At.Framework.Library.Crypt.Cipher.Symmetric;


namespace EU.CqrXs.Service
{

    /// <summary>
    /// CqrService offers a simple chat room service with strong encryption
    /// </summary>
    [WebService(Namespace = "https://srv.cqrxs.eu/v1.2/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class CqrService : WebService
    {

        protected internal static HashSet<CContact> _contacts;
        protected internal CContact _contact;
		protected internal SymmCipherPipe _symmPipe;
        protected internal string _serverKey = string.Empty;

        public CqrService()
        {
            InitMethod();
        }

        /// <summary>
        /// InitMethos inits _serverKey,  
        /// </summary>
        public void InitMethod()
        {
            _contacts = JsonContacts.GetContacts(true);
            _serverKey = GetServerKey();
            _symmPipe = new SymmCipherPipe(_serverKey);
            _contact = null;            
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


            CSrvMsg<string> cChatRSrvMsg = JsonChatRoom.SaveChatRoom(cSrvMsg);
            string chatRoomNumber = cChatRSrvMsg.CRoom.ChatRoomNr;
            cChatRSrvMsg.Message = chatRoomNumber;
            JsonChatRoom.AddJsonChatRoomToCache(chatRoomNumber);

            // serialize chat room in msg later then saving
            cChatRSrvMsg.SerializedMsg = cChatRSrvMsg.ToJson();

            return cChatRSrvMsg;
        }

        /// <summary>
        /// Add LastPolled to contact and also to <see cref="CContact.PolledMsgDates"/>
        /// reduces <see cref="CContact.PolledMsgDates"/>, if contact wears a to huge amount of polling history
        /// TODO: implement ring buffer here
        /// </summary>
        /// <param name="contact"><see cref="CContact"/> to modify</param>
        /// <param name="date"></param>
        /// <returns>modified <see cref="CContact"/></returns>
        [Obsolete("CContact has no more chat room", true)]
        public CContact AddPollDate(CContact contact, DateTime date, bool pushed = false)
        {
            if (pushed)
                contact.Md5Hash = date.ToLongDateString();

            return contact;
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

        /// <summary>
        /// GetCachedMessageDict returns one chat room message dictionary
        /// either from Application State in proc or from Valkey Elastic Cache on AWS
        /// </summary>
        /// <param name="chatRoomNumber">json chat room number</param>
        /// <returns>one chat room message dictionary</returns>
        public static Dictionary<long, string> GetCachedMessageDict(string chatRoomNumber)
        {
            Dictionary<long, string> dict = new Dictionary<long, string>();

            dict = (Dictionary<long, string>)MemoryCache.CacheDict.GetValue<Dictionary<long, string>>(chatRoomNumber);

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
        public static void SetCachedMessageDict(string chatRoomNumber, Dictionary<long, string> dict)
        {
            MemoryCache.CacheDict.SetValue<Dictionary<long, string>>(chatRoomNumber, dict);

            return;
        }


        /// <summary>
        /// Send1StSrvMsg sends first registration message of contact
        /// </summary>
        /// <param name="cryptMsg">with sercerkey encrypted message</param>
        /// <returns>with serverkey encrypted responnse of own contact</returns>
        [WebMethod]
        public string Send1StSrvMsg(string cryptMsg)
        {
            Area23Log.LogOriginMsg("CqrService", $"Send1StSrvMsg(string cryptMsg) called.  cryptMsg.Length = {cryptMsg.Length}.\n");
            InitMethod();
            string responseString = "", decrypted = "";
            
            CContact cContact = new CContact() { Hash = _symmPipe.PipeString };

            try
            {
                if (!string.IsNullOrEmpty(cryptMsg) && cryptMsg.Length >= 8)
                {                    
                    _contact = cContact.FromJson<CContact>(cryptMsg);                    
                    decrypted = _contact.ToJson();
                    MemoryCache.CacheDict.SetValue<string>("lastmsg", decrypted);
                    Area23Log.LogOriginMsg("CqrService", $"Contact decrypted successfully: " + decrypted + "\n");                    
                }
            }
            catch (Exception ex)
            {
                CqrException.SetLastException(ex);
                Area23Log.LogOriginMsgEx("CqrService", $"Exception {ex.GetType()} when decrypting contact: {ex.Message}", ex);
            }

            responseString = _contact.EncryptToJson(_serverKey);

            if (!string.IsNullOrEmpty(decrypted) && _contact != null && !string.IsNullOrEmpty(_contact.NameEmail))
            {
                // MemoryCache.CacheDict.SetValue<string>("lastdecrypted", decrypted);                     

                CContact foundCt = JsonContacts.AddContact(_contact);
                responseString = foundCt.EncryptToJson(_serverKey);
            }

            Area23Log.LogOriginMsg("CqrService", $"Send1StSrvMsg(string cryptMsg) finished.  _contact.Cuid = {_contact.Cuid}.\n");
            return responseString;
        }


        /// <summary>
        /// Invites to a chat romm 
        /// with an encrypted <see cref="CSrvMsg<string>"/>
        /// </summary>
        /// <param name="cryptMsg">encrypted <see cref="CSrvMsg<string>"/></param>
        /// <returns>encrypted <see cref="CSrvMsg<string>"/> including chatroom number</returns>
        [WebMethod]             
        public string ChatRoomInvite(string cryptMsg)
        {
            Area23Log.LogOriginMsg("CqrService", "ChatRoomInvite(string cryptMsg) called.  cryptMsg.Length = " + cryptMsg.Length +  ".\n");
            InitMethod();
            string responseString = "", chatRoomNumber = "";

            CSrvMsg<string> cSrvMsg, chatRSrvMsg = new CSrvMsg<string>(cryptMsg, CType.Json) { Hash = _symmPipe.PipeString, SerializedMsg = cryptMsg };
            chatRSrvMsg = chatRSrvMsg.FromJson(cryptMsg);

            try
            {
                if (!string.IsNullOrEmpty(cryptMsg) && cryptMsg.Length >= 8)
                {
                    cSrvMsg = CSrvMsg<string>.FromJsonDecrypt(_serverKey, cryptMsg);    // decrypt CSrvMsg<string>
                    Area23Log.LogOriginMsg("CqrService", "ChatRoomInvite: Serialized cSrvMsg = " + cSrvMsg.ToJson());
                    cSrvMsg = chatRSrvMsg.DecryptFromJson(_serverKey, cryptMsg);        // decrypt CSrvMsg<string>
                    Area23Log.LogOriginMsg("CqrService", "ChatRoomInvite: Serialized cSrvMsg = " + cSrvMsg.ToJson());

                    _contact = JsonContacts.AddContact(cSrvMsg.Sender);                 // add contact from FullSrvMsg<string>   
                    chatRSrvMsg = InviteToChatRoom(cSrvMsg);                            // generate a FullSrvMsg<string> chatserver message by inviting                           
                    chatRoomNumber = chatRSrvMsg.CRoom.ChatRoomNr;                      // set chat room number
                    responseString = chatRSrvMsg.EncryptToJson(_serverKey);             // crypt chatRSrvMsg with _serverKey and serialize as json
                }
            }
            catch (Exception ex)
            {
                CqrException.SetLastException(ex);
                Area23Log.LogOriginMsgEx("CqrService", "ChatRoomInvite(...)", ex);
            }

            Area23Log.LogOriginMsg("CqrService", "ChatRoomInvite(string cryptMsg) finished. ChatRoomNr = " + chatRoomNumber  + ".");
            return responseString;

        }

        /// <summary>
        /// Polls a chat room for new messages
        /// </summary>
        /// <param name="cryptMsg">encrypted <see cref="CSrvMsg{string}"/> with chat room number and last polled dates</param>
        /// <returns>
        /// encrypted <see cref="CSrvMsg{string}"/> including chatroom number 
        /// with encrypted clientmsg with clientkey.
        /// Server doesn't know client key and always delivers encrypted encrypted messages
        /// Server can only read and decrypt outer envelope message encrypted with server key
        /// </returns>
        [WebMethod]
        public string ChatRoomPoll(string cryptMsg)
        {
            Area23Log.LogOriginMsg("CqrService", $"ChatRoomPoll(string cryptMsg) called.  cryptMsg.Length = " + cryptMsg.Length +".\n");
            InitMethod();
            string responseString = "", chatRoomNumber = "";
            bool isValid = false;

            Dictionary<long, string> dict = new Dictionary<long, string>();            
            
            CSrvMsg<string> cSrvMsg, aSrvMsg =  new CSrvMsg<string>(cryptMsg, CType.Json) { Hash = _symmPipe.PipeString, SerializedMsg = cryptMsg };
            aSrvMsg = aSrvMsg.FromJson(cryptMsg);

            try
            {
                if (!string.IsNullOrEmpty(cryptMsg) && cryptMsg.Length >= 8)
                {
                    cSrvMsg = CSrvMsg<string>.FromJsonDecrypt(_serverKey, cryptMsg);        // decrypt FullSrvMsg<string>
                    Area23Log.LogOriginMsg("CqrService", "ChatRoomPoll: Serialized cSrvMsg = " + cSrvMsg.ToJson());
                    cSrvMsg = aSrvMsg.DecryptFromJson(_serverKey, cryptMsg);                // decrypt FullSrvMsg<string>
                    Area23Log.LogOriginMsg("CqrService", "ChatRoomPoll: Serialized cSrvMsg = " + cSrvMsg.ToJson());

                    _contact = cSrvMsg.Sender;
                    chatRoomNumber = (cSrvMsg.CRoom != null && !string.IsNullOrEmpty(cSrvMsg.CRoom.ChatRoomNr)) ? cSrvMsg.CRoom.ChatRoomNr : cSrvMsg.Message;

                    CSrvMsg<string> chatRoomMsg = JsonChatRoom.LoadChatRoom(cSrvMsg);
                    chatRoomMsg = JsonChatRoom.CheckPermission(cSrvMsg, out isValid);

                    if (isValid)
                    {
                        dict = GetCachedMessageDict(chatRoomNumber);
                        List<long> longKeyList = (dict == null || dict.Count < 1) ? new List<long>() : dict.Keys.ToList();
                        List<long> pollKeys = GetNewMessageIndices(longKeyList, cSrvMsg);
                        
                        long polledPtr = -1;
                        if (dict != null && dict.Count > 0 && pollKeys != null && pollKeys.Count > 0)
                        {
                            polledPtr = pollKeys[0];
                            string firstPollClientMsg = dict[polledPtr] ?? "";
                            if (string.IsNullOrEmpty(firstPollClientMsg) && pollKeys.Count > 1)
                            {
                                chatRoomMsg = AddLastDate(chatRoomMsg, polledPtr, false);
                                polledPtr = pollKeys[1];
                                firstPollClientMsg = dict[polledPtr] ?? "";
                            }
                            chatRoomMsg = AddLastDate(chatRoomMsg, polledPtr, false);

                            JsonContacts.UpdateContact(chatRoomMsg.Sender);
                            chatRoomMsg = JsonChatRoom.SaveChatRoom(chatRoomMsg);

                            chatRoomMsg.TContent = firstPollClientMsg;
                        }

                    }

                    responseString = chatRoomMsg.EncryptToJson(_serverKey);        // encrypt chatRoomMsg and json serialize it

                }
            }
            catch (Exception ex)
            {
                CqrException.SetLastException(ex);
                Area23Log.LogOriginMsgEx("CqrService", "ChatRoomPoll(...)", ex);
            }

            Area23Log.LogOriginMsg("CqrService", "ChatRoomPushMessage(string cryptMsg, string chatRoomMembersCrypted) finihed. ChatRoomNr =  " + chatRoomNumber + ".\n");
            return responseString;

        }

        /// <summary>
        /// Pushes a new message for chatroom to the server
        /// </summary>
        /// <param name="cryptMsg">encrypted <see cref="CSrvMsg{string}"/> with chat room number and last polled dates</param>
        /// <param name="chatRoomMembersCrypted">with client key encrypted message, that is stored in proc of server, but server can't decrypt</param>
        /// <returns>encrypted <see cref="CSrvMsg{string}"/> with chat room number and last polled date changed to now</returns>
        [WebMethod]
        public string ChatRoomPush(string cryptMsg)
        {
            Area23Log.LogOriginMsg("CqrService", $"ChatRoomPushMessage(string cryptMsg) called.\n");
            InitMethod();
            string responseString = "", chatRoomNumber = "", cRoomMembersCrypt = "";
            bool isValid = false;
            Dictionary<long, string> dict;

            CSrvMsg<string> cSrvMsg, aSrvMsg = new CSrvMsg<string>(cryptMsg, CType.Json) { Hash = _symmPipe.PipeString, SerializedMsg = cryptMsg };
            aSrvMsg = aSrvMsg.FromJson(cryptMsg);

            CSrvMsg<string> chatRoomMsg = new CSrvMsg<string>();                            // construct an empty message

            try
            {
                if (!string.IsNullOrEmpty(cryptMsg) && cryptMsg.Length >= 8)
                {
                    cSrvMsg = CSrvMsg<string>.FromJsonDecrypt(_serverKey, cryptMsg);        // decrypt FullSrvMsg<string>
                    Area23Log.LogOriginMsg("CqrService", 
                        "ChatRoomPush: Serialized cSrvMsg = " + cSrvMsg.ToJson());
                    cSrvMsg = aSrvMsg.DecryptFromJson(_serverKey, cryptMsg);                // decrypt FullSrvMsg<string>
                    Area23Log.LogOriginMsg("CqrService", 
                        "ChatRoomPush: Serialized cSrvMsg =" + cSrvMsg.ToJson());

                    _contact = cSrvMsg.Sender;
                    chatRoomNumber = (cSrvMsg.CRoom != null &&                              // get chat room number
                        !string.IsNullOrEmpty(cSrvMsg.CRoom.ChatRoomNr)) ? 
                        cSrvMsg.CRoom.ChatRoomNr : "";
                    cRoomMembersCrypt = cSrvMsg.TContent;                                   // set chatRoomMembersCrypted to cSrvMsg.TContent

                    Area23Log.LogOriginMsg("CqrService", "ChatRoomPush: " +
                        "string chatRoomMembersCrypted = cSrvMsg.TContent; \r\n\t" +
                        $"cRoomMembersCrypt len = {cRoomMembersCrypt.Length}.\n");
                    chatRoomMsg = JsonChatRoom.LoadChatRoom(cSrvMsg);                       // Load json chat room from file system json file                                                                                                                  
                    cSrvMsg = JsonChatRoom.CheckPermission(cSrvMsg, out isValid);           // Check sender's permission to access chat room (must be creator or invited)

                    if (isValid)    
                    {
                        DateTime now = DateTime.Now;                                        // Determine DateTime.Now

                        dict = GetCachedMessageDict(chatRoomNumber);                        // Get chatroom message dictionary out of cache

                        dict.Add(now.Ticks, cRoomMembersCrypt);                             // Add new entry to cached chatroom message dictionary with DateTime.Now
                        chatRoomMsg.CRoom.TicksLong.Add(now.Ticks);
                        chatRoomMsg.CRoom.LastPushed = now;
                        SetCachedMessageDict(chatRoomNumber, dict);                         // Saves chatroom msg dict back to cache (Amazon valkey or ApplicationState)

                        // UpdateContact(_contact);        
                        chatRoomMsg.TContent = "";                                          // set TContent empty, because we don't want a same huge response as request                                             
                        chatRoomMsg = JsonChatRoom.SaveChatRoom(chatRoomMsg);               // saves chat room back to json file


                        chatRoomMsg.CRoom.LastPushed = now;
                        chatRoomMsg.CRoom.TicksLong.Remove(now.Ticks);                      // TODO: Delete later, with that, you get your own message in sended queue
                        chatRoomMsg.Sender.Message = chatRoomNumber;                          
                    }
                    else
                        chatRoomMsg.TContent = cSrvMsg.Sender.NameEmail + " has no permission for chat room " + chatRoomNumber;

                    responseString = chatRoomMsg.EncryptToJson(_serverKey);

                }
            }
            catch (Exception ex)
            {
                CqrException.SetLastException(ex);
                Area23Log.LogOriginMsgEx("CqrService", "ChatRoomPush(...)", ex);
            }

            Area23Log.LogOriginMsg("CqrService", $"ChatRoomPush(string cryptMsg, string cRoomMembersCrypt) finished. ChatRoomNr = " + chatRoomNumber + ".\n");
            return responseString;
        }

        /// <summary>
        /// ChatRoomClose
        /// </summary>
        /// <param name="cryptMsg"></param>
        /// <returns></returns>
        [WebMethod]
        public string ChatRoomClose(string cryptMsg)
        {
            Area23Log.LogOriginMsg("CqrService", $"ChatRoomClose(string cryptMsg) started. cryptMsg.Length =  {cryptMsg.Length}.\n");
            InitMethod();
            string responseString = "", chatRoomNumber = "";
            bool isValid = false;

            CSrvMsg<string> cSrvMsg, aSrvMsg = new CSrvMsg<string>(cryptMsg, CType.Json) { Hash = _symmPipe.PipeString, SerializedMsg = cryptMsg };
            aSrvMsg = aSrvMsg.FromJson(cryptMsg);
            List<CContact> _invited = new List<CContact>();

            try
            {
                if (!string.IsNullOrEmpty(cryptMsg) && cryptMsg.Length >= 8)
                {
                    cSrvMsg = CSrvMsg<string>.FromJsonDecrypt(_serverKey, cryptMsg);
                    cSrvMsg = aSrvMsg.DecryptFromJson(_serverKey, cryptMsg);                    // decrypt FullSrvMsg<string>
                    _contact = JsonContacts.AddContact(cSrvMsg.Sender);
                    chatRoomNumber = (cSrvMsg.CRoom != null && !string.IsNullOrEmpty(cSrvMsg.CRoom.ChatRoomNr)) ? cSrvMsg.CRoom.ChatRoomNr : "";

                    CSrvMsg<string> chatRoomMsg = JsonChatRoom.LoadChatRoom(cSrvMsg);
                    cSrvMsg = JsonChatRoom.CheckPermission(cSrvMsg, out isValid, true);

                    if (isValid)
                    {
                        if (JsonChatRoom.DeleteChatRoom(chatRoomNumber))
                        {
                            chatRoomMsg.CRoom = null;
                            chatRoomMsg.Sender.Message = "";
                        }
                    }
                    
                    responseString = chatRoomMsg.EncryptToJson(_serverKey);
                }
            }
            catch (Exception ex)
            {
                CqrException.SetLastException(ex);
                Area23Log.LogOriginMsgEx("CqrService", "ChatRoomClose(...)", ex);
            }

            Area23Log.LogOriginMsg("CqrService", $"ChatRoomClose(string cryptMsg) finished. deleted chat room ChatRoomNr =  " + chatRoomNumber +".\n");

            return responseString;

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

                string[] allKeys = MemoryCache.CacheDict.AllKeys;
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
                        Area23Log.LogOriginMsgEx("CqrService", $"Loading chat room {room} failed. ", exChatRoom);
                    }
                }
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


        [WebMethod]
        public string GetKey(string key)
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



    }


}
