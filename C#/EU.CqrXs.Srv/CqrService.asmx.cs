using Area23.At.Framework.Library.Cache;
using Area23.At.Framework.Library.Cqr;
using Area23.At.Framework.Library.Cqr.Msg;
using Area23.At.Framework.Library.Util;
using EU.CqrXs.Srv.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Services;


namespace EU.CqrXs.Srv
{

    /// <summary>
    /// CqrService offers a simple chat room service with strong encryption
    /// </summary>
    [WebService(Namespace = "https://srv.cqrxs.eu/v1.6/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class CqrService : Util.BaseWebService
    {


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
            Area23Log.LogOriginMsg("CqrService", "ChatRoomInvite(string cryptMsg) called.  cryptMsg.Length = " + cryptMsg.Length + ".\n");
            InitMethod();
            string responseString = "", chatRoomNumber = "";
            CSrvMsg<string> cSrvMsg, chatRoomMsg;

            try
            {
                if (!string.IsNullOrEmpty(cryptMsg) && cryptMsg.Length >= 8)
                {
                    cSrvMsg = CSrvMsg<string>.FromJsonDecrypt(_serverKey, cryptMsg);    // decrypt CSrvMsg<string>

                    _contact = JsonContacts.AddContact(cSrvMsg.Sender);                 // add contact from FullSrvMsg<string>   
                    chatRoomMsg = InviteToChatRoom(cSrvMsg);                            // generate a FullSrvMsg<string> chatserver message by inviting                           
                    chatRoomNumber = chatRoomMsg.CRoom.ChatRoomNr;                      // set chat room number
                    responseString = chatRoomMsg.EncryptToJson(_serverKey);             // crypt chatRSrvMsg with _serverKey and serialize as json
                }
            }
            catch (Exception ex)
            {
                CqrException.SetLastException(ex);
                Area23Log.LogOriginMsgEx("CqrService", "ChatRoomInvite(...)", ex);
            }

            Area23Log.LogOriginMsg("CqrService", "ChatRoomInvite(string cryptMsg) finished. ChatRoomNr = " + chatRoomNumber + ".");

            return responseString;
        }

        /// <summary>
        /// Polls all chats from a chat room
        /// </summary>
        /// <param name="cryptMsg">encrypted <see cref="CSrvMsg{string}"/> with chat room number and last polled dates</param>
        /// <returns>
        /// encrypted <see cref="CSrvMsg{List{String}}"/> including chatroom number 
        /// with encrypted clientmsg with clientkey.
        /// Server doesn't know client key and always delivers encrypted encrypted messages
        /// Server can only read and decrypt outer envelope message encrypted with server key
        /// </returns>
        [WebMethod]
        public string ChatPollAll(string cryptMsg)
        {
            Area23Log.LogOriginMsg("CqrService", $"ChatPollAll(string cryptMsg) called.  cryptMsg.Length = " + cryptMsg.Length + ".\n");
            InitMethod();
            string responseString = "", chatRoomNumber = "";
            bool isValid = false;
            Dictionary<long, string> dict = new Dictionary<long, string>();
            CSrvMsg<string> cSrvMsg, chatRoomMsg;
            CSrvMsg<List<string>> allPollMsg;

            try
            {
                if (!string.IsNullOrEmpty(cryptMsg) && cryptMsg.Length >= 8)
                {
                    cSrvMsg = CSrvMsg<string>.FromJsonDecrypt(_serverKey, cryptMsg);        // decrypt FullSrvMsg<string>

                    _contact = cSrvMsg.Sender;
                    chatRoomNumber = (cSrvMsg.CRoom != null && !string.IsNullOrEmpty(cSrvMsg.CRoom.ChatRoomNr)) ? cSrvMsg.CRoom.ChatRoomNr : cSrvMsg.Message;

                    chatRoomMsg = JsonChatRoom.LoadChatRoom(ref cSrvMsg);
                    isValid = JsonChatRoom.CheckPermission(ref cSrvMsg);
                    allPollMsg = new CSrvMsg<List<string>>(cSrvMsg.Sender, cSrvMsg.Recipients.ToArray(), new List<string>(), cSrvMsg.Hash, cSrvMsg.CRoom) { Md5Hash = cSrvMsg.Md5Hash, Message = cSrvMsg.Message, MsgType = cSrvMsg.MsgType };

                    if (isValid)
                    {
                        CChatRoom ccr = JsonChatRoom.GetCachedChatRoom(chatRoomNumber);
                        dict = ccr.MsgDict;

                        List<long> longKeyList = (dict == null || dict.Count < 1) ? new List<long>() : dict.Keys.ToList();
                        List<long> pollKeys = GetNewMessageIndices(longKeyList, cSrvMsg);
                        string firstPollClientMsg = "";
                        long polledPtr = -1;
                        int pollIdx = 0;

                        if (dict != null && dict.Count > 0 && pollKeys != null && pollKeys.Count > 0)
                        {
                            while (pollIdx < pollKeys.Count)
                            {
                                Area23Log.LogOriginMsg("CqrService", "ChatPollAll: " + chatRoomNumber +
                                    $"\r\n\tpollIdx = {pollIdx} to dict\r\n\tpollKeys.Count = {pollKeys.Count} \r\n");

                                polledPtr = pollKeys[pollIdx++];
                                // firstPollClientMsg = dict[polledPtr] ?? "";
                                firstPollClientMsg = MemoryCache.CacheDict.GetValue<string>(polledPtr.ToString()) ?? "";
                                chatRoomMsg.CRoom.LastPolled = new DateTime(polledPtr);
                                allPollMsg.CRoom.LastPolled = new DateTime(polledPtr);

                                if (!string.IsNullOrEmpty(firstPollClientMsg))
                                {
                                    allPollMsg.TContent.Add(firstPollClientMsg);
                                }
                                allPollMsg.CRoom.MsgDict.Add(polledPtr, ccr.MsgDict[polledPtr]);

                                if (ccr.LastPolled < allPollMsg.CRoom.LastPolled)
                                {
                                    ccr.LastPolled = allPollMsg.CRoom.LastPolled;
                                    JsonChatRoom.SetCachedChatRoom(chatRoomNumber, ccr);
                                }
                            }

                            chatRoomMsg.CRoom.MsgDict = ccr.MsgDict;
                            allPollMsg.CRoom.MsgDict = ccr.MsgDict;

                            Area23Log.LogOriginMsg("CqrService", $"ChatPollAll: \r\n\tallPollMsg.TContent.Count = {allPollMsg.TContent.Count}\r\n");

                            JsonContacts.UpdateContact(chatRoomMsg.Sender);
                            chatRoomMsg = JsonChatRoom.SaveChatRoom(ref chatRoomMsg);
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
            Area23Log.LogOriginMsg("CqrService", $"ChatRoomPoll(string cryptMsg) called.  cryptMsg.Length = " + cryptMsg.Length + ".\n");
            InitMethod();
            string responseString = "", chatRoomNumber = "";
            bool isValid = false;
            Dictionary<long, string> dict = new Dictionary<long, string>();
            CSrvMsg<string> cSrvMsg, chatRoomMsg;
            CSrvMsg<List<string>> allPollMsg;

            try
            {
                if (!string.IsNullOrEmpty(cryptMsg) && cryptMsg.Length >= 8)
                {
                    cSrvMsg = CSrvMsg<string>.FromJsonDecrypt(_serverKey, cryptMsg);        // decrypt FullSrvMsg<string>

                    _contact = cSrvMsg.Sender;
                    chatRoomNumber = (cSrvMsg.CRoom != null && !string.IsNullOrEmpty(cSrvMsg.CRoom.ChatRoomNr)) ? cSrvMsg.CRoom.ChatRoomNr : cSrvMsg.Message;

                    chatRoomMsg = JsonChatRoom.LoadChatRoom(ref cSrvMsg);
                    isValid = JsonChatRoom.CheckPermission(ref cSrvMsg);
                    allPollMsg = new CSrvMsg<List<string>>(cSrvMsg.Sender, cSrvMsg.Recipients.ToArray(), new List<string>(), cSrvMsg.Hash, cSrvMsg.CRoom) { Md5Hash = cSrvMsg.Md5Hash, Message = cSrvMsg.Message, MsgType = cSrvMsg.MsgType };

                    if (isValid)
                    {
                        CChatRoom ccr = JsonChatRoom.GetCachedChatRoom(chatRoomNumber);
                        dict = ccr.MsgDict;

                        List<long> longKeyList = (dict == null || dict.Count < 1) ? new List<long>() : dict.Keys.ToList();
                        List<long> pollKeys = GetNewMessageIndices(longKeyList, cSrvMsg);

                        long polledPtr = -1;
                        int pollIdx = 0;
                        string firstPollClientMsg = "";

                        if (dict != null && dict.Count > 0 && pollKeys != null && pollKeys.Count > 0)
                        {
                            while (pollIdx < pollKeys.Count)
                            {
                                Area23Log.LogOriginMsg("CqrService", "ChatRoomPush: " + chatRoomNumber + $"\r\n\tpollIdx = {pollIdx} to dict\r\n\tpollKeys.Count = {pollKeys.Count} \r\n");

                                polledPtr = pollKeys[pollIdx++];
                                firstPollClientMsg = MemoryCache.CacheDict.GetValue<string>(polledPtr.ToString()) ?? "";
                                // firstPollClientMsg = dict[polledPtr] ?? "";                                
                                chatRoomMsg.CRoom.LastPolled = new DateTime(polledPtr);
                                cSrvMsg.CRoom.LastPolled = new DateTime(polledPtr);
                                if (ccr.LastPolled < allPollMsg.CRoom.LastPolled)
                                    ccr.LastPolled = allPollMsg.CRoom.LastPolled;

                                if (!string.IsNullOrEmpty(firstPollClientMsg))
                                {
                                    chatRoomMsg.TContent = firstPollClientMsg;
                                    cSrvMsg.TContent = firstPollClientMsg;
                                    break;
                                }
                            }

                            chatRoomMsg.CRoom = ccr;
                            chatRoomMsg.CRoom.MsgDict = ccr.MsgDict;
                            cSrvMsg.CRoom.MsgDict = ccr.MsgDict;
                        }

                        JsonContacts.UpdateContact(chatRoomMsg.Sender);
                        chatRoomMsg = JsonChatRoom.SaveChatRoom(ref chatRoomMsg);

                    }

                    responseString = chatRoomMsg.EncryptToJson(_serverKey);        // encrypt chatRoomMsg and json serialize it

                }
            }
            catch (Exception ex)
            {
                CqrException.SetLastException(ex);
                Area23Log.LogOriginMsgEx("CqrService", "ChatPollAll(...)", ex);
            }

            Area23Log.LogOriginMsg("CqrService", "ChatPollAll(string cryptMsg) finihed. ChatRoomNr =  " + chatRoomNumber + ".\n");

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
            CSrvMsg<string> cSrvMsg, chatRoomMsg;
            CSrvMsg<List<string>> allPollMsg;

            try
            {
                if (!string.IsNullOrEmpty(cryptMsg) && cryptMsg.Length >= 8)
                {
                    cSrvMsg = CSrvMsg<string>.FromJsonDecrypt(_serverKey, cryptMsg);        // decrypt FullSrvMsg<string>

                    _contact = cSrvMsg.Sender;
                    chatRoomNumber = (cSrvMsg.CRoom != null && !string.IsNullOrEmpty(cSrvMsg.CRoom.ChatRoomNr))
                         ? cSrvMsg.CRoom.ChatRoomNr : "";                                   // get chat room number

                    cRoomMembersCrypt = cSrvMsg.TContent;                                   // set chatRoomMembersCrypted to cSrvMsg.TContent

                    Area23Log.LogOriginMsg("CqrService", "ChatRoomPush: " +
                        chatRoomNumber + $"\r\n\tsender = {cSrvMsg.Sender.NameEmail};\r\n\tall emails = {cSrvMsg.Emails}; \r\n");

                    allPollMsg = new CSrvMsg<List<string>>(cSrvMsg.Sender, cSrvMsg.Recipients.ToArray(), new List<string>(), cSrvMsg.Hash, cSrvMsg.CRoom) { Md5Hash = cSrvMsg.Md5Hash, Message = cSrvMsg.Message, MsgType = cSrvMsg.MsgType };

                    chatRoomMsg = JsonChatRoom.LoadChatRoom(ref cSrvMsg);                   // Load json chat room from file system json file                                                                                                                  
                    isValid = JsonChatRoom.CheckPermission(ref cSrvMsg);                    // Check sender's permission to access chat room (must be creator or invited)

                    if (isValid)
                    {
                        DateTime now = DateTime.Now;                                        // Determine DateTime.Now

                        CChatRoom ccr = JsonChatRoom.GetCachedChatRoom(chatRoomNumber);                  // Get chatroom message dictionary out of cache

                        Area23Log.LogOriginMsg("CqrService", "ChatRoomPush: " + chatRoomNumber + $"\r\n\tdict.keys = {ccr.MsgDict.Keys.Count} \r\n");

                        MemoryCache.CacheDict.SetValue<string>(now.Ticks.ToString(), cRoomMembersCrypt);

                        ccr.MsgDict.Add(now.Ticks, cSrvMsg.Sender.NameEmail);               // Add new entry to cached chatroom message dictionary with DateTime.Now
                        ccr.LastPushed = now;
                        chatRoomMsg.CRoom.MsgDict.Add(now.Ticks, cSrvMsg.Sender.NameEmail);
                        chatRoomMsg.CRoom.LastPushed = now;
                        allPollMsg.CRoom.MsgDict.Add(now.Ticks, cSrvMsg.Sender.NameEmail);
                        allPollMsg.CRoom.LastPushed = now;

                        JsonChatRoom.SetCachedChatRoom(chatRoomNumber, ccr);                             // Saves chatroom msg dict back to cache (Amazon valkey or ApplicationState)

                        Area23Log.LogOriginMsg("CqrService", "ChatRoomPush: " + chatRoomNumber + $"\r\n\tadded key {now.Ticks} to dict\r\n\tdict.keys = {ccr.MsgDict.Keys.Count} \r\n");

                        chatRoomMsg.TContent = "";                                          // set TContent empty, because we don't want a same huge response as request                                             
                        chatRoomMsg = JsonChatRoom.SaveChatRoom(ref chatRoomMsg);
                        // saves chat room back to json file
                        // chatRoomMsg.CRoom.LastPushed = now;
                        // chatRoomMsg.CRoom.TicksLong.Remove(now.Ticks);                      // TODO: Delete later, with that, you get your own message in sended queue
                        chatRoomMsg.Sender.Message = chatRoomNumber;

                        dict = ccr.MsgDict;
                        List<long> longKeyList = (dict == null || dict.Count < 1) ? new List<long>() : dict.Keys.ToList();
                        List<long> pollKeys = GetNewMessageIndices(longKeyList, cSrvMsg);

                        long polledPtr = -1;
                        int pollIdx = 0;
                        string firstPollClientMsg = "";

                        if (dict != null && dict.Count > 0 && pollKeys != null && pollKeys.Count > 0)
                        {
                            while (pollIdx < pollKeys.Count)
                            {
                                Area23Log.LogOriginMsg("CqrService", "ChatRoomPush: " + chatRoomNumber + $"\r\n\tpollIdx = {pollIdx} to dict\r\n\tpollKeys.Count = {pollKeys.Count} \r\n");

                                polledPtr = pollKeys[pollIdx++];
                                firstPollClientMsg = MemoryCache.CacheDict.GetValue<string>(polledPtr.ToString()) ?? "";
                                // firstPollClientMsg = dict[polledPtr] ?? "";                                
                                chatRoomMsg.CRoom.LastPolled = new DateTime(polledPtr);
                                allPollMsg.CRoom.LastPolled = new DateTime(polledPtr);
                                if (ccr.LastPolled < allPollMsg.CRoom.LastPolled)
                                    ccr.LastPolled = allPollMsg.CRoom.LastPolled;

                                if (!string.IsNullOrEmpty(firstPollClientMsg))
                                {
                                    allPollMsg.TContent.Add(firstPollClientMsg);
                                }
                            }

                            Area23Log.LogOriginMsg("CqrService", $"ChatRoomPush: allPollMsg.TContent.Count = {allPollMsg.TContent.Count}\r\n");

                            chatRoomMsg.CRoom.MsgDict = ccr.MsgDict;
                            cSrvMsg.CRoom.MsgDict = ccr.MsgDict;

                            JsonChatRoom.SetCachedChatRoom(chatRoomNumber, ccr);
                            JsonContacts.UpdateContact(chatRoomMsg.Sender);
                            chatRoomMsg = JsonChatRoom.SaveChatRoom(ref chatRoomMsg);
                        }

                        responseString = allPollMsg.EncryptToJson(_serverKey);
                    }
                    else
                    {
                        chatRoomMsg.TContent = cSrvMsg.Sender.NameEmail + " has no permission for chat room " + chatRoomNumber;
                        responseString = chatRoomMsg.EncryptToJson(_serverKey);
                    }
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
            CSrvMsg<string> cSrvMsg, chatRoomMsg;
            List<CContact> _invited = new List<CContact>();

            try
            {
                if (!string.IsNullOrEmpty(cryptMsg) && cryptMsg.Length >= 8)
                {
                    cSrvMsg = CSrvMsg<string>.FromJsonDecrypt(_serverKey, cryptMsg);            // decrypt FullSrvMsg<string>                    

                    _contact = JsonContacts.AddContact(cSrvMsg.Sender);
                    chatRoomNumber = (cSrvMsg.CRoom != null && !string.IsNullOrEmpty(cSrvMsg.CRoom.ChatRoomNr)) ? cSrvMsg.CRoom.ChatRoomNr : "";

                    chatRoomMsg = JsonChatRoom.LoadChatRoom(ref cSrvMsg);
                    isValid = JsonChatRoom.CheckPermission(ref cSrvMsg, true);

                    if (isValid)
                    {
                        if (JsonChatRoom.DeleteChatRoom(ref cSrvMsg))
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


            Area23Log.LogOriginMsg("CqrService", $"ChatRoomClose(string cryptMsg) finished. deleted chat room ChatRoomNr =  " + chatRoomNumber + ".\n");

            return responseString;
        }


        [WebMethod]
        public override string TestService()
        {
            return base.TestService();
        }

        [WebMethod]
        public override string GetIPAddress()
        {
            return base.GetIPAddress();
        }

        [WebMethod]
        public override string TestCache()
        {
            return base.TestCache();
        }

        [WebMethod]
        public override string GetKey(string key)
        {
            return base.GetKey(key);
        }

        [WebMethod]
        public override string ResetCache()
        {
            return base.ResetCache();
        }

    }

}
