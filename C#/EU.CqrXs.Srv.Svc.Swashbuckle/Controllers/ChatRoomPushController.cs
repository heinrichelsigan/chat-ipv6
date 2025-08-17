using Area23.At.Framework.Core.Cqr;
using Area23.At.Framework.Core.Cqr.Msg;
using Area23.At.Framework.Core.Util;
using EU.CqrXs.Srv.Svc.Swashbuckle.Util;
using Microsoft.AspNetCore.Mvc;

namespace EU.CqrXs.Srv.Svc.Swashbuckle.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ChatRoomPushController : CqrSrvControllerBase
    {

        private readonly ILogger<ChatRoomPushController> _logger;

        public ChatRoomPushController(ILogger<ChatRoomPushController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "ChatRoomPush")]
        public IEnumerable<string> Get(string cryptMsg)
        {
            Area23Log.LogStatic($"ChatRoomPushMessage(string cryptMsg) called.\n");
            InitMethod();
            string cRoomMembersCrypt = "";
            bool isValid = false;
            Dictionary<long, string> dict;

            CSrvMsg<string> cSrvMsg, aSrvMsg = new CSrvMsg<string>(cryptMsg, CType.Json) { Hash = cqrFacade.PipeString, SerializedMsg = cryptMsg };
            aSrvMsg = aSrvMsg.FromJson(cryptMsg);

            _responseString = ""; // set empty response string per default
            CSrvMsg<string> chatRoomMsg = new CSrvMsg<string>(); // construct an empty message

            try
            {
                if (!string.IsNullOrEmpty(cryptMsg) && cryptMsg.Length >= 8)
                {
                    cSrvMsg = aSrvMsg.DecryptFromJson(_serverKey, cryptMsg);
                    cRoomMembersCrypt = cSrvMsg.TContent ?? "";
                    _contact = cSrvMsg.Sender;
                    _chatRoomNumber = (cSrvMsg.CRoom != null && !string.IsNullOrEmpty(cSrvMsg.CRoom.ChatRoomNr)) ? cSrvMsg.CRoom.ChatRoomNr : cSrvMsg.Sender.Message;
                    Area23Log.LogStatic($"chatRoomMembersCrypted len = {cRoomMembersCrypt.Length}.\n");

                    chatRoomMsg = JsonChatRoom.LoadChatRoom(cSrvMsg);
                    chatRoomMsg.TContent = ""; // set string empty, if no message

                    isValid = JsonChatRoom.CheckPermission(cSrvMsg);
                    if (isValid)
                    {
                        DateTime now = DateTime.Now;

                        dict = GetCachedMessageDict(_chatRoomNumber);

                        dict.Add(now.Ticks, cRoomMembersCrypt);
                        chatRoomMsg.CRoom.TicksLong.Add(now.Ticks);
                        chatRoomMsg.CRoom.LastPushed = now;
                        // chatRoomMsg.Sender.TicksLong.Add(now.Ticks);
                        SetCachedMessageDict(_chatRoomNumber, dict);

                        chatRoomMsg.TContent = "";
                        chatRoomMsg = JsonChatRoom.SaveChatRoom(chatRoomMsg);

                        chatRoomMsg.CRoom.LastPushed = now;
                        chatRoomMsg.CRoom.TicksLong.Remove(now.Ticks);                      // TODO: Delete later, with that, you get your own message in sended queue
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
                Area23Log.LogOriginMsgEx("CqrService", "ChatRoomPush(...)", ex);
            }

            Area23Log.LogOriginMsg("CqrService", $"ChatRoomPush(string cryptMsg, string cRoomMembersCrypt) finished. ChatRoomNr = " + _chatRoomNumber + ".\n");
            string[] resp = { _responseString };

            List<string> list = new List<string>(resp);
            return list;

        }
    }


}
