using Area23.At.Framework.Library.Static;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Area23.At.Framework.Library.Cache
{

    /// <summary>
    /// CacheHashDict an application cache implemented with a <see cref="ConcurrentDictionary{string, CacheValue}"/> saved in <see cref="AppDomain.CurrentDomain"/>
    /// </summary>
    public class AppDomainCache : MemCacheDict
    {

        /// <summary>
        /// public property get accessor for <see cref="_appDict"/> stored in <see cref="AppDomain.CurrentDomain"/>
        /// </summary>
        protected new static ConcurrentDictionary<string, CacheValue> AppDict
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
                    _appDict = value;

                if (_appDict != null && _appDict.Count > 0)
                    AppDomain.CurrentDomain.SetData(Constants.APP_CONCURRENT_DICT, _appDict);
            }
        }

    }

}
