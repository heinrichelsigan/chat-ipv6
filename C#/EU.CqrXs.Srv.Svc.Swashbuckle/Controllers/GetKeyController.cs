using Area23.At.Framework.Core.Cache;
using Area23.At.Framework.Core.RedisCache;
using Area23.At.Framework.Core.Static;
using Area23.At.Framework.Core.Util;
using Microsoft.AspNetCore.Mvc;


namespace EU.CqrXs.Srv.Svc.Swashbuckle.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GetKeyController : CqrSrvControllerBase
    {

        private readonly ILogger<TestCacbeController> _logger;

        public GetKeyController(ILogger<TestCacbeController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetKey")]
        public string Get(string key)
        {
            Area23Log.LogOriginMsg("GetKeyController", "GetKeyController.Get(string key = " + key + ") started.\n");

            string vlKey = "";
            string testReport = DateTime.Now.Area23DateTimeWithMillis() + ": GetKey(" + key + ") => ";
            try
            {
                InitMethod();
                vlKey = MemoryCache.CacheDict.GetString(key);
                if (string.IsNullOrEmpty(vlKey))
                    vlKey = RedisValkeyCache.ValKeyInstance.GetString(key);
                testReport += vlKey;
            }
            catch (Exception ex1)
            {
                testReport += DateTime.Now.Area23DateTimeWithMillis() + ": Exception " + ex1.GetType() + ": " + ex1.Message + "\n\t" + ex1 + "\n";
            }

            return string.IsNullOrEmpty(vlKey) ? testReport : vlKey;

        }

    }
}
