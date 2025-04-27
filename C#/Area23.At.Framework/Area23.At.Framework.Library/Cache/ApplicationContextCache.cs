using Area23.At.Framework.Library.Static;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Area23.At.Framework.Library.Cache
{

    /// <summary>
    /// CacheHashDict an application cache implemented with a <see cref="ConcurrentDictionary{string, CacheValue}"/> saved in <see cref="HttpApplicationState"/>
    /// </summary>
    public class ApplicationContextCache : MemCacheDict
    {

        /// <summary>
        /// public property get accessor for <see cref="_appDict"/> stored in <see cref="HttpApplicationState"/>
        /// </summary>
        protected new static ConcurrentDictionary<string, CacheValue> AppDict
        {
            get
            {
                _appDict = (ConcurrentDictionary<string, CacheValue>)HttpContext.Current.Application[Constants.APP_CONCURRENT_DICT];
                if (_appDict == null)
                {
                    _appDict = new ConcurrentDictionary<string, CacheValue>();
                    HttpContext.Current.Application[Constants.APP_CONCURRENT_DICT] = _appDict;
                }

                return _appDict;
            }
            set
            {
                if (value != null && value.Count > 0)
                    _appDict = value;

                if (_appDict != null && _appDict.Count > 0)
                    HttpContext.Current.Application[Constants.APP_CONCURRENT_DICT] = _appDict;
            }
        }

    }

}
