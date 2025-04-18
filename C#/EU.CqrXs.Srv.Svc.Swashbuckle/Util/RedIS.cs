using Area23.At.Framework.Core.Cqr;
using Area23.At.Framework.Core.Cqr.Msg;
using Area23.At.Framework.Core.Static;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;
using System.Web;
using static ICSharpCode.SharpZipLib.Zip.ExtendedUnixData;

namespace EU.CqrXs.Srv.Svc.Swashbuckle.Util
{

    /// <summary>
    /// Redis AWS elastic valkey cache singelton connector
    /// </summary>
    public class RedIS
    {
        private static readonly Lazy<RedIS> _instance = new Lazy<RedIS>(() => new RedIS());

        private static readonly object _lock = new object();

        ConnectionMultiplexer connMux;
        ConfigurationOptions options;
        string endpoint = "cqrcachecqrxseu-53g0xw.serverless.eus2.cache.amazonaws.com:6379";
        StackExchange.Redis.IDatabase db;

        public static RedIS ValKey => _instance.Value;

        private static HashSet<string> _allKeys = new HashSet<string>();
        public static string[] AllKeys { get => GetAllKeys().ToArray(); }

        public static string EndPoint
        {
            get
            {
                _instance.Value.endpoint = Constants.VALKEY_CACHE_HOST_PORT; // "cqrcachecqrxseu-53g0xw.serverless.eus2.cache.amazonaws.com:6379";                
                if (System.Configuration.ConfigurationManager.AppSettings != null && 
                    System.Configuration.ConfigurationManager.AppSettings[Constants.VALKEY_CACHE_HOST_PORT_KEY] != null)
                    _instance.Value.endpoint = (string)System.Configuration.ConfigurationManager.AppSettings[Constants.VALKEY_CACHE_HOST_PORT_KEY];
                return _instance.Value.endpoint;
            }
        }

        public static StackExchange.Redis.IDatabase Db
        {
            get
            {
                if (_instance.Value.db == null)
                    _instance.Value.db = ConnMux.GetDatabase();

                return _instance.Value.db;
            }
        }

        public static StackExchange.Redis.ConnectionMultiplexer ConnMux
        {
            get
            {
                if (_instance.Value.connMux == null)
                {
                    if (_instance.Value.options == null)
                        _instance.Value.options = new ConfigurationOptions
                        {
                            EndPoints = { EndPoint },
                            Ssl = true
                        };
                    _instance.Value.connMux = ConnectionMultiplexer.Connect(_instance.Value.options);
                }
                return _instance.Value.connMux;
            }
        }


        /// <summary>
        /// default parameterless constructor for RedIsValKey cache singleton 
        /// </summary>
        public RedIS()
        {
            endpoint = Constants.VALKEY_CACHE_HOST_PORT; // "cqrcachecqrxseu-53g0xw.serverless.eus2.cache.amazonaws.com:6379";
            if (System.Configuration.ConfigurationManager.AppSettings != null &&
                System.Configuration.ConfigurationManager.AppSettings[Constants.VALKEY_CACHE_HOST_PORT_KEY] != null)
            {
                endpoint = (string)System.Configuration.ConfigurationManager.AppSettings[Constants.VALKEY_CACHE_HOST_PORT_KEY];
            }
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
        public void SetString(string redIsKey, string redIsString, TimeSpan? expiry = null, bool keepTtl = false, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            Db.StringSet(redIsKey, redIsString, expiry, keepTtl, when, flags);
            
            var allRedIsKeys = GetAllKeys();
            lock (_lock)
            {
                if (!allRedIsKeys.Contains(redIsKey))
                {
                    allRedIsKeys.Add(redIsKey);
                    string jsonVal = JsonConvert.SerializeObject(allRedIsKeys.ToArray());
                    Db.StringSet("AllKeys", jsonVal, null, false, When.Always, CommandFlags.None);
                    _allKeys = allRedIsKeys;
                }
            }

        }


        /// <summary>
        /// SetKey<typeparamref name="T"/> sets a genric type T with a referenced key
        /// </summary>
        /// <typeparam name="T">generic type or class</typeparam>
        /// <param name="redIsKey">key for cache</param>
        /// <param name="tValue">Generic value to set</param>
        /// <param name="expiry"></param>
        /// <param name="keepTtl"></param>
        /// <param name="when"></param>
        /// <param name="flags"></param>
        public void SetKey<T>(string redIsKey, T tValue, TimeSpan? expiry = null, bool keepTtl = false, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            string jsonVal = JsonConvert.SerializeObject(tValue);
            SetString(redIsKey, jsonVal, expiry, keepTtl, when, flags);
        }

        /// <summary>
        /// GetKey<typeparamref name="T"/> gets a generic class type T from redis cache with key
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="redIsKey">key</param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public T GetKey<T>(string redIsKey, CommandFlags flags = CommandFlags.None)
        {
            string jsonVal = Db.StringGet(redIsKey, flags);
            T tval = default(T);
            if (jsonVal != null)
            {
                tval = JsonConvert.DeserializeObject<T>(jsonVal);
            }
            return tval;
        }

        /// <summary>
        /// DeleteKey delete entry referenced at key
        /// </summary>
        /// <param name="redIsKey">key</param>
        /// <param name="flags"><see cref="CommandFlags.FireAndForget"/> as default</param>
        public void DeleteKey(string redIsKey, CommandFlags flags = CommandFlags.FireAndForget)
        {            
            var allRedIsKeys = GetAllKeys();
            lock (_lock)
            {
                if (allRedIsKeys.Contains(redIsKey))
                {
                    allRedIsKeys.Remove(redIsKey);
                    string jsonVal = JsonConvert.SerializeObject(allRedIsKeys.ToArray());
                    Db.StringSet("AllKeys", jsonVal, null, false, When.Always, flags);
                    _allKeys = allRedIsKeys;
                }
                try
                {
                    Db.StringGetDelete(redIsKey, flags);
                }
                catch (Exception ex)
                {
                    CqrException.SetLastException(ex);
                }
            }
        }


        public bool ContainsKey(string redIsKey)
        {
            return  GetAllKeys().Contains(redIsKey);
        }



        protected static internal HashSet<string> GetAllKeys()
        {
            if (_allKeys == null || _allKeys.Count == 0)
            {
                string? jsonVal = Db.StringGet(Constants.ALL_KEYS, CommandFlags.None);
                string[]? keys = (string.IsNullOrEmpty(jsonVal)) ? new string[0] : JsonConvert.DeserializeObject<string[]>(jsonVal);
                if (keys != null && keys.Length > 0)
                    _allKeys = new HashSet<string>(keys);
            }

            return _allKeys ?? new HashSet<string>();
        }


    }

}