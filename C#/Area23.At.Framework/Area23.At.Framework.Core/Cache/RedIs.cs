using Area23.At.Framework.Core.Static;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Nodes;
using Area23.At.Framework.Core.Cqr;
using Newtonsoft.Json.Linq;
using static ICSharpCode.SharpZipLib.Zip.ExtendedUnixData;
using System.Windows.Input;

namespace Area23.At.Framework.Core.Cache
{

    /// <summary>
    /// RedIs AWS elastic valkey cache singelton connector
    /// </summary>
    public class RedIs : MemCacheDict
    {
        // private static readonly new Lazy<MemCacheDict> _instance = new Lazy<MemCacheDict>(() => new RedIs());

        private static readonly object _lock = new object();

        ConnectionMultiplexer connMux;
        ConfigurationOptions options;
        string endpoint = "cqrcachecqrxseu-53g0xw.serverless.eus2.cache.amazonaws.com:6379";
        StackExchange.Redis.IDatabase db;

        public static MemCacheDict ValKey => _instance.Value;

        private static HashSet<string> _allKeys = new HashSet<string>();
        public override string[] AllKeys { get => GetAllKeys().ToArray(); }

        public static string EndPoint
        {
            get
            {
                ((RedIs)(_instance.Value)).endpoint = Constants.VALKEY_CACHE_HOST_PORT; // "cqrcachecqrxseu-53g0xw.serverless.eus2.cache.amazonaws.com:6379";                
                if (ConfigurationManager.AppSettings != null && ConfigurationManager.AppSettings[Constants.VALKEY_CACHE_HOST_PORT_KEY] != null)
                    ((RedIs)(_instance.Value)).endpoint = (string)ConfigurationManager.AppSettings[Constants.VALKEY_CACHE_HOST_PORT_KEY];
                return ((RedIs)(_instance.Value)).endpoint;
            }
        }

        public static StackExchange.Redis.IDatabase Db
        {
            get
            {
                if (((RedIs)(_instance.Value)).db == null)
                    ((RedIs)(_instance.Value)).db = ConnMux.GetDatabase();

                return ((RedIs)(_instance.Value)).db;
            }
        }

        public static StackExchange.Redis.ConnectionMultiplexer ConnMux
        {
            get
            {
                if (((RedIs)(_instance.Value)).connMux == null)
                {
                    if (((RedIs)(_instance.Value)).options == null)
                        ((RedIs)(_instance.Value)).options = new ConfigurationOptions
                        {
                            EndPoints = { EndPoint },
                            Ssl = true
                        };
                    ((RedIs)(_instance.Value)).connMux = ConnectionMultiplexer.Connect(((RedIs)(_instance.Value)).options);
                }
                return ((RedIs)(_instance.Value)).connMux;
            }
        }


        /// <summary>
        /// default parameterless constructor for RedIsValKey cache singleton 
        /// </summary>
        public RedIs()
        {
            endpoint = Constants.VALKEY_CACHE_HOST_PORT; // "cqrcachecqrxseu-53g0xw.serverless.eus2.cache.amazonaws.com:6379";
            if (ConfigurationManager.AppSettings != null && ConfigurationManager.AppSettings[Constants.VALKEY_CACHE_HOST_PORT_KEY] != null)
                endpoint = (string)ConfigurationManager.AppSettings[Constants.VALKEY_CACHE_HOST_PORT_KEY];
            options = new ConfigurationOptions
            {
                EndPoints = { endpoint },
                Ssl = true
            };
            if (connMux == null)
                connMux = ConnectionMultiplexer.Connect(options);
            if (db == null)
                db = connMux.GetDatabase();

        }


        /// <summary>
        /// GetString gets a string value by redis key
        /// </summary>
        /// <param name="redIsKey">key</param>
        /// <param name="flags"><see cref="CommandFlags"/></param>
        /// <returns>(<see cref="string"/>) value for key redIsKey</returns>
        public string GetString(string redIsKey, CommandFlags flags = CommandFlags.None)
        {
            string redIsString = Db.StringGet(redIsKey, flags);
            return redIsString;
        }

