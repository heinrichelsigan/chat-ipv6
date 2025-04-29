using Area23.At.Framework.Core.Static;
using System.Collections.Concurrent;

namespace Area23.At.Framework.Core.Cache
{

    /// <summary>
    /// AppDomainCache an application cache implemented with a <see cref="ConcurrentDictionary{string, CacheValue}"/>
    /// </summary>
    public class AppDomainCache : MemoryCache
    {

        /// <summary>
        /// public property get accessor for <see cref="_appDict"/> stored in <see cref="AppDomain.CurrentDomain"/>
        /// </summary>
        protected override ConcurrentDictionary<string, CacheValue> AppDict
        {
            get
            {
                _appDict = (ConcurrentDictionary<string, CacheValue>)AppDomain.CurrentDomain.GetData(APP_CONCURRENT_DICT);
                if (_appDict == null)
                {
                    lock (_lock)
                    {
                        _appDict = new ConcurrentDictionary<string, CacheValue>();
                        AppDomain.CurrentDomain.SetData(APP_CONCURRENT_DICT, _appDict);
                    }
                }

                return _appDict;
            }
            set
            {
                if (value != null && value.Count > 0)
                {
                    lock (_lock)
                    {
                        _appDict = value;
                        AppDomain.CurrentDomain.SetData(APP_CONCURRENT_DICT, _appDict);
                    }
                }
            }
        }

        public AppDomainCache()
        {
            if (AppDict == null) ;
        }
    }

}
