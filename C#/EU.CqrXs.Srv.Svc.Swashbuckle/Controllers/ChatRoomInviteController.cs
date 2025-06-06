using Area23.At.Framework.Core;
using Area23.At.Framework.Core.Win32Api;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.InteropServices;
using System.IO;
using Grpc.Core;
using Microsoft.AspNetCore.Components.Forms;
using Area23.At.Framework.Core.Util;
using Area23.At.Framework.Core.Cqr.Msg;
using Area23.At.Framework.Core.Cqr;

namespace EU.CqrXs.Srv.Svc.Swashbuckle.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ChatRoomInviteController : CqrSrvControllerBase
    {        
        private readonly ILogger<ChatRoomInviteController> _logger;

        public ChatRoomInviteController(ILogger<ChatRoomInviteController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "ChatRoomInvite")]
        public string Get(string cryptMsg)
        {
            Area23Log.LogStatic("ChatRoomInvite(string cryptMsg) called.  cryptMsg.Length = " + cryptMsg.Length + ".\n");
            InitMethod();

            _chatRoomNumber = "";            
            CSrvMsg<string>? cSrvMsg = new CSrvMsg<string>(cryptMsg, CType.Json) { _hash = cqrFacade.PipeString, SerializedMsg = cryptMsg };
            cSrvMsg = cSrvMsg.FromJson(cryptMsg);

            _responseString = "";

            try
            {
                if (!string.IsNullOrEmpty(cryptMsg) && cryptMsg.Length >= 8)
                {
                    cSrvMsg = CSrvMsg<string>.FromJsonDecrypt(_serverKey, cryptMsg);    // decrypt CSrvMsg<string>            
                    _contact = AddContact(cSrvMsg.Sender);                              // add contact from FullSrvMsg<string>   
                    cSrvMsg = InviteToChatRoom(cSrvMsg);                                // generate a FullSrvMsg<string> chatserver message by inviting                           

                    _responseString = cSrvMsg.EncryptToJson(_serverKey);                // crypt chatRSrvMsg with _serverKey and serialize as json
                }
            }
            catch (Exception ex)
            {
                CqrException.SetLastException(ex);
                Area23Log.LogStatic(ex);
            }

            Area23Log.LogStatic("ChatRoomInvite(string cryptMsg) finished. ChatRoomNr = " + _chatRoomNumber + ".\n");
            return _responseString;

        }

    }
}
