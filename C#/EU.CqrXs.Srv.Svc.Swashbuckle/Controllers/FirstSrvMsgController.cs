using Area23.At.Framework.Core.Cqr;
using Area23.At.Framework.Core.Cqr.Msg;
using Area23.At.Framework.Core.Util;
using EU.CqrXs.Srv.Svc.Swashbuckle.Util;
using Microsoft.AspNetCore.Mvc;


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
            Area23Log.LogOriginMsg("FirstSrvMsgController", "Send1StSrvMsg(string cryptMsg) called.  cryptMsg.Length = " + cryptMsg.Length + ".\n");
            InitMethod();

            try
            {
                if (!string.IsNullOrEmpty(cryptMsg) && cryptMsg.Length >= 8)
                {
                    _contact = new CContact();
                    _contact = _contact.FromJson<CContact>(cryptMsg);
                    _decrypted = _contact.ToJson().ToString();
                    Area23Log.LogOriginMsg("FirstSrvMsgController", $"Contact decrypted successfully: {_decrypted}\n");
                }
            }
            catch (Exception ex)
            {
                CqrException.SetLastException(ex);
                Area23Log.LogOriginMsgEx("FirstSrvMsgController", $"Exception {ex.GetType()} when decrypting contact.", ex);
            }

            _responseString = CContact.ToJsonEncrypt(_serverKey, _contact);

            if (!string.IsNullOrEmpty(_decrypted) && _contact != null && !string.IsNullOrEmpty(_contact.NameEmail))
            {               
                CContact foundCt = JsonContacts.AddContact(_contact);
                foundCt = (foundCt == null) ? _contact : foundCt;
                _responseString = CContact.ToJsonEncrypt(_serverKey, _contact);
            }

            Area23Log.LogOriginMsg("FirstSrvMsgController", $"Send1StSrvMsg(string cryptMsg) finished.  _contact.Cuid = {_contact.Cuid}.\n");
            return _responseString;
        }

    }
}
