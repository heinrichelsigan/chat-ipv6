﻿using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Area23.At.Framework.Core.Cache
{

    /// <summary>
    /// CacheHashDict an application cache implemented with a <see cref="ConcurrentDictionary{string, CacheValue}"/> saved in memory only at runtime
    /// derive from <see cref="MemoryCache"/> and implement your own cache by implementing a new variant for property <see cref="AppDict"/>
    /// </summary>
    public abstract class MemoryCache
    {
        
        public const string APP_CONCURRENT_DICT = "APP_CONCURRENT_DICT";
        protected internal static readonly Lock _lock = new Lock();

        protected internal static readonly Lazy<MemoryCache> _instance;

        public static MemoryCache CacheDict  => _instance.Value;
        

        /// <summary>
        /// private <see cref="ConcurrentDictionary{string, CacheValue}"/> 
        /// </summary>
        protected internal static ConcurrentDictionary<string, CacheValue> _appDict = new ConcurrentDictionary<string, CacheValue>();

        protected internal static readonly string CacheVariant = "MemoryCache";

        /// <summary>
        /// public property get accessor for <see cref="_appDict"/> stored in <see cref="AppDomain.CurrentDomain"/>
        /// </summary>
        protected virtual ConcurrentDictionary<string, CacheValue> AppDict
        {
            get
            {
                // _appDict =  (ConcurrentDictionary<string, CacheValue>) get it where to get it
                if (_appDict == null)
                {
                    _appDict = new ConcurrentDictionary<string, CacheValue>();
                    // where to set it _appDict
                }
                return _appDict;
            }
            set
            {
                if (value != null && value.Count > 0)
                    _appDict = value;

                // if (_appDict != null && _appDict.Count > 0)
                //      set it where to set it _appDict
            }
        }

        /// <summary>
        /// Get all keys from <see cref="AppDict"/> which is implemented as a <see cref="ConcurrentDictionary{string, CacheValue}"/>
        /// </summary>
        public virtual string[] AllKeys { get => AppDict.Keys.ToArray(); }

        public object? this[string ckey]
        {
            get => (AppDict.ContainsKey(ckey) && AppDict.TryGetValue(ckey, out CacheValue? cvalue)) ? cvalue._Value : null;
            set
            {                
                object? ovalue = value;
                Type? otype = value?.GetType();
                if (AppDict.ContainsKey(ckey) && AppDict.TryGetValue(ckey, out CacheValue oldValue))
                    AppDict.TryRemove(ckey, out oldValue);

                AppDict.TryAdd(ckey, new CacheValue(ovalue, otype));
            }
        }

        /// <summary>
        /// static ctor
        /// </summary>
        static MemoryCache()
        {

            PersistType cacheType = PersistInCache.CacheType;
            CacheVariant = cacheType.ToString();

            switch (cacheType)
            {
                case PersistType.JsonFile:                    
                    _instance = new Lazy<MemoryCache>(() => new JsonFileCache());
                    break;
                case PersistType.Redis:
                    // TODO: Redis                   
                    _instance = new Lazy<MemoryCache>(() => new RedisCache());
                    break;
                case PersistType.AppDomain:
                case PersistType.ApplicationState:
                default:
                    _instance = new Lazy<MemoryCache>(() => new AppDomainCache());
                    break;
            }
        }


        /// <summary>
        /// Static constructor
        /// </summary>
        public MemoryCache()
        {
            _appDict = new ConcurrentDictionary<string, CacheValue>();
        }

        /// <summary>
        /// Gets a value from <see cref="ConcurrentDictionary{string, CacheValue}"/> stored <see cref="System.AppDomain.CurrentDomain"/>
        /// </summary>
        /// <typeparam name="T">generic type of cached value</typeparam>
        /// <param name="ckey">cache key</param>
        /// <returns>generic cached value stored at key</returns>
        public virtual T GetValue<T>(string ckey)
        {
            T tvalue = (AppDict.ContainsKey(ckey) && AppDict.TryGetValue(ckey, out var cvalue)) ? cvalue.GetValue<T>() : default(T);

            return tvalue;
        }

        /// <summary>
        /// Sets a generic value to <see cref="ConcurrentDictionary{string, CacheValue}"/> stored <see cref="System.AppDomain.CurrentDomain"/>
        /// </summary>
        /// <typeparam name="T">generic type of cached value</typeparam>
        /// <param name="ckey">cache key</param>
        /// <param name="cvalue">generic value to stored at key in cache</param>
        /// <returns>true, if add or updated succeeded, otherwise false</returns>
        public virtual bool SetValue<T>(string ckey, T tvalue)
        {
            bool addedOrUpdated = false;

            if (string.IsNullOrEmpty(ckey) || tvalue == null)
                return addedOrUpdated;

            CacheValue cvalue = new CacheValue();
            cvalue.SetValue<T>(tvalue);

            if (!AppDict.ContainsKey(ckey))
                addedOrUpdated = AppDict.TryAdd(ckey, cvalue);
            else if (AppDict.TryGetValue(ckey, out CacheValue oldValue))
                addedOrUpdated = _appDict.TryUpdate(ckey, cvalue, oldValue);

            // MAYBE SHORTER BUT NOBODY CAN QUICK READ AND UNDERSTAND THIS
            // addedOrUpdated = (!AppCache.ContainsKey(ckey)) ? AppCache.TryAdd(ckey, cvalue) :
            //    (AppCache.TryGetValue(ckey, out CacheValue oldValue)) ? _appCache.TryUpdate(ckey, cvalue, oldValue) : false;

            if (addedOrUpdated)
                AppDict = _appDict;  // saves the modified ConcurrentDictionary{string, CacheValue} back to AppDomain

            return addedOrUpdated;
        }

        /// <summary>
        /// Looks, if  <see cref="ConcurrentDictionary{string, CacheValue}"/>  contains the key
        /// </summary>
        /// <param name="ckey">lookup key</param>
        /// <returns>true, if ckey is not null or empty and <see cref="AppDict"/> contains ckey, otherwise false</returns>
        public virtual bool ContainsKey(string ckey)
        {
            return (!string.IsNullOrEmpty(ckey) && AppDict.ContainsKey(ckey));
        }

        /// <summary>
        /// RemoveKey removes a key value pair from <see cref="AppDict"/>
        /// </summary>
        /// <param name="ckey">key to remove</param>
        /// <returns>true, if key value pair was successfully removed or <see cref="AppDict"/> doesn't contain anymore ckey;
        /// false if ckey is <see cref="null"/> or <see cref="string.Empty"/> or removing ckey from <see cref="ConcurrentDictionary{string, CacheValue}"/> failed.</returns>
        public virtual bool RemoveKey(string ckey)
        {
            bool success = false;
            if (string.IsNullOrEmpty(ckey))
                return success;

            if ((success = !AppDict.ContainsKey(ckey)) == false)
                if ((success = AppDict.TryRemove(ckey, out CacheValue cvalue)) == true)
                    AppDict = _appDict; // saves the modified ConcurrentDictionary{string, CacheValue} back to AppDomain

            return success;
        }

    }

}