        /// <summary>
        /// SetString set key with string value
        /// </summary>
        /// <param name="redIsKey">key for string/param>
        /// <param name="redIsString"></param>
        /// <param name="expiry"></param>
        /// <param name="keepTtl"></param>
        /// <param name="when"></param>
        /// <param name="flags"></param>
        public bool SetString(string redIsKey, string redIsString, TimeSpan? expiry = null, bool keepTtl = false, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            bool success = false;
            lock (_lock)
            {
                var allRedIsKeys = GetAllKeys();
                success = Db.StringSet(redIsKey, redIsString, expiry, when, flags);

                if (success && !allRedIsKeys.Contains(redIsKey))
                {
                    allRedIsKeys.Add(redIsKey);
                    string jsonVal = JsonConvert.SerializeObject(AllKeys);
                    success = Db.StringSet(Constants.ALL_KEYS, jsonVal, null, keepTtl, When.Always, CommandFlags.None);
                    _allKeys = allRedIsKeys;
                }
            }

            return success;
        }

        /// <summary>
        /// SetValue sets value to cache
        /// </summary>
        /// <typeparam name="T">typeparameter</typeparam>
        /// <param name="ckey">key to set</param>
        /// <param name="tvalue">generic value</param>
        /// <returns>success on true</returns>
        public override bool SetValue<T>(string ckey, T tvalue)
        {
            TimeSpan? expiry = null;
            bool keepTtl = false;
            When when = When.Always;
            CommandFlags flags = CommandFlags.None;
            string jsonVal = JsonConvert.SerializeObject(tvalue);
            bool success = SetString(ckey, jsonVal, expiry, keepTtl, when, flags);

            return success;
            // return base.SetValue(ckey, tvalue);
        }

        /// <summary>
        /// gets a generic class type T from redis cache with key
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ckey">rediskey</param>
        /// <returns>T value/returns>
        public override T GetValue<T>(string ckey)
        {
            CommandFlags flags = CommandFlags.None;
            string jsonVal = Db.StringGet(ckey, flags);
            T tval = default(T);
            if (jsonVal != null)
            {
                tval = JsonConvert.DeserializeObject<T>(jsonVal);
            }
            
            return tval;
            // return base.GetValue<T>(ckey);
        }

        

        /// <summary>
        /// DeleteKey delete entry referenced at key
        /// </summary>
        /// <param name="redIsKey">key</param>
        /// <param name="flags"><see cref="CommandFlags.FireAndForget"/> as default</param>
        public override bool RemoveKey(string redIsKey)
        {
            CommandFlags flags = CommandFlags.FireAndForget;
            lock (_lock)
            {
                var allRedIsKeys = GetAllKeys();
                if (allRedIsKeys.Contains(redIsKey))
                {
                    allRedIsKeys.Remove(redIsKey);
                    string jsonVal = JsonConvert.SerializeObject(allRedIsKeys.ToArray());
                    Db.StringSet("AllKeys", jsonVal, null, false, When.Always, flags);
                    _allKeys = allRedIsKeys;
                }
                try
                {
                    TimeSpan span = new TimeSpan(0, 0, 1);
                    Db.StringGetDelete(redIsKey, flags);
                }
                catch (Exception ex)
                {
                    CqrException.SetLastException(ex);
                    return false;
                }
            }

            return true;
        }


        /// <summary>
        /// ContainsKey check if <see cref="Constants.ALL_KEYS">AllKeys</see> key contains element redIsKey
        /// </summary>
        /// <param name="ckey">redIsKey to search</param>
        /// <returns>true, if cache contains key, otherwise false</returns>
        public override bool ContainsKey(string ckey)
        {
            if (GetAllKeys().Contains(ckey))
            {
                string redIsString = Db.StringGet(ckey, CommandFlags.None);
                if (!string.IsNullOrEmpty(redIsString))
                    return true;
            }

            return false;
        }


        /// <summary>
        /// GetAllKeys returns <see cref="HashSet{string}"/></string> <see cref="_allKeys"/>
        /// </summary>
        /// <returns>returns <see cref="HashSet{string}"/></string> <see cref="_allKeys"/></returns>
        public static HashSet<string> GetAllKeys()
        {
            if (_allKeys == null || _allKeys.Count == 0)
            {
                string jsonVal = Db.StringGet(Constants.ALL_KEYS, CommandFlags.None);
                string[] keys = (jsonVal != null) ? JsonConvert.DeserializeObject<string[]>(jsonVal) : new string[0];
                if (keys != null && keys.Length > 0)
                    _allKeys = new HashSet<string>(keys);
            }

            return _allKeys;
        }

    }

}
