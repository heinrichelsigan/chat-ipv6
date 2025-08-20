using Area23.At.Framework.Core.Cqr;
using Area23.At.Framework.Core.Cqr.Msg;
using Area23.At.Framework.Core.Util;
using EU.CqrXs.Srv.Svc.Swashbuckle.Util;
using Microsoft.AspNetCore.Mvc;

namespace EU.CqrXs.Srv.Svc.Swashbuckle.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ChatRoomPollController : CqrSrvControllerBase
    {

        private readonly ILogger<ChatRoomPollController> _logger;

        public ChatRoomPollController(ILogger<ChatRoomPollController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "ChatRoomPoll")]
        public string Get(string cryptMsg)
        {
            Area23Log.LogStatic($"ChatRoomPoll(string cryptMsg) called.  cryptMsg.Length = " + cryptMsg.Length + ".\n");
            InitMethod();

            Dictionary<long, string> dict = new Dictionary<long, string>();
            bool isValid = false;

            CSrvMsg<string> cSrvMsg;

            _responseString = "";

            try
            {
                if (!string.IsNullOrEmpty(cryptMsg) && cryptMsg.Length >= 8)
                {
                    cSrvMsg = CSrvMsg<string>.FromJsonDecrypt(_serverKey, cryptMsg);          // decrypt FullSrvMsg<string>
                    _contact = cSrvMsg.Sender;
                    _chatRoomNumber = (cSrvMsg.CRoom != null && !string.IsNullOrEmpty(cSrvMsg.CRoom.ChatRoomNr)) ? cSrvMsg.CRoom.ChatRoomNr : cSrvMsg.Sender.Message;

                    CSrvMsg<string> chatRoomMsg = JsonChatRoom.LoadChatRoom(ref cSrvMsg);
                    isValid = JsonChatRoom.CheckPermission(ref cSrvMsg);
                    chatRoomMsg.TContent = string.Empty;

                    if (isValid)
                    {
                        dict = GetCachedMessageDict(_chatRoomNumber);
                        List<long> longKeyList = (dict == null || dict.Count < 1) ? new List<long>() : dict.Keys.ToList();
                        List<long> pollKeys = GetNewMessageIndices(dict.Keys.ToList(), cSrvMsg);

                        long polledPtr = -1;
                        if (pollKeys.Count > 0)
                        {
                            polledPtr = pollKeys[0];
                            string firstPollClientMsg = dict[polledPtr];
                            if (string.IsNullOrEmpty(firstPollClientMsg) && pollKeys.Count > 1)
                            {
                                DateTime date = new DateTime(polledPtr);
                                chatRoomMsg.CRoom.LastPolled = date;
                                if (!chatRoomMsg.CRoom.TicksLong.Contains(polledPtr))
                                    chatRoomMsg.CRoom.TicksLong.Add(polledPtr);

                                polledPtr = pollKeys[1];
                                firstPollClientMsg = dict[polledPtr];
                            }

                            DateTime datePolled = new DateTime(polledPtr);
                            chatRoomMsg.CRoom.LastPolled = datePolled;
                            if (!chatRoomMsg.CRoom.TicksLong.Contains(polledPtr))
                                chatRoomMsg.CRoom.TicksLong.Add(polledPtr);

                            JsonContacts.UpdateContact(chatRoomMsg.Sender);
                            chatRoomMsg = JsonChatRoom.SaveChatRoom(ref chatRoomMsg);

                            chatRoomMsg.TContent = firstPollClientMsg;
                        }

                    }

                    _responseString = chatRoomMsg.EncryptToJson(_serverKey);        // encrypt chatRoomMsg and json serialize it

                }
            }
            catch (Exception ex)
            {
                CqrException.SetLastException(ex);
                Area23Log.LogOriginMsgEx("CqrService", "ChatRoomPoll(...)", ex);
            }

            Area23Log.LogOriginMsg("CqrService", "ChatRoomPushMessage(string cryptMsg, string chatRoomMembersCrypted) finihed. ChatRoomNr =  " + _chatRoomNumber + ".\n");

            return _responseString;
        }

    }


}
