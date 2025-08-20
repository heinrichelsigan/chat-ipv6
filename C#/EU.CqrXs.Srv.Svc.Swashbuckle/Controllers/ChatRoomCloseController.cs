using Area23.At.Framework.Core.Cqr;
using Area23.At.Framework.Core.Cqr.Msg;
using Area23.At.Framework.Core.Util;
using EU.CqrXs.Srv.Svc.Swashbuckle.Util;
using Microsoft.AspNetCore.Mvc;

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
        
        [HttpGet(Name = "ChatRoomClose")]
        public string Get(string cryptMsg)
        {
            Area23Log.LogOriginMsg("ChatRoomCloseController", $"ChatRoomClose(string cryptMsg) started. cryptMsg.Length =  {cryptMsg.Length}.\n");
            InitMethod();            
            _chatRoomNumber = "";
            bool isValidToClose = false;
            
            CSrvMsg<string>? cSrvMsg;

            // List<CContact> _invited = new List<CContact>();

            _responseString = "";

            try
            {
                if (!string.IsNullOrEmpty(cryptMsg) && cryptMsg.Length >= 8)
                {
                    cSrvMsg = CSrvMsg<string>.FromJsonDecrypt(_serverKey, cryptMsg); 
                    _contact = JsonContacts.AddContact(cSrvMsg.Sender);
                    _chatRoomNumber = (cSrvMsg.CRoom != null && !string.IsNullOrEmpty(cSrvMsg.CRoom.ChatRoomNr)) ? cSrvMsg.CRoom.ChatRoomNr : "";

                    CSrvMsg<string> chatRoomMsg = JsonChatRoom.LoadChatRoom(ref cSrvMsg);
                    isValidToClose = JsonChatRoom.CheckChatRoomClosePermission(ref cSrvMsg);

                    if (isValidToClose)
                    {
                        if (JsonChatRoom.DeleteChatRoom(_chatRoomNumber))
                        {
                            chatRoomMsg.CRoom = null;
                            chatRoomMsg.Sender.Message = "";
                            chatRoomMsg.TContent = "";
                        }
                    }

                    _responseString = chatRoomMsg.EncryptToJson(_serverKey);

                }
            }
            catch (Exception ex)
            {
                CqrException.SetLastException(ex);
                Area23Log.LogOriginMsgEx("ChatRoomCloseController", "Exception " + ex.GetType() + " Get(string)", ex);
            }

            Area23Log.LogOriginMsg("ChatRoomCloseController", $"ChatRoomClose(string cryptMsg) finished. deleted chat room ChatRoomNr =  {_chatRoomNumber}.\n");

            return _responseString;

        }


    }
}
