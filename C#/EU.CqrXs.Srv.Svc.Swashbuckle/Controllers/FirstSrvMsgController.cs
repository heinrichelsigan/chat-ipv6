using Area23.At.Framework.Core;
using Area23.At.Framework.Core.Win32Api;
using Area23.At.Framework.Core.Util;
using Area23.At.Framework.Core.Cqr.Msg;
using Area23.At.Framework.Core.Cqr;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.InteropServices;
using System.IO;
using Grpc.Core;

using Microsoft.AspNetCore.Components.Forms;
using EU.CqrXs.Srv.Svc.Swashbuckle.Util;


namespace EU.CqrXs.Srv.Svc.Swashbuckle.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FirstSrvMsgController : CqrSrvControllerBase
    {        
        private readonly ILogger<FirstSrvMsgController> _logger;

        public FirstSrvMsgController(ILogger<FirstSrvMsgController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "Send1StSrvMsg")]
        public string Get(string cryptMsg)
        {
            Area23Log.LogStatic($"Send1StSrvMsg(string cryptMsg) called.  cryptMsg.Length = {cryptMsg.Length}.\n");
            InitMethod();


            CContact cContact = new CContact() { _hash = cqrFacade.PipeString };

            try
            {
                if (!string.IsNullOrEmpty(cryptMsg) && cryptMsg.Length >= 8)
                {
                    _contact = cContact.DecryptFromJson(_serverKey, cryptMsg);
                    _decrypted = _contact.ToJson();
                    Area23Log.LogStatic($"Contact decrypted successfully: {_decrypted}\n");
                }
            }
            catch (Exception ex)
            {
                CqrException.SetLastException(ex);
                Area23Log.LogStatic($"Exception {ex.GetType()} when decrypting contact: {ex.Message}\n\t{ex.ToString()}\n");
            }

            _responseString = _contact.EncryptToJson(_serverKey);

            if (!string.IsNullOrEmpty(_decrypted) && _contact != null && !string.IsNullOrEmpty(_contact.NameEmail))
            {               
                CContact foundCt = AddContact(_contact);
                _responseString = foundCt.EncryptToJson(_serverKey);
            }

            Area23Log.LogStatic($"Send1StSrvMsg(string cryptMsg) finished.  _contact.Cuid = {_contact.Cuid}.\n");
            return _responseString;
        }

    }
}
