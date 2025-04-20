using Area23.At.Framework.Core;
using Area23.At.Framework.Core.Cqr.Msg;
using Area23.At.Framework.Core.Cqr;
using Area23.At.Framework.Core.Util;
using Area23.At.Framework.Core.Win32Api;
using EU.CqrXs.Srv.Svc.Swashbuckle.Util;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.InteropServices;
using System.Reflection;
using Area23.At.Framework.Core.Static;

namespace EU.CqrXs.Srv.Svc.Swashbuckle.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestCacheController : CqrSrvControllerBase
    {

        private readonly ILogger<TestCacheController> _logger;

        public TestCacheController(ILogger<TestCacheController> logger)
        {
            _logger = logger;
        }

        [HttpGet("TestCacheController")]
        public string Get()
        {
            Area23Log.LogStatic($"TestCacheController.Get() started.\n");

            string testReport = $"{GetDateNow()}:TestCache() started.\n";
            try
            {
                InitMethod();
            }
            catch (Exception ex1)
            {
                testReport += $"{GetDateNow()}: Exception {ex1.GetType()}: {ex1.Message}\n\t{ex1}\n";
            }

            testReport += $"{GetDateNow()}: InitMethod() completed.\n";

            testReport += $"{GetDateNow()}: Persistence in {PersistMsgIn.PersistMsg.ToString()}\n";

            Dictionary<Guid, CContact> dictCacheTest = new Dictionary<Guid, CContact>();
            foreach (CContact c in _contacts)
            {
                if (c != null && c.Cuid != null && c.Cuid != Guid.Empty &&
                    !dictCacheTest.Keys.Contains(c.Cuid))
                    dictCacheTest.Add(c.Cuid, c);
            }
            testReport += $"{GetDateNow()}: Added {dictCacheTest.Count} count contacts to Dictionary<Guid, CqrContact>...\n";
            if (PersistMsgInAmazonElasticCache)
            {
                try
                {
                    testReport += $"{GetDateNow()}: Ready to connect to {System.Configuration.ConfigurationManager.AppSettings[Constants.VALKEY_CACHE_HOST_PORT_KEY]}\n";
                    string status = RedIS.ConnMux.GetStatus();
                    testReport += $"{GetDateNow()}: ConnectionMulitplexer.Status = {status}" + Environment.NewLine;

                    testReport += $"{GetDateNow()}: Preparing to set Dictionary<Guid, CqrContact> in cache." + Environment.NewLine;
                    RedIS.ValKey.SetKey<Dictionary<Guid, CContact>>("TestCache", dictCacheTest);
                    testReport += $"{GetDateNow()}: Added serialized json string to cache." + Environment.NewLine;

                    Dictionary<Guid, CContact> outdict = (Dictionary<Guid, CContact>)RedIS.ValKey.GetKey<Dictionary<Guid, CContact>>("TestCache");
                    testReport += $"{GetDateNow()}: Got Dictionary<Guid, CqrContact> from cache with {outdict.Keys.Count} keys." + Environment.NewLine;
                    foreach (CContact contact in outdict.Values)
                    {
                        testReport += $"{GetDateNow()}: Contact Cuid={contact.Cuid} NameEmail={contact.NameEmail} Mobile={contact.Mobile}" + Environment.NewLine;
                    }

                    List<string> chatRooms = JsonChatRoom.GetJsonChatRoomsFromCache();
                    testReport += $"{GetDateNow()}:Found {chatRooms.Count} chat room keys in cache." + Environment.NewLine;
                    foreach (string room in chatRooms)
                    {
                        try
                        {
                            Dictionary<long, string> dicTest = GetCachedMessageDict(room);
                            testReport += $"{GetDateNow()}: chat room {room} with keys {dicTest.Keys.Count} messages." + Environment.NewLine;
                        }
                        catch (Exception exChatRoom)
                        {
                            testReport += $"{GetDateNow()}: loading chat room {room} failed. Exception: {exChatRoom.Message}." + Environment.NewLine;
                            Area23Log.LogStatic($"Loading chat room {room} failed. ", exChatRoom, "");
                        }
                    }
                }
                catch (Exception ex2)
                {
                    testReport += $"{GetDateNow()}: Exception {ex2.GetType()}: {ex2.Message}\n\t{ex2}\n";
                }
            }

            return testReport;

        }


    }
}
