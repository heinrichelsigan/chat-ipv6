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
    public class ChatRoomCloseController : CqrSrvControllerBase
    {

        private readonly ILogger<ChatRoomCloseController> _logger;

        public ChatRoomCloseController(ILogger<ChatRoomCloseController> logger)
        {
            _logger = logger;
        }

        [HttpGet("ChatRoomClose")]
        public string Get(string cryptMsg)
        {
            Area23Log.LogStatic($"ChatRoomClose(string cryptMsg) started. cryptMsg.Length =  {cryptMsg.Length}.\n");
            InitMethod();
            bool isValid = false;

            CSrvMsg<string>? cSrvMsg = null;                     
            List<CContact> _invited = new List<CContact>();

            _responseString = "";

            try
            {
                if (!string.IsNullOrEmpty(cryptMsg) && cryptMsg.Length >= 8)
                {
                    cSrvMsg = CSrvMsg<string>.FromJsonDecrypt(_serverKey, cryptMsg); 
                    _contact = AddContact(cSrvMsg.Sender);
                    _chatRoomNumber = (cSrvMsg.CRoom != null && !string.IsNullOrEmpty(cSrvMsg.CRoom.ChatRoomNr)) ? cSrvMsg.CRoom.ChatRoomNr : "";
                    cSrvMsg = JsonChatRoom.LoadChatRoom(cSrvMsg, _chatRoomNumber);
                    isValid = ChatRoomCheckPermission(cSrvMsg, _chatRoomNumber, true);
                    if (isValid)
                    {
                        JsonChatRoom.DeleteChatRoom(_chatRoomNumber);
                        cSrvMsg.CRoom = null;
                        cSrvMsg.Sender._message = "";
                    }

                    _responseString = cSrvMsg.EncryptToJson(_serverKey);

                }
            }
            catch (Exception ex)
            {
                CqrException.SetLastException(ex);
                Area23Log.LogStatic(ex);
            }

            Area23Log.LogStatic($"ChatRoomClose(string cryptMsg) finished. deleted chat room ChatRoomNr =  {_chatRoomNumber}.\n");

            return _responseString;

        }


    }
}
