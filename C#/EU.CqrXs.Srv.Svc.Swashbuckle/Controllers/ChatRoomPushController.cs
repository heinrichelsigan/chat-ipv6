using Area23.At.Framework.Core;
using Area23.At.Framework.Core.Cqr.Msg;
using Area23.At.Framework.Core.Cqr;
using Area23.At.Framework.Core.Util;
using Area23.At.Framework.Core.Win32Api;
using EU.CqrXs.Srv.Svc.Swashbuckle.Util;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.InteropServices;

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
            string chatRoomMembersCrypted = "";
            bool isValid = false;
            Dictionary<long, string> dict;

            CSrvMsg<string> cSrvMsg, aSrvMsg = new CSrvMsg<string>(cryptMsg, CType.Json) { _hash = cqrFacade.PipeString, SerializedMsg = cryptMsg };
            aSrvMsg = aSrvMsg.FromJson(cryptMsg);

            _responseString = ""; // set empty response string per default
            CSrvMsg<string> chatRoomMsg = new CSrvMsg<string>(); // construct an empty message

            try
            {
                if (!string.IsNullOrEmpty(cryptMsg) && cryptMsg.Length >= 8)
                {
                    cSrvMsg = aSrvMsg.DecryptFromJson(_serverKey, cryptMsg);
                    chatRoomMembersCrypted = cSrvMsg.TContent ?? "";
                    _contact = cSrvMsg.Sender;
                    _chatRoomNumber = (cSrvMsg.CRoom != null && !string.IsNullOrEmpty(cSrvMsg.CRoom.ChatRoomNr)) ? cSrvMsg.CRoom.ChatRoomNr : cSrvMsg.Sender.CRoom.ChatRoomNr;
                    Area23Log.LogStatic($"chatRoomMembersCrypted len = {chatRoomMembersCrypted.Length}.\n");

                    chatRoomMsg = JsonChatRoom.LoadChatRoom(cSrvMsg, _chatRoomNumber);
                    chatRoomMsg.TContent = ""; // set string empty, if no message

                    isValid = ChatRoomCheckPermission(cSrvMsg, _chatRoomNumber);
                    if (isValid)
                    {
                        DateTime now = DateTime.Now;

                        dict = GetCachedMessageDict(_chatRoomNumber);

                        dict.Add(now.Ticks, chatRoomMembersCrypted);
                        chatRoomMsg.CRoom.TicksLong.Add(now.Ticks);
                        // chatRoomMsg.Sender.TicksLong.Add(now.Ticks);

                        SetCachedMessageDict(_chatRoomNumber, dict);

                        _contact = AddPollDate(_contact, now, true);
                        chatRoomMsg.Sender = AddPollDate(chatRoomMsg.Sender, now, true);

                        UpdateContact(_contact);
                        chatRoomMsg = JsonChatRoom.SaveChatRoom(chatRoomMsg, chatRoomMsg.CRoom);
                        chatRoomMsg.Sender.CRoom.LastPushed = now;
                        chatRoomMsg.CRoom.LastPushed = now;

                    }

                    _responseString = chatRoomMsg.EncryptToJson(_serverKey);

                }
            }
            catch (Exception ex)
            {
                CqrException.SetLastException(ex);
                Area23Log.LogStatic(ex);
            }

            Area23Log.LogStatic($"ChatRoomPushMessage(string cryptMsg, string chatRoomMembersCrypted) finished. ChatRoomNr =  {_chatRoomNumber}.\n");
            string[] resp = { _responseString };

            List<string> list = new List<string>(resp);
            return list;

        }
    }


}
