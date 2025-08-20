using Area23.At.Framework.Core.Cache;
using Area23.At.Framework.Core.Cqr.Msg;
using Area23.At.Framework.Core.Static;
using Area23.At.Framework.Core.Util;
using EU.CqrXs.Srv.Svc.Swashbuckle.Util;
using Microsoft.AspNetCore.Mvc;


namespace EU.CqrXs.Srv.Svc.Swashbuckle.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestCacbeController : CqrSrvControllerBase
    {

        private readonly ILogger<TestCacbeController> _logger;

        public TestCacbeController(ILogger<TestCacbeController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "TestCacbe")]
        public string Get()
        {
            Area23Log.LogOriginMsg("TestCacheController", $"TestCacheController.Get() started.\n");

            string testReport = DateTime.Now.Area23DateTimeWithMillis() + ": TestCache() started.\n";
            try
            {
                InitMethod();
            }
            catch (Exception ex1)
            {
                testReport += DateTime.Now.Area23DateTimeWithMillis() + ": Exception " + ex1.GetType() + ": " + ex1.Message + "\n\t" + ex1 + "\n";
            }

            testReport += DateTime.Now.Area23DateTimeWithMillis() + ": InitMethod() completed.\n";

            testReport += DateTime.Now.Area23DateTimeWithMillis() + ": Persistence in " + PersistInCache.CacheType.ToString() + "\n";

            Dictionary<Guid, CContact> dictCacheTest = new Dictionary<Guid, CContact>();
            foreach (CContact c in _contacts)
            {
                if (c != null && c.Cuid != null && c.Cuid != Guid.Empty &&
                    !dictCacheTest.Keys.Contains(c.Cuid))
                        dictCacheTest.Add(c.Cuid, c);
            }
            testReport += DateTime.Now.Area23DateTimeWithMillis() + ": Added " + dictCacheTest.Count + " count contacts to Dictionary<Guid, CqrContact>...\n";

            try
            {
                //if (PersistInCache.CacheType == PersistType.RedisValkey)
                //{
                //    string valkeyCacheHostPort = System.Configuration.ConfigurationManager.AppSettings[Constants.VALKEY_CACHE_HOST_PORT_KEY];
                //    testReport += $"{DateTime.Now.Area23DateTimeWithMillis()}: Ready to connect to {valkeyCacheHostPort}\n";
                //    string status = RedisValkeyCache.ValKeyInstance.Status;
                //    testReport += $"{DateTime.Now.Area23DateTimeWithMillis()}: ConnectionMulitplexer.Status = {status}" + Environment.NewLine;
                //}

                testReport += DateTime.Now.Area23DateTimeWithMillis() + ": Preparing to set Dictionary<Guid, CContact> in cache." + Environment.NewLine;
                MemoryCache.CacheDict.SetValue<Dictionary<Guid, CContact>>("TestCache", dictCacheTest);
                testReport += DateTime.Now.Area23DateTimeWithMillis() + ": Added serialized json string to cache." + Environment.NewLine;

                Dictionary<Guid, CContact> outdict = (Dictionary<Guid, CContact>)MemoryCache.CacheDict.GetValue<Dictionary<Guid, CContact>>("TestCache");
                testReport += DateTime.Now.Area23DateTimeWithMillis() + ": Got Dictionary<Guid, CContact> from cache with " +
                    outdict.Keys.Count + " keys." + Environment.NewLine;
                foreach (CContact contact in outdict.Values)
                {
                    testReport += DateTime.Now.Area23DateTimeWithMillis() + ": Contact Cuid=" + contact.Cuid + " NameEmail=" +
                        contact.NameEmail + " Mobile=" + contact.Mobile + Environment.NewLine;
                }

                List<string> chatRooms = JsonChatRoom.GetJsonChatRoomsFromCache();
                string[] chatRoomArray = new string[chatRooms.Count];
                Array.Copy(chatRooms.ToArray(), 0, chatRoomArray, 0, chatRooms.Count);

                testReport += DateTime.Now.Area23DateTimeWithMillis() + ": Found " + chatRooms.Count + " chat room keys in cache." + Environment.NewLine;
                foreach (string room in chatRoomArray)
                {
                    try
                    {
                        Dictionary<long, string> dicTest = GetCachedMessageDict(room);
                        if (dicTest != null)
                        {
                            testReport += DateTime.Now.Area23DateTimeWithMillis() + ": chat room " + room + " with keys " + dicTest.Keys.Count + ": messages." + Environment.NewLine;
                        }
                        else
                        {
                            MemoryCache.CacheDict.RemoveKey(room);
                            chatRooms.Remove(room);
                        }
                    }
                    catch (Exception exChatRoom)
                    {
                        string exMsg = "loading chat room " + room + " failed. Exception: " + exChatRoom.Message + "." + Environment.NewLine;
                        testReport += DateTime.Now.Area23DateTimeWithMillis() + ": " + exMsg;
                        Area23Log.LogOriginMsgEx("TestCacheController", $"loading chat room {room} failed. Exception {exChatRoom.GetType()}.", exChatRoom);
                    }
                }
                MemoryCache.CacheDict.SetValue<List<string>>(Constants.CHATROOMS, chatRooms);
            }
            catch (Exception ex2)
            {
                string ex2Msg = "Exception " + ex2.GetType() + ": " + ex2.Message + "\n\t" + ex2.ToString() + "\n";
                Area23Log.LogOriginMsgEx("TestCacheController", "Exception " + ex2.GetType(), ex2);
                testReport += DateTime.Now.Area23DateTimeWithMillis() + ": " + ex2Msg;
            }
            

            return testReport;

        }

    }
}
