using Area23.At.Framework.Core.Static;
using System.Collections.Concurrent;

namespace Area23.At.Framework.Core.Cache
{

    /// <summary>
    /// CacheHashDict an application cache implemented with a <see cref="ConcurrentDictionary{string, CacheValue}"/> saved in <see cref="AppDomain.CurrentDomain"/>
    /// </summary>
    public class AppDomainCacheDict : MemCacheDict
    {

        //protected internal static readonly new Lazy<MemCacheDict> _instance = new Lazy<MemCacheDict>(() => new AppDomainCacheDict());

        //public static new MemCacheDict CacheDict => _instance.Value;

        /// <summary>
        /// public property get accessor for <see cref="_appDict"/> stored in <see cref="AppDomain.CurrentDomain"/>
        /// </summary>
        protected override ConcurrentDictionary<string, CacheValue> AppDict
        {
            get
            {
                _appDict = (ConcurrentDictionary<string, CacheValue>)AppDomain.CurrentDomain.GetData(Constants.APP_CONCURRENT_DICT);
                if (_appDict == null)
                {
                    _appDict = new ConcurrentDictionary<string, CacheValue>();
                    AppDomain.CurrentDomain.SetData(Constants.APP_CONCURRENT_DICT, _appDict);
                }

                return _appDict;
            }
            set
            {
                if (value != null && value.Count > 0)
                {
                    _appDict = value;
                    AppDomain.CurrentDomain.SetData(Constants.APP_CONCURRENT_DICT, _appDict);
                }
            }
        }

        public AppDomainCacheDict()
        {
            if (AppDict == null) ;
        }

    }

}
