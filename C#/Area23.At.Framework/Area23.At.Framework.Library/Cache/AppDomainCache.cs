using Area23.At.Framework.Library.Static;
using Area23.At.Framework.Library.Util;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Area23.At.Framework.Library.Cache
{

    /// <summary>
    /// AppDomainCache an application cache implemented with a <see cref="ConcurrentDictionary{string, CacheValue}"/>
    /// </summary>
    public class AppDomainCache : MemoryCache
    {
        protected internal static readonly object _smartLock = new object();

        public static new string CacheVariant = "AppDomainCache";
        public override string CacheType => "AppDomainCache";

        /// <summary>
        /// public property get accessor for <see cref="_appDict"/> stored in <see cref="AppDomain.CurrentDomain"/>
        /// </summary>
        protected override ConcurrentDictionary<string, CacheValue> AppDict
        {
            get
            {
                lock (_smartLock)
                {
                    try
                    {
                        _appDict = (ConcurrentDictionary<string, CacheValue>)AppDomain.CurrentDomain.GetData(APP_CONCURRENT_DICT);
                    }
                    catch (Exception appDomDictEx)
                    {
                        Area23Log.LogStatic(appDomDictEx);
                    }

                    if (_appDict == null)
                    {
                        lock (_lock)
                        {                        
                            _appDict = new ConcurrentDictionary<string, CacheValue>();
                            AppDomain.CurrentDomain.SetData(APP_CONCURRENT_DICT, _appDict);
                        }
                    }
                }

                return _appDict;
            }
            set
            {
                lock (_smartLock)
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
        }

        public AppDomainCache(PersistType cacheType = PersistType.AppDomain)
        {
            if (AppDict == null) ;
        }

    }

}
