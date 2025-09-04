using Area23.At.Framework.Core.Static;
using System.Configuration;

namespace Area23.At.Framework.Core.RedisCache
{
    /// <summary>
    /// enum persist type
    /// </summary>
    public enum CacheType
    {
        None = 0,
        AppDomain = 1,
        RedisValkey = 2,       
        JsonFile = 3,
        ApplicationState = 4
        //RedisMS = 5
    }

    /// <summary>
    /// PersistInCache is a static class, which loads persistence type from Web.Config or App.Config 
    /// </summary>
    public static class CacheTypeHelper
    {
        private static readonly CacheType _cacheType = CacheType.AppDomain;


        /// <summary>
        /// returns where message is persisted
        /// </summary>
        public static CacheType CacheKind { get => _cacheType; }

        /// <summary>
        /// static ctor
        /// </summary>
        static CacheTypeHelper()
        {
            string cacheWhere = "AppDomain";
            if (ConfigurationManager.AppSettings[Constants.PERSIST_MSG_IN] != null)
                cacheWhere = (string)ConfigurationManager.AppSettings[Constants.PERSIST_MSG_IN].ToString();

            if (!Enum.TryParse<CacheType>(cacheWhere, out _cacheType))
                _cacheType = CacheType.AppDomain;
        }
    }

}
