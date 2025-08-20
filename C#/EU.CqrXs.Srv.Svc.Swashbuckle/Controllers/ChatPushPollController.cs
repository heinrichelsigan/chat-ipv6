using Area23.At.Framework.Core.Cqr;
using Area23.At.Framework.Core.Cqr.Msg;
using Area23.At.Framework.Core.Util;
using EU.CqrXs.Srv.Svc.Swashbuckle.Util;
using Microsoft.AspNetCore.Mvc;

namespace EU.CqrXs.Srv.Svc.Swashbuckle.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class ChatPushPollController : CqrSrvControllerBase
    {

        private readonly ILogger<ChatPushPollController> _logger;

        public ChatPushPollController(ILogger<ChatPushPollController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "ChatPushPoll")]
        public string Get(string cryptMsg)
        {
            Area23Log.LogStatic($"ChatPushPoll(string cryptMsg) called.\n");
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

                    Area23Log.LogOriginMsg("CqrService", "ChatPushPoll: " +
                        chatRoomNumber + "\r\n\tsender = " + cSrvMsg.Sender.NameEmail + ";\r\n\tall emails = " + cSrvMsg.Emails + "; \r\n");

                    allPollMsg = new CSrvMsg<List<string>>(cSrvMsg.Sender, cSrvMsg.Recipients.ToArray(), new List<string>(), cSrvMsg.Hash, cSrvMsg.CRoom) { Md5Hash = cSrvMsg.Md5Hash, Message = cSrvMsg.Message, MsgType = cSrvMsg.MsgType };

                    chatRoomMsg = JsonChatRoom.LoadChatRoom(ref cSrvMsg);                   // Load json chat room from file system json file                                                                                                                  
                    isValid = JsonChatRoom.CheckPermission(ref cSrvMsg);                    // Check sender's permission to access chat room (must be creator or invited)

                    if (isValid)
                    {
                        DateTime now = DateTime.Now;                                        // Determine DateTime.Now

                        dict = GetCachedMessageDict(chatRoomNumber);                        // Get chatroom message dictionary out of cache

                        Area23Log.LogOriginMsg("CqrService", "ChatPushPoll: " + chatRoomNumber + "\r\n\tdict.keys = " + dict.Keys.Count + " \r\n");

                        dict.Add(now.Ticks, cRoomMembersCrypt);                             // Add new entry to cached chatroom message dictionary with DateTime.Now
                        chatRoomMsg.CRoom.TicksLong.Add(now.Ticks);
                        chatRoomMsg.CRoom.LastPushed = now;

                        SetCachedMessageDict(chatRoomNumber, dict);                         // Saves chatroom msg dict back to cache (Amazon valkey or ApplicationState)

                        Area23Log.LogOriginMsg("CqrService", "ChatPushPoll: " + chatRoomNumber + 
                            "\r\n\tadded key " + now.Ticks + " to dict\r\n\tdict.keys = " + dict.Keys.Count + " \r\n");

                        chatRoomMsg.TContent = "";                                          // set TContent empty, because we don't want a same huge response as request                                             
                        chatRoomMsg = JsonChatRoom.SaveChatRoom(ref chatRoomMsg);
                        // saves chat room back to json file
                        chatRoomMsg.CRoom.LastPushed = now;
                        chatRoomMsg.CRoom.TicksLong.Remove(now.Ticks);                      // TODO: Delete later, with that, you get your own message in sended queue
                        chatRoomMsg.Sender.Message = chatRoomNumber;

                        allPollMsg.CRoom.TicksLong.Add(now.Ticks);
                        allPollMsg.CRoom.LastPushed = now;

                        List<long> longKeyList = (dict == null || dict.Count < 1) ? new List<long>() : dict.Keys.ToList();
                        List<long> pollKeys = GetNewMessageIndices(longKeyList, cSrvMsg);

                        long polledPtr = -1;
                        int pollIdx = 0;
                        string firstPollClientMsg = "";

                        if (dict != null && dict.Count > 0 && pollKeys != null && pollKeys.Count > 0)
                        {
                            while (pollIdx < pollKeys.Count)
                            {
                                Area23Log.LogOriginMsg("CqrService", "ChatPushPoll: " + chatRoomNumber + 
                                    "\r\n\tpollIdx = " + pollIdx + " to dict\r\n\tpollKeys.Count = " + pollKeys.Count + " \r\n");

                                polledPtr = pollKeys[pollIdx++];
                                firstPollClientMsg = dict[polledPtr] ?? "";
                                if (string.IsNullOrEmpty(firstPollClientMsg) && pollKeys.Count > pollIdx)
                                {
                                    chatRoomMsg.CRoom.LastPolled = new DateTime(polledPtr);
                                    allPollMsg.CRoom.LastPolled = new DateTime(polledPtr);
                                    polledPtr = pollKeys[pollIdx++];
                                    firstPollClientMsg = dict[polledPtr] ?? "";
                                }
                                chatRoomMsg.CRoom.LastPolled = new DateTime(polledPtr);
                                allPollMsg.CRoom.LastPolled = new DateTime(polledPtr);

                                allPollMsg.TContent.Add(firstPollClientMsg);
                            }

                            Area23Log.LogOriginMsg("CqrService", "ChatPushPoll: allPollMsg.TContent.Count = " + allPollMsg.TContent.Count + "\r\n");

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
                Area23Log.LogOriginMsgEx("CqrService", "ChatPushPoll(...)", ex);
            }

            Area23Log.LogOriginMsg("CqrService", "ChatPushPoll(string cryptMsg) finished. ChatRoomNr = " + chatRoomNumber + ".\n");
            return responseString;

        }

    }


}
