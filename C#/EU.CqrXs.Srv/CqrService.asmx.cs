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
            Area23Log.Logger.LogOriginMsg("CqrService", $"Send1StSrvMsg(string cryptMsg) called.  cryptMsg.Length = {cryptMsg.Length}.\n");
            InitMethod();

            
            CContact cContact = new CContact() { Hash = _cqrFacade.PipeString };

            try
            {
                if (!string.IsNullOrEmpty(cryptMsg) && cryptMsg.Length >= 8)
                {
                    _contact = cContact.FromJson<CContact>(cryptMsg);
                    _contact = cContact.DecryptFromJson(_serverKey, cryptMsg);
                    _decrypted = _contact.ToJson();
                    MemoryCache.CacheDict.SetValue<string>("lastmsg", _decrypted);
                    Area23Log.Logger.LogOriginMsg("CqrService", $"Contact decrypted successfully: {_decrypted}\n");
                }
            }
            catch (Exception ex)
            {
                CqrException.SetLastException(ex);
                Area23Log.Logger.LogOriginMsgEx("CqrService", $"Exception {ex.GetType()} when decrypting contact: {ex.Message}", ex);
            }

            _responseString = _contact.EncryptToJson(_serverKey);

            if (!string.IsNullOrEmpty(_decrypted) && _contact != null && !string.IsNullOrEmpty(_contact.NameEmail))
            {
                // MemoryCache.CacheDict.SetValue<string>("lastdecrypted", _decrypted);                     

                CContact foundCt = JsonContacts.AddContact(_contact);
                _responseString = foundCt.EncryptToJson(_serverKey);
            }

            Area23Log.Logger.LogOriginMsg("CqrService", $"Send1StSrvMsg(string cryptMsg) finished.  _contact.Cuid = {_contact.Cuid}.\n");
            return _responseString;
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
            Area23Log.Logger.LogOriginMsg("CqrService", "ChatRoomInvite(string cryptMsg) called.  cryptMsg.Length = " + cryptMsg.Length + ".");
            InitMethod();
            
            CSrvMsg<string> cSrvMsg, chatRSrvMsg = new CSrvMsg<string>(cryptMsg, CType.Json) { Hash = _cqrFacade.PipeString, SerializedMsg = cryptMsg };
            chatRSrvMsg = chatRSrvMsg.FromJson(cryptMsg);

            try
            {
                if (!string.IsNullOrEmpty(cryptMsg) && cryptMsg.Length >= 8)
                {
                    cSrvMsg = CSrvMsg<string>.FromJsonDecrypt(_serverKey, cryptMsg);    // decrypt CSrvMsg<string>
                    cSrvMsg = chatRSrvMsg.DecryptFromJson(_serverKey, cryptMsg);        // decrypt CSrvMsg<string>
                    _contact = JsonContacts.AddContact(cSrvMsg.Sender);                       // add contact from FullSrvMsg<string>   
                    chatRSrvMsg = InviteToChatRoom(cSrvMsg);                            // generate a FullSrvMsg<string> chatserver message by inviting                           

                    _responseString = chatRSrvMsg.EncryptToJson(_serverKey);            // crypt chatRSrvMsg with _serverKey and serialize as json
                }
            }
            catch (Exception ex)
            {
                CqrException.SetLastException(ex);
                Area23Log.Logger.LogOriginMsgEx("CqrService", "ChatRoomInvite Exception", ex);
            }

            Area23Log.Logger.LogOriginMsg("CqrService", "ChatRoomInvite(string cryptMsg) finished. ChatRoomNr = " + _chatRoomNumber  + ".\n");
            return _responseString;

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
            Area23Log.Logger.LogOriginMsg("CqrService", $"ChatRoomPoll(string cryptMsg) called.  cryptMsg.Length = " + cryptMsg.Length + ".\n");
            InitMethod();

            Dictionary<long, string> dict = new Dictionary<long, string>();
            
            CSrvMsg<string> cSrvMsg, aSrvMsg =  new CSrvMsg<string>(cryptMsg, CType.Json) { Hash = _cqrFacade.PipeString, SerializedMsg = cryptMsg };
            aSrvMsg = aSrvMsg.FromJson(cryptMsg);

            try
            {
                if (!string.IsNullOrEmpty(cryptMsg) && cryptMsg.Length >= 8)
                {
                    cSrvMsg = CSrvMsg<string>.FromJsonDecrypt(_serverKey, cryptMsg);        // decrypt FullSrvMsg<string>
                    cSrvMsg = aSrvMsg.DecryptFromJson(_serverKey, cryptMsg);                // decrypt FullSrvMsg<string>
                    _contact = cSrvMsg.Sender;
                    _chatRoomNumber = (cSrvMsg.CRoom != null && !string.IsNullOrEmpty(cSrvMsg.CRoom.ChatRoomNr)) ? cSrvMsg.CRoom.ChatRoomNr : cSrvMsg.Message;

                    CSrvMsg<string> chatRoomMsg = JsonChatRoom.LoadChatRoom(cSrvMsg, _chatRoomNumber);
                    chatRoomMsg = JsonChatRoom.CheckPermission(cSrvMsg, chatRoomMsg, _chatRoomNumber, out _isValid);

                    if (_isValid)
                    {
                        dict = GetCachedMessageDict(_chatRoomNumber);
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
                            chatRoomMsg = JsonChatRoom.SaveChatRoom(chatRoomMsg, chatRoomMsg.CRoom);

                            chatRoomMsg.TContent = firstPollClientMsg;
                        }

                    }

                    _responseString = chatRoomMsg.EncryptToJson(_serverKey);        // encrypt chatRoomMsg and json serialize it

                }
            }
            catch (Exception ex)
            {
                CqrException.SetLastException(ex);
                Area23Log.Logger.LogOriginMsgEx("CqrService", "ChatRoomPoll Exception", ex);
            }

            Area23Log.Logger.LogOriginMsg("CqrService", "ChatRoomPushMessage(string cryptMsg, string chatRoomMembersCrypted) finshed. ChatRoomNr =  " + _chatRoomNumber + ".\n");
            return _responseString;

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
            Area23Log.Logger.LogOriginMsg("CqrService", $"ChatRoomPushMessage(string cryptMsg) called.\n");
            InitMethod();

            string chatRoomMembersCrypted = "";
            Dictionary<long, string> dict;

            CSrvMsg<string> cSrvMsg, aSrvMsg = new CSrvMsg<string>(cryptMsg, CType.Json) { Hash = _cqrFacade.PipeString, SerializedMsg = cryptMsg };
            aSrvMsg = aSrvMsg.FromJson(cryptMsg);

            CSrvMsg<string> chatRoomMsg = new CSrvMsg<string>(); // construct an empty message

            try
            {
                if (!string.IsNullOrEmpty(cryptMsg) && cryptMsg.Length >= 8)
                {
                    cSrvMsg = CSrvMsg<string>.FromJsonDecrypt(_serverKey, cryptMsg);            // decrypt FullSrvMsg<string>
                    cSrvMsg = aSrvMsg.DecryptFromJson(_serverKey, cryptMsg);                    // decrypt FullSrvMsg<string>
                    _contact = cSrvMsg.Sender;
                    _chatRoomNumber = (cSrvMsg.CRoom != null && !string.IsNullOrEmpty(cSrvMsg.CRoom.ChatRoomNr)) 
                        ? cSrvMsg.CRoom.ChatRoomNr : "";                                        // get chat room number
                    chatRoomMembersCrypted = cSrvMsg.TContent;                                  // set chatRoomMembersCrypted to cSrvMsg.TContent

                    Area23Log.Logger.LogOriginMsg("CqrService", $"string chatRoomMembersCrypted = cSrvMsg.TContent; \r\n\tchatRoomMembersCrypted len = {chatRoomMembersCrypted.Length}.");
                    chatRoomMsg = JsonChatRoom.LoadChatRoom(cSrvMsg, _chatRoomNumber);          // Load json chat room from file system json file                                                                                                                  
                    cSrvMsg = JsonChatRoom.CheckPermission(cSrvMsg, chatRoomMsg,                // Check sender's permission to access chat room (must be creator or invited)
                        _chatRoomNumber, out _isValid);   

                    if (_isValid)    
                    {
                        DateTime now = DateTime.Now;                                            // Determine DateTime.Now

                        dict = GetCachedMessageDict(_chatRoomNumber);                           // Get chatroom message dictionary out of cache

                        dict.Add(now.Ticks, chatRoomMembersCrypted);                            // Add new entry to cached chatroom message dictionary with DateTime.Now
                        chatRoomMsg.CRoom.TicksLong.Add(now.Ticks);
                        chatRoomMsg.CRoom.LastPushed = now;
                        SetCachedMessageDict(_chatRoomNumber, dict);                            // Saves chatroom msg dict back to cache (Amazon valkey or ApplicationState)

                        // UpdateContact(_contact);        
                        chatRoomMsg.TContent = "";                                              // set TContent empty, because we don't want a same huge response as request                                             
                        chatRoomMsg = JsonChatRoom.SaveChatRoom(
                            chatRoomMsg, chatRoomMsg.CRoom);                                    // saves chat room back to json file
                        
                        chatRoomMsg.CRoom.LastPushed = now;
                        chatRoomMsg.CRoom.TicksLong.Remove(now.Ticks);                          // TODO: Delete later, with that, you get your own message in sended queue
                        chatRoomMsg.Sender.Message = _chatRoomNumber;                          
                    }
                    else
                        chatRoomMsg.TContent = cSrvMsg.Sender.NameEmail + " has no permission for chat room " + _chatRoomNumber;

                    _responseString = chatRoomMsg.EncryptToJson(_serverKey);

                }
            }
            catch (Exception ex)
            {
                CqrException.SetLastException(ex);
                Area23Log.Logger.LogOriginMsgEx("CqrService", "ChatRoomPush Exception", ex);
            }

            Area23Log.Logger.LogOriginMsg("CqrService", $"ChatRoomPushMessage(string cryptMsg, string chatRoomMembersCrypted) finished. ChatRoomNr =  {_chatRoomNumber}.\n");
            return _responseString;
        }

        /// <summary>
        /// ChatRoomClose
        /// </summary>
        /// <param name="cryptMsg"></param>
        /// <returns></returns>
        [WebMethod]
        public string ChatRoomClose(string cryptMsg)
        {
            Area23Log.Logger.LogOriginMsg("CqrService", $"ChatRoomClose(string cryptMsg) started. cryptMsg.Length =  {cryptMsg.Length}.\n");
            InitMethod();

            CSrvMsg<string> cSrvMsg, aSrvMsg = new CSrvMsg<string>(cryptMsg, CType.Json) { Hash = _cqrFacade.PipeString, SerializedMsg = cryptMsg };
            aSrvMsg = aSrvMsg.FromJson(cryptMsg);
            List<CContact> _invited = new List<CContact>();            

            try
            {
                if (!string.IsNullOrEmpty(cryptMsg) && cryptMsg.Length >= 8)
                {
                    cSrvMsg = CSrvMsg<string>.FromJsonDecrypt(_serverKey, cryptMsg);
                    cSrvMsg = aSrvMsg.DecryptFromJson(_serverKey, cryptMsg);                    // decrypt FullSrvMsg<string>
                    _contact = JsonContacts.AddContact(cSrvMsg.Sender);
                    _chatRoomNumber = (cSrvMsg.CRoom != null && !string.IsNullOrEmpty(cSrvMsg.CRoom.ChatRoomNr)) ? cSrvMsg.CRoom.ChatRoomNr : "";
                    
                    CSrvMsg<string> chatRoomMsg = JsonChatRoom.LoadChatRoom(cSrvMsg, _chatRoomNumber);
                    cSrvMsg = JsonChatRoom.CheckPermission(cSrvMsg, chatRoomMsg, _chatRoomNumber, out _isValid, true);

                    if (_isValid)
                    {
                        if (JsonChatRoom.DeleteChatRoom(_chatRoomNumber))
                        {
                            chatRoomMsg.CRoom = null;
                            chatRoomMsg.Sender.Message = "";
                        }
                    }
                    
                    _responseString = chatRoomMsg.EncryptToJson(_serverKey);

                }
            }
            catch (Exception ex)
            {
                CqrException.SetLastException(ex);
                Area23Log.Logger.LogOriginMsgEx("CqrService", "ChatRoomClose Exception", ex);
            }

            Area23Log.Logger.LogOriginMsg("CqrService", $"ChatRoomClose(string cryptMsg) finished. deleted chat room ChatRoomNr =  {_chatRoomNumber}.\n");

            return _responseString;

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
