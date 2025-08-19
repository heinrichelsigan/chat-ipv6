using Newtonsoft.Json;
using StackExchange.Redis;
using System.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Threading;
using Area23.At.Framework.Core.Static;

namespace Area23.At.Framework.Core.Cache
{

    /// <summary>
    /// enum persist type
    /// </summary>
    public enum Persist
    {
        None = 0,
        App = 1,
        Redis = 2,
        Json = 3,
        State = 4
        //RedisMS = 5
    }

    public static class PersistCache
    {
        private static Persist _cache= Persist.App;

        public const string PERSIST_MSG_IN = "PersistMsgIn";

        /// <summary>
        /// returns where message is persisted
        /// </summary>
        public static Persist CacheType { get => _cache; }

        /// <summary>
        /// static ctor
        /// </summary>
        static PersistCache()
        {
            string persistWhere = "AppDomain";
            if (ConfigurationManager.AppSettings[Constants.PERSIST_MSG_IN] != null)
                persistWhere = (string)ConfigurationManager.AppSettings[PERSIST_MSG_IN].ToString();

            if (!Enum.TryParse<Persist>(persistWhere, out _cache))
                _cache = PersistType.AppDomain;
        }

        public static void SetRedis(PersistType cacheType = Persist.Redis { _cache = cacheType; }

    }


    /// <summary>
    /// CacheTypVal any cached value.
    /// Use default empty ctor <see cref="CacheTypVal()"/> and
    /// <see cref="SetValue{T}(T)"/> to set the cached value;
    /// setting cache value via <see cref="CacheTypVal(object, Type)"/> ctor is obsolete.
    /// Use <see cref="GetValue{T}"/> to get the cached value
    /// </summary>
    [Serializable]
    public class CacheTypVal
    {

        public object? _Value { get; protected internal set; }
        public Type? _Type { get; protected internal set; }

        /// <summary>
        /// Empty default ctor
        /// </summary>
        public CacheTypVal()
        {
            _Type = null;
            _Value = null;
        }

        /// <summary>
        /// Obsolete ctor, please use default empty ctor <see cref="CacheTypVal()"/>
        /// and then <see cref="SetValue{T}(T)"/> to set a cached value instead.
        /// </summary>
        /// <param name="ovalue"><see cref="object" /> ovalue</param>
        /// <param name="atype"><see cref="Type"/> atype</param>
        [Obsolete("Don't use ctor CacheTypeValue(object, Type) to set a cache value, use SetValue<T>(T tvalue) instead.", false)]
        public CacheTypVal(object ovalue, Type atype)
        {
            _Type = atype;
            _Value = ovalue;
        }

        /// <summary>
        /// gets the <see cref="Type"/> of generic cached value
        /// </summary>
        /// <returns><see cref="Type"/> of generic value or null if cached value is <see cref="null"/></returns>
        public Type? GetType() => _Type;


        /// <summary>
        /// Get a value from cache
        /// </summary>
        /// <typeparam name="T">generic type of value passed by typeparameter</typeparam>
        /// <returns>generic T value</returns>
        /// <exception cref="InvalidOperationException">thrown, when cached value isn't of typeof(T)</exception>
        public T? GetValue<T>()
        {
            T? tvalue;
            if (typeof(T) == _Type || typeof(T).IsSubclassOf(_Type))
                tvalue = (T?)_Value;
            else
                throw new InvalidOperationException($"typeof(T) = {typeof(T)} while _type = {_Type}");

            return tvalue ?? default(T);
        }



        /// <summary>
        /// Sets a generic cached value
        /// </summary>
        /// <typeparam name="T">generic type of value passed by typeparameter</typeparam>
        /// <param name="tvalue">generic value to set cached</param>
        public void SetValue<T>(T tvalue)
        {
            _Type = typeof(T);
            _Value = (object)tvalue;
        }

    }


    /// <summary>
    /// CacheHashDict an application cache implemented with a <see cref="ConcurrentDictionary{string, CacheTypVal}"/> saved in memory only at runtime
    /// derive from <see cref="MemCache"/> and implement your own cache by implementing a new variant for property <see cref="AppDict"/>
    /// </summary>
    public class MemCache
    {

        public const string APP_CONCURRENT_DICT = "APP_CONCURRENT_DICT";
        public const int CACHE_READ_UPDATE_INTERVAL = 120;
        protected internal static readonly object _lock = new object(), _outerlock = new object();

        protected internal DateTime _lastCacheRW = DateTime.Now;

        protected internal TimeSpan _timePassedSinceLastRW = TimeSpan.Zero;

        protected internal static HashSet<string> _allKeys = new HashSet<string>();

        public static string CacheVariant = "MemCache";

        protected internal static bool _onceCreated = false;

        protected internal Persist _persist;
        public virtual string CacheType { get => _persist.ToString(); }

        protected internal static Lazy<MemCache> _instance;
        public static MemCache CacheDict
        {
            get
            {
                if (_instance == null && !_onceCreated)
                {
                    Persist cacheType = PersistCache.CacheType;
                    CreateInstance(cacheType);
                }

                return _instance.Value;
            }
        }


        /// <summary>
        /// private <see cref="ConcurrentDictionary{string, CacheTypVal}"/> 
        /// </summary>
        protected ConcurrentDictionary<string, CacheTypVal> _appDict = new ConcurrentDictionary<string, CacheTypVal>();

        /// <summary>
        /// public property get accessor for <see cref="_appDict"/> stored in <see cref="AppDomain.CurrentDomain"/>
        /// </summary>
        protected virtual ConcurrentDictionary<string, CacheTypVal> AppDict
        {
            get => LoadDictionaryCache();           // get, where to get it (_appDict from cache)
            set => SaveDictionaryToCache(value);    // set it where to set it (value to _appDict to cache)
        }


        /// <summary>
        /// Indexer with string 
        /// </summary>
        /// <param name="ckey">key to lookup</param>
        /// <returns>object or null, thou must cast object</returns>
        public string this[string ckey]
        {
            get => GetString(ckey);
            set => SetString(ckey, value);
        }

        /// <summary>
        /// Get all keys from <see cref="AppDict"/> which is implemented as a <see cref="ConcurrentDictionary{string, CacheTypVal}"/>
        /// </summary>
        public virtual string[] AllKeys { get => GetAllKeys().ToArray(); }


        /// <summary>
        /// ctor
        /// </summary>
        static MemCache()
        {
            Persist persistType = PersistCache.CacheType;
            CreateInstance(persistType);
        }


        public MemCache()
        {
            _persist = PersistCache.CacheType;
        }

        protected internal static void CreateInstance(Persist cacheType)
        {
            lock (_lock)
            {
                switch (cacheType)
                {
                    case Persist.Json:
                        _instance = new Lazy<MemCache>(() => new JsonCache());
                        break;
                    case Persist.Redis:
                        _instance = new Lazy<MemCache>(() => new RedisCache());
                        break;
                    //case Persist.RedisMS:
                    //    _instance = new Lazy<MemCache>(() => new RedisMSCache());
                    //    break;
                    case Persist.App:
                    case Persist.State:
                    default:
                        _instance = new Lazy<MemCache>(() => new AppCache());
                        break;
                }

                _onceCreated = true;
                CacheVariant = cacheType.ToString();
            }

        }

        /// <summary>
        /// get, where to get it (_appDict from cache)
        /// </summary>
        /// <param name="repeatLoadingPeriodically">if true, _appDict will be repeatedly loaded from cache <see cref="CACHE_READ_UPDATE_INTERVAL" /> in seconds</param>
        /// <returns><see cref="ConcurrentDictionary{string, CacheTypVal}"/> _appDict</returns>
        public virtual ConcurrentDictionary<string, CacheTypVal> LoadDictionaryCache(bool repeatLoadingPeriodically = false)
        {

            if (_appDict == null || _appDict.Count == 0)
            {
                _timePassedSinceLastRW = DateTime.Now.Subtract(_lastCacheRW);
                _lastCacheRW = DateTime.Now;
                _appDict = new ConcurrentDictionary<string, CacheTypVal>();
                // where to set it _appDict
            }

            return _appDict;
        }

        /// <summary>
        /// set where to set <see cref="ConcurrentDictionary{string, CacheTypVal}">it</see>  
        /// (value to _appDict to cache)
        /// </summary>
        /// <param name="cacheDict"><see cref="ConcurrentDictionary{string, CacheTypVal}"/></param>
        public virtual void SaveDictionaryToCache(ConcurrentDictionary<string, CacheTypVal> cacheDict)
        {
            if (cacheDict != null) //  && value.Count > 0
                _appDict = cacheDict;
        }

        
        #region virtual cache operations on _appDict methods

        public virtual string GetString(string ckey)
        {
            string valString = "";

            if (!string.IsNullOrEmpty(ckey))
            {
                _appDict = AppDict;
                lock (_lock)
                {
                    if (_appDict.ContainsKey(ckey) && _appDict.TryGetValue(ckey, out var cvalue))
                    {
                        if (cvalue != null)
                            valString = cvalue.ToString();
                    }
                }
            }

            return valString;
        }

        /// <summary>
        /// Gets a value from <see cref="ConcurrentDictionary{string, CacheTypVal}"/> stored <see cref="System.AppDomain.CurrentDomain"/>
        /// </summary>
        /// <typeparam name="T">generic type of cached value</typeparam>
        /// <param name="ckey">cache key</param>
        /// <returns>generic cached value stored at key</returns>
        public virtual T GetValue<T>(string ckey)
        {
            lock (_outerlock)
            {
                T tvalue = default(T);

                if (!string.IsNullOrEmpty(ckey))
                {
                    _appDict = AppDict;
                    lock (_lock)
                    {
                        if (_appDict.ContainsKey(ckey) && _appDict.TryGetValue(ckey, out var cvalue))
                        {
                            if (cvalue != null)
                            {
                                tvalue = cvalue.GetValue<T>();
                            }
                        }
                    }
                }

                return tvalue;
            }

        }

        public virtual Nullable<T> GetNullableValue<T>(string ckey) where T : struct
        {
            Nullable<T> nullableT = null;

            if (!string.IsNullOrEmpty(ckey))
            {
                lock (_lock)
                {
                    if (_appDict.ContainsKey(ckey) && _appDict.TryGetValue(ckey, out var cvalue))
                    {
                        if (cvalue != null)
                            nullableT = cvalue.GetNullableValue<T>();
                    }
                }
            }

            return nullableT;
        }

        public virtual bool SetString(string ckey, string svalue)
        {
            lock (_outerlock)
            {
                bool addedOrUpdated = false;

                if (string.IsNullOrEmpty(ckey) || svalue == null)
                    return addedOrUpdated;

                CacheTypVal cvalue = new CacheTypVal();
                cvalue.SetValue<string>(svalue);

                _appDict = LoadDictionaryCache(true);

                lock (_lock)
                {
                    if (!_appDict.ContainsKey(ckey))
                        addedOrUpdated = _appDict.TryAdd(ckey, cvalue);
                    else if (_appDict.TryGetValue(ckey, out CacheTypVal oldValue))
                        addedOrUpdated = _appDict.TryUpdate(ckey, cvalue, oldValue);

                    // MAYBE SHORTER BUT NOBODY CAN QUICK READ AND UNDERSTAND THIS
                    // addedOrUpdated = (!AppCache.ContainsKey(ckey)) ? AppCache.TryAdd(ckey, cvalue) :
                    //    (AppCache.TryGetValue(ckey, out CacheTypVal oldValue)) ? _appCache.TryUpdate(ckey, cvalue, oldValue) : false;

                    if (addedOrUpdated) // saves the modified ConcurrentDictionary{string, CacheTypVal} back to AppDomain
                        SaveDictionaryToCache(_appDict);
                }

                return addedOrUpdated;
            }
        }

        /// <summary>
        /// Sets a generic value to <see cref="ConcurrentDictionary{string, CacheTypVal}"/> stored <see cref="System.AppDomain.CurrentDomain"/>
        /// </summary>
        /// <typeparam name="T">generic type of cached value</typeparam>
        /// <param name="ckey">cache key</param>
        /// <param name="cvalue">generic value to stored at key in cache</param>
        /// <returns>true, if add or updated succeeded, otherwise false</returns>
        public virtual bool SetValue<T>(string ckey, T tvalue)
        {
            lock (_outerlock)
            {
                bool addedOrUpdated = false;

                if (string.IsNullOrEmpty(ckey) || tvalue == null)
                    return addedOrUpdated;

                CacheTypVal cvalue = new CacheTypVal();
                cvalue.SetValue<T>(tvalue);

                _appDict = LoadDictionaryCache(true);

                lock (_lock)
                {
                    if (!_appDict.ContainsKey(ckey))
                        addedOrUpdated = _appDict.TryAdd(ckey, cvalue);
                    else if (_appDict.TryGetValue(ckey, out CacheTypVal oldValue))
                        addedOrUpdated = _appDict.TryUpdate(ckey, cvalue, oldValue);

                    // MAYBE SHORTER BUT NOBODY CAN QUICK READ AND UNDERSTAND THIS
                    // addedOrUpdated = (!AppCache.ContainsKey(ckey)) ? AppCache.TryAdd(ckey, cvalue) :
                    //    (AppCache.TryGetValue(ckey, out CacheTypVal oldValue)) ? _appCache.TryUpdate(ckey, cvalue, oldValue) : false;

                    if (addedOrUpdated) // saves the modified ConcurrentDictionary{string, CacheTypVal} back to AppDomain
                        SaveDictionaryToCache(_appDict);
                }

                return addedOrUpdated;
            }
        }

        /// <summary>
        /// Looks, if  <see cref="ConcurrentDictionary{string, CacheTypVal}"/>  contains the key
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
        /// false if ckey is <see cref="null"/> or <see cref="string.Empty"/> or removing ckey from <see cref="ConcurrentDictionary{string, CacheTypVal}"/> failed.</returns>
        public virtual bool RemoveKey(string ckey)
        {
            lock (_outerlock)
            {
                bool success = false;
                if (string.IsNullOrEmpty(ckey))
                    return success;

                _appDict = AppDict;

                lock (_lock)
                {
                    if ((success = !_appDict.ContainsKey(ckey)) == false)
                        if ((success = _appDict.TryRemove(ckey, out CacheTypVal cvalue)) == true)
                            SaveDictionaryToCache(_appDict);  // saves the modified ConcurrentDictionary{string, CacheTypVal} back to AppDomain
                }

                return success;
            }
        }

        public virtual HashSet<string> GetAllKeys()
        {
            _allKeys = new HashSet<string>(AppDict.Keys.ToArray());
            return _allKeys;
        }


        #endregion virtual cache operations on _appDict methods

    }


    /// <summary>
    /// JsonCache an application cache implemented with <see cref="ConcurrentDictionary{string, CacheTypVal}"/> serialized with json    
    /// </summary>
    public class JsonCache : MemCache
    {

        protected internal static readonly object _smartLock = new object();
        const string JSON_APPCACHE_FILE = "AppCache.json";
        readonly static string JsonFullDirPath = LibPaths.SystemDirJsonPath;
        readonly static string JsonFullFilePath = Path.Combine(JsonFullDirPath, JSON_APPCACHE_FILE);

        public static new string CacheVariant = "JsonFileCache";

        protected static JsonSerializerSettings JsonSettings = new JsonSerializerSettings()
        {
            Formatting = Formatting.Indented,
            MaxDepth = 16,
            NullValueHandling = NullValueHandling.Include,
            MissingMemberHandling = MissingMemberHandling.Ignore,
            ObjectCreationHandling = ObjectCreationHandling.Auto,
            DateFormatHandling = DateFormatHandling.IsoDateFormat,
            DateParseHandling = DateParseHandling.DateTime,
            PreserveReferencesHandling = PreserveReferencesHandling.All,
            ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor
        };

        /// <summary>
        /// public property get accessor for <see cref="_appDict"/> stored in <see cref="AppDomain.CurrentDomain"/>
        /// </summary>
        protected override ConcurrentDictionary<string, CacheTypVal> AppDict
        {
            get => LoadDictionaryCache(true);
            set => SaveDictionaryToCache(value);
        }

        /// <summary>
        /// get, where to get it (_appDict from cache)
        /// </summary>
        /// <param name="repeatLoadingPeriodically">if true, _appDict will be repeatedly loaded from cache <see cref="CACHE_READ_UPDATE_INTERVAL" /> in seconds</param>
        /// <returns><see cref="ConcurrentDictionary{string, CacheTypVal}"/> _appDict</returns>        
        public override ConcurrentDictionary<string, CacheTypVal> LoadDictionaryCache(bool repeatLoadingPeriodically = false)
        {
            lock (_smartLock)
            {
                _timePassedSinceLastRW = DateTime.Now.Subtract(_lastCacheRW);

                try
                {
                    if (_appDict == null || _appDict.Count == 0 || (repeatLoadingPeriodically && _timePassedSinceLastRW.TotalSeconds > CACHE_READ_UPDATE_INTERVAL))
                    {
                        var mutex = StaticCacheMutex.TheStaticCacheMutex;
                        if (mutex != null && mutex.WaitOne(1024, false))
                        {
                            throw new SynchronizationLockException("Mutex " + mutex.ToString() + " blocks loading serialized json from " + JsonFullFilePath + ".");
                        }

                        lock (_lock)
                        {
                            if (!Directory.Exists(JsonFullDirPath))
                                Directory.CreateDirectory(JsonFullDirPath);

                            string jsonSerializedAppDict = (System.IO.File.Exists(JsonFullFilePath)) ? System.IO.File.ReadAllText(JsonFullFilePath) : "";
                            if (!string.IsNullOrEmpty(jsonSerializedAppDict))
                            {
                                _appDict = (ConcurrentDictionary<string, CacheTypVal>)JsonConvert.DeserializeObject<ConcurrentDictionary<string, CacheTypVal>>(jsonSerializedAppDict);
                                _lastCacheRW = DateTime.Now;
                            }
                        }
                    }
                }
                catch (Exception exGetRead)
                {
                    CqrException.SetLastException(exGetRead);
                    // Console.WriteLine($"Exception {exGetRead.GetType()}: {exGetRead.Message} \r\n\t{exGetRead}");
                }
                finally
                {
                    if (_appDict == null || _appDict.Count == 0)
                    {
                        _appDict = new ConcurrentDictionary<string, CacheTypVal>();
                        _lastCacheRW = DateTime.Now;
                    }
                }
                return _appDict;
            }

        }

        /// <summary>
        /// set where to set <see cref="ConcurrentDictionary{string, CacheTypVal}">it</see>  
        /// (value to _appDict to cache)
        /// </summary>
        /// <param name="cacheDict"><see cref="ConcurrentDictionary{string, CacheTypVal}"/></param>
        public override void SaveDictionaryToCache(ConcurrentDictionary<string, CacheTypVal> cacheDict)
        {
            lock (_outerlock)
            {
                string jsonDeserializedAppDict = "";
                var mutex = StaticCacheMutex.TheStaticCacheMutex;
                if (cacheDict == null) //  && value.Count > 0
                    return;

                try
                {
                    if (mutex != null && mutex.WaitOne(1024, false))
                    {
                        throw new SynchronizationLockException("Mutex " + mutex.ToString() + " blocks writing serialized json to " + JsonFullFilePath + ".");
                    }

                    StaticCacheMutex.CreateMutex("CacheWrite", false);

                    lock (_lock)
                    {
                        _appDict = cacheDict;

                        // set it, where to set it _appDict
                        jsonDeserializedAppDict = JsonConvert.SerializeObject(_appDict, Formatting.Indented, JsonSettings);
                        System.IO.File.WriteAllText(JsonFullFilePath, jsonDeserializedAppDict, Encoding.UTF8);
                        _lastCacheRW = DateTime.Now;
                    }

                }
                catch (Exception exSetWrite)
                {
                    CqrException.SetLastException(exSetWrite);
                    // Console.WriteLine($"Exception {exSetWrite.GetType()}: {exSetWrite.Message} \r\n\t{exSetWrite}");
                }
                finally
                {
                    StaticCacheMutex.ReleaseCloseDisposeMutex();
                }
            }
        }


        /// <summary>
        /// Gets a value from <see cref="ConcurrentDictionary{string, CacheTypVal}"/> stored <see cref="System.AppDomain.CurrentDomain"/>
        /// </summary>
        /// <typeparam name="T">generic type of cached value</typeparam>
        /// <param name="ckey">cache key</param>
        /// <returns>generic cached value stored at key</returns>
        public override T GetValue<T>(string ckey)
        {
            T tvalue = default;

            if (!string.IsNullOrEmpty(ckey))
            {
                lock (_lock)
                {

                    if (_appDict.ContainsKey(ckey) && _appDict.TryGetValue(ckey, out var cvalue))
                    {
                        if (cvalue != null)
                        {
                            tvalue = cvalue.GetValue<T>();
                        }
                    }
                }
            }

            return tvalue;
        }


        public override Nullable<T> GetNullableValue<T>(string ckey)
        {
            T tvalue = default(T);

            if (!string.IsNullOrEmpty(ckey))
            {
                lock (_lock)
                {

                    if (_appDict.ContainsKey(ckey) && _appDict.TryGetValue(ckey, out var cvalue))
                    {
                        if (cvalue != null)
                        {
                            var nilvalue = cvalue.GetNullableValue<T>();
                            if (nilvalue != null && nilvalue.HasValue)
                                tvalue = nilvalue.Value;
                        }
                    }
                }
            }

            return tvalue;
        }


        public JsonCache() : this(Persist.Json) { }

        public JsonCache(Persist cacheType)
        {
            _persist = (cacheType == Persist.Json) ? cacheType : Persist.Json;

            lock (_lock)
            {

                try
                {
                    if (!Directory.Exists(JsonFullDirPath))
                        Directory.CreateDirectory(JsonFullDirPath);
                }
                catch (Exception ioEx)
                {
                    CqrException.SetLastException(ioEx);
                }
                _appDict = LoadDictionaryCache(true);
            }

        }

    }


    /// <summary>
    /// AppCache an application cache implemented with a <see cref="ConcurrentDictionary{string, CacheTypVal}"/>
    /// </summary>
    public class AppCache : MemCache
    {

        /// <summary>
        /// public property get accessor for <see cref="_appCache"/> stored in <see cref="AppDomain.CurrentDomain"/>
        /// </summary>
        protected override ConcurrentDictionary<string, CacheTypVal> AppCache
        {
            get
            {
                _appCache = (ConcurrentDictionary<string, CacheTypVal>)AppDomain.CurrentDomain.GetData(APP_CONCURRENT_DICT);
                if (_appCache == null)
                {
                    lock (_lock)
                    {
                        _appCache = new ConcurrentDictionary<string, CacheTypVal>();
                        AppDomain.CurrentDomain.SetData(APP_CONCURRENT_DICT, _appCache);
                    }
                }

                return _appCache;
            }
            set
            {
                if (value != null && value.Count > 0)
                {
                    lock (_lock)
                    {
                        _appCache = value;
                        AppDomain.CurrentDomain.SetData(APP_CONCURRENT_DICT, _appCache);
                    }
                }
            }
        }

        public AppCache()
        {
            if (AppCache == null) ;
        }
    }


    /// <summary>
    /// RedisCache AWS elastic valkey cache singelton connector
    /// </summary>
    public class RedisCache : MemCache
    {

        #region const and static

        const string VALKEY_CACHE_HOST_PORT = "cqrcachecqrxseu-53g0xw.serverless.eus2.cache.amazonaws.com:6379";
        const string VALKEY_CACHE_APP_KEY = "RedisValkeyCache";
        const string REDIS_VALKEY_SSL = "RedisValkeySsl";
        const string ALL_KEYS = "AllKeys";

        protected internal static object _redIsLock = new object();
        public static new string CacheVariant = "RedisValkey";
        private static bool _ssl = true;
        private static string _endPoint = VALKEY_CACHE_HOST_PORT;
        public static string EndPoint
        {
            get
            {
                if (ConfigurationManager.AppSettings != null && ConfigurationManager.AppSettings[VALKEY_CACHE_APP_KEY] != null)
                    _endPoint = (string)ConfigurationManager.AppSettings[VALKEY_CACHE_APP_KEY];
                if (string.IsNullOrEmpty(_endPoint))
                    _endPoint = VALKEY_CACHE_HOST_PORT; // back to default
                return _endPoint;
            }
        }
        public static RedisCache ValKeyInstance => ((RedisCache)_instance.Value);

        public string Status { get => ConnMux.GetStatus(); }

        #endregion const and static


        ConfigurationOptions options;

        public override string[] AllKeys { get => GetAllKeys().ToArray(); }

        StackExchange.Redis.IDatabase _db;
        public StackExchange.Redis.IDatabase Db
        {
            get
            {
                if (_db == null)
                    _db = ConnMux.GetDatabase();

                return _db;
            }
        }

        ConnectionMultiplexer _connMux;
        public StackExchange.Redis.ConnectionMultiplexer ConnMux
        {
            get
            {
                if (_connMux == null)
                {
                    if (options == null)
                    {
                        if (!bool.TryParse((string)ConfigurationManager.AppSettings[REDIS_VALKEY_SSL], out _ssl))
                            _ssl = true;
                        options = new ConfigurationOptions
                        {
                            EndPoints = { EndPoint },
                            AbortOnConnectFail = false,
                            Ssl = _ssl
                        };
                    }
                    _connMux = ConnectionMultiplexer.Connect(options);
                }
                return _connMux;
            }
        }


        #region constructors

        public RedisCache() : this(Persist.Redis) { }

        /// <summary>
        /// default constructor for RedisCacheValKey cache singleton
        /// </summary>
        public RedisCache(Persist cacheType)
        {
            _persist = (cacheType == Persist.Redis) ? cacheType : Persist.Redis;
            _allKeys = new HashSet<string>();
            if (!bool.TryParse((string)ConfigurationManager.AppSettings[REDIS_VALKEY_SSL], out _ssl))
                _ssl = true;

            options = new ConfigurationOptions
            {
                EndPoints = { EndPoint },
                AbortOnConnectFail = false,
                Ssl = _ssl,
                ConnectTimeout = 6000,
                AsyncTimeout = 6000,
                SyncTimeout = 9000
            };

            _connMux = ConnectionMultiplexer.Connect(options);
            _db = _connMux.GetDatabase();
        }

        #endregion constructors

        #region GetString GetValue
        /// <summary>
        /// GetString gets a string value by RedisCache key
        /// </summary>
        /// <param name="redIsKey">key</param>
        /// <returns>(<see cref="string"/>) value for key redIsKey</returns>
        public override string GetString(string redIsKey)
        {
            return GetStringWithParams(redIsKey, CommandFlags.None);
        }

        public virtual string GetStringWithParams(string ckey, CommandFlags flags = CommandFlags.None)
        {
            return Db.StringGet(ckey, flags);
        }

        /// <summary>
        /// gets a generic class type T from redis cache with key
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ckey">rediskey</param>
        /// <returns>T value/returns>
        public override T GetValue<T>(string ckey)
        {
            string jsonVal = GetStringWithParams(ckey, CommandFlags.None); // Db.StringGet(ckey, flags);

            T tval = default(T);
            if (jsonVal != null)
            {
                tval = JsonConvert.DeserializeObject<T>(jsonVal);
            }

            return tval;
        }

        #endregion GetString GetValue

        #region SetString SetValue
        /// <summary>
        /// SetString set key with string value
        /// </summary>
        /// <param name="ckey">key for string/param>
        /// <param name="svalue">value to set</param>
        public override bool SetString(string ckey, string svalue)
        {
            return SetStringWithParams(ckey, svalue);
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
        public virtual bool SetStringWithParams(string redIsKey, string redIsString, TimeSpan? expiry = null, bool keepTtl = false, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            bool success = false;
            lock (_redIsLock)
            {
                _allKeys = GetAllKeys();
                success = Db.StringSet(redIsKey, redIsString, expiry, when, flags);

                if (success && !_allKeys.Contains(redIsKey))
                {
                    _allKeys.Add(redIsKey);
                    string jsonVal = JsonConvert.SerializeObject(_allKeys);
                    success = Db.StringSet(ALL_KEYS, jsonVal, null, keepTtl, When.Always, CommandFlags.None);
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
            TimeSpan? expiry = new TimeSpan(1, 1, 1, 1);
            bool keepTtl = false;
            When when = When.Always;
            CommandFlags flags = CommandFlags.None;
            string jsonVal = JsonConvert.SerializeObject(tvalue);
            bool success = SetStringWithParams(ckey, jsonVal, expiry, keepTtl, when, flags);

            return success;
        }

        #endregion SetString SetValue

        #region RemoveKey ContainsKey

        /// <summary>
        /// DeleteKey delete entry referenced at key
        /// </summary>
        /// <param name="redIsKey">key</param>
        /// <param name="flags"><see cref="CommandFlags.FireAndForget"/> as default</param>
        public override bool RemoveKey(string redIsKey)
        {
            CommandFlags flags = CommandFlags.FireAndForget;
            lock (_redIsLock)
            {
                if (ContainsKey(redIsKey) || _allKeys.Contains(redIsKey))
                {
                    _allKeys.Remove(redIsKey);
                    string jsonVal = JsonConvert.SerializeObject(_allKeys.ToArray());
                    Db.StringSet("AllKeys", jsonVal, null, false, When.Always, flags);
                }
                try
                {
                    TimeSpan span = new TimeSpan(0, 0, 1);
                    Db.StringGetDelete(redIsKey, flags);
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"Exception {ex.GetType()}: {ex.Message}\r\n\t{ex}");
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
            _allKeys = GetAllKeys();
            if (_allKeys.Contains(ckey))
            {
                string redIsString = Db.StringGet(ckey, CommandFlags.None);
                if (redIsString != null)
                    return true;
            }
            return false;
        }

        #endregion RemoveKey ContainsKey

        /// <summary>
        /// GetAllKeys returns <see cref="HashSet{string}"/></string> <see cref="_allKeys"/>
        /// </summary>
        /// <returns>returns <see cref="HashSet{string}"/></string> <see cref="_allKeys"/></returns>
        public override HashSet<string> GetAllKeys()
        {
            if (_allKeys == null || _allKeys.Count == 0)
            {
                string jsonVal = Db.StringGet(ALL_KEYS, CommandFlags.None);
                string[] keys = (jsonVal != null) ? JsonConvert.DeserializeObject<string[]>(jsonVal) : new string[0];
                if (keys != null && keys.Length > 0)
                    _allKeys = new HashSet<string>(keys);
            }

            return _allKeys;
        }

    }

    /// <summary>
    /// static <see cref="Mutex"/> for mutal exclusion, you can only use it once for one mutal exclusion caae, cause it's static 
    /// get <see cref="Mutex"/> by calling <see cref="CreateMutex(string, bool)"/> 
    /// release <see cref="Mutex"/> by calling <see cref="ReleaseCloseDisposeMutex"/>
    /// </summary>
    internal static class StaticCacheMutex
    {
        private static readonly object _outerLock = new object(), _lock = new object();

        private static Mutex _theStaticCacheMutex = null;

        /// <summary>
        /// Gets the Mutal Exclusion
        /// </summary>
        internal static Mutex TheStaticCacheMutex { get => _theStaticCacheMutex; }

        /// <summary>
        /// static ctor
        /// </summary>
        static StaticCacheMutex()
        {
            _theStaticCacheMutex = null;
        }

        /// <summary>
        /// Gets existing mutex or creates a new <see cref="Mutex"/> 
        /// </summary>
        /// <param name="mutexUniqueName">unique string identifier for the mutal exlusion</param>
        /// <param name="useExistingMutex">if true, existing and valid <see cref="Mutex"/> will be returned, 
        /// otherwise a new <see cref="Mutex"/> will be created; default <see cref="false"/></param>
        /// <returns><see cref="Mutex"/></returns>
        internal static Mutex CreateMutex(string mutexUniqueName = "StaticCacheMutex", bool useExistingMutex = false)
        {
            if (useExistingMutex && _theStaticCacheMutex != null && _theStaticCacheMutex.SafeWaitHandle != null &&
                !_theStaticCacheMutex.SafeWaitHandle.IsClosed && !_theStaticCacheMutex.SafeWaitHandle.IsInvalid)
                return _theStaticCacheMutex;

            Thread.Sleep(16);
            _theStaticCacheMutex = new Mutex(true, mutexUniqueName);

            return _theStaticCacheMutex;
        }

        /// <summary>
        /// Release Mutax exclusion, that not 2 chat programs could be started at same machine
        /// </summary>
        internal static void ReleaseCloseDisposeMutex()
        {
            Exception ex = null;
            Microsoft.Win32.SafeHandles.SafeWaitHandle safeWaitHandle = null;
            IntPtr safeMutextWin32Handle = IntPtr.Zero;

            lock (_outerLock)
            {
                if (_theStaticCacheMutex != null)
                {
                    lock (_lock)
                    {
                        safeWaitHandle = _theStaticCacheMutex.GetSafeWaitHandle();
                        safeMutextWin32Handle = safeWaitHandle.DangerousGetHandle();
                        if (safeWaitHandle != null && !safeWaitHandle.IsClosed)
                        {
                            try
                            {
                                _theStaticCacheMutex.ReleaseMutex();
                                //    safeWaitHandle.DangerousRelease();
                            }
                            catch (Exception exRelease)
                            {
                                ex = new CqrException("Releasing Mutex failed", exRelease);
                                CqrException.SetLastException(ex);
                            }
                            try
                            {
                                _theStaticCacheMutex.Close();
                                //    safeWaitHandle.Close();
                            }
                            catch (Exception exClose)
                            {
                                ex = new CqrException("Closing Mutex failed", exClose);
                                CqrException.SetLastException(ex);
                            }
                        }

                        try
                        {
                            _theStaticCacheMutex.Dispose();
                            //    safeWaitHandle.Dispose();
                        }
                        catch (Exception exDispose)
                        {
                            ex = new CqrException("Disposing Mutex failed", exDispose);
                            CqrException.SetLastException(ex);
                        }
                    }
                }

                try
                {
                    _theStaticCacheMutex = null;
                }
                catch (Exception exNull)
                {
                    ex = new CqrException("Setting Mutex to null failed", exNull);
                    CqrException.SetLastException(ex);
                }
                finally
                {
                    if (ex != null)
                    {
                        CqrException.SetLastException(new CqrException("Disposing mutex and safeWaitHandle throwed exception.", ex));
                    }
                }
            }

            return;
        }

    }

    [Serializable]
    public class CacheData
    {
        static readonly byte[] buffer = new byte[4096];
        static Random random = new Random((DateTime.Now.Millisecond + 1) * (DateTime.Now.Second + 1));
        [JsonIgnore]
        protected internal int CIndex { get; set; }
        public string CKey { get; set; }
        public string CValue { get; set; }
        public int CThreadId { get; set; }
        public DateTime CTime { get; set; }

        static CacheData()
        {
            random.NextBytes(buffer);
        }

        public CacheData()
        {
            CIndex = 0;
            CValue = string.Empty;
            CKey = string.Empty;
            CTime = DateTime.MinValue;
            CThreadId = -1;
        }

        public CacheData(string ckey) : this()
        {
            CKey = ckey;
            CIndex = Int32.Parse(ckey.Replace("Key_", ""));
            CValue = GetRandomString(CIndex);
            CTime = DateTime.Now;
        }

        public CacheData(string ckey, int cThreadId) : this(ckey)
        {
            CThreadId = cThreadId;
        }

        public CacheData(int cThreadId) : this(string.Concat("Key_", cThreadId))
        {
            CThreadId = cThreadId;
        }


        internal string GetRandomString(int ix)
        {
            byte[] restBytes = new byte[64];
            Array.Copy(buffer, ix, restBytes, 0, 64);
            return Convert.ToBase64String(restBytes, 0, 64);
        }

    }

    internal class Program
    {
        static void Main(string[] args)
        {
            RunTasks(256);
            RunSerial(256);

            Console.WriteLine($"\nPress any key to continue...\n");
            Console.ReadKey();
        }

        static void RunTasks(int numberOfTasks, short maxKexs = 16)
        {
            string parallelCache = MemCache.CacheVariant;
            Console.WriteLine($"RunTasks(int numberOfTasks = {numberOfTasks}) cache = {parallelCache}.");
            DateTime now = DateTime.Now;
            if (numberOfTasks <= 0)
                numberOfTasks = 16;
            if ((numberOfTasks % 4) != 0)
                numberOfTasks += (4 - (numberOfTasks % 4));

            int quater = numberOfTasks / 4;
            int half = numberOfTasks / 2;
            int threequater = quater + half;

            Task[] taskArray = new Task[numberOfTasks];
            for (int i = 0; i < numberOfTasks; i++)
            {
                if (i < quater || (i >= half && i < threequater))
                {
                    taskArray[i] = Task.Factory.StartNew((object obj) =>
                    {
                        string ckey = string.Concat("Key_", (i % maxKexs).ToString());
                        CacheData data = obj as CacheData;
                        if (data == null)
                            data = new CacheData(ckey, Thread.CurrentThread.ManagedThreadId);

                        data.CThreadId = Thread.CurrentThread.ManagedThreadId;
                        MemCache.CacheDict.SetValue<CacheData>(ckey, data);
                        // Console.WriteLine($"Task set cache key #{data.CKey} created at {data.CTime} on thread #{data.CThreadId}.");
                    },
                    new CacheData("Key_" + (i % maxKexs).ToString()));
                }
                else if ((i >= quater && i < half) || i >= threequater)
                {
                    taskArray[i] = Task.Factory.StartNew((object obj) =>
                    {
                        string ckey = string.Concat("Key_", (i % maxKexs).ToString());
                        string strkey = obj as string;
                        if (string.IsNullOrEmpty(strkey))
                            strkey = ckey;

                        CacheData data = (CacheData)MemCache.CacheDict[strkey];
                        // Console.WriteLine($"Task get cache key #{strkey} => {data.CValue} created at {data.CTime} original thread {data.CThreadId} on current thread #{Thread.CurrentThread.ManagedThreadId}.");
                    },
                    new StringBuilder(string.Concat("Key_", (i % maxKexs).ToString())).ToString());
                }
            }

            Task.WaitAll(taskArray);

            TimeSpan ts = DateTime.Now.Subtract(now);
            double doublePerSecond = numberOfTasks / ts.TotalSeconds;
            if (numberOfTasks > ts.TotalSeconds)
                doublePerSecond = (1000000 * numberOfTasks) / ts.TotalMicroseconds;
            ulong perSecond = (ulong)doublePerSecond;
            Console.WriteLine($"Finished {numberOfTasks} parallel tasks in {ts.Minutes:d2}:{ts.Seconds:d2}.{ts.Milliseconds:d3}.{ts.Microseconds:d3}\n\t{perSecond} tasks per second.\n");
        }

        static void RunSerial(int iterationsCount, short maxKexs = 16)
        {
            string serialSache = MemCache.CacheVariant;
            Console.WriteLine($"RunSerial(int iterationsCount = {iterationsCount}) cache = {serialSache}.");

            if (iterationsCount <= 0)
                iterationsCount = 16;
            if ((iterationsCount % 4) != 0)
                iterationsCount += (4 - (iterationsCount % 4));
            int quater = iterationsCount / 4;
            int half = iterationsCount / 2;
            int threequater = quater + half;

            DateTime now = DateTime.Now;
            for (int i = 0; i < iterationsCount; i++)
            {
                if (i < quater || (i >= half && i < threequater))
                {
                    string ckey = string.Concat("Key_", (i % maxKexs).ToString());
                    CacheData data = new CacheData(ckey, Thread.CurrentThread.ManagedThreadId);
                    MemCache.CacheDict.SetValue<CacheData>(ckey, data);
                    // Console.WriteLine($"Task set cache key #{data.CKey} created at {data.CTime} on thread #{data.CThreadId}.");
                }
                else if ((i >= quater && i < half) || i >= threequater)
                {
                    string strkey = "Key_" + (i % maxKexs).ToString();
                    CacheData cacheData = (CacheData)MemCache.CacheDict[strkey];
                    // Console.WriteLine($"Task get cache key #{strkey} => {cacheData.CValue} created at {cacheData.CTime} original thread {cacheData.CThreadId} on current thread #{Thread.CurrentThread.ManagedThreadId}.");
                }
            }

            // var tasks = new List<Task>(taskArray);            
            // Parallel.ForEach(tasks, task => { task.Start(); });
            //Task.WhenAll(tasks).ContinueWith(done => { Console.WriteLine("done"); });

            TimeSpan ts = DateTime.Now.Subtract(now);
            double doublePerSecond = iterationsCount / ts.TotalSeconds;
            if (iterationsCount > ts.TotalSeconds)
                doublePerSecond = (1000000 * iterationsCount) / ts.TotalMicroseconds;
            ulong perSecond = (ulong)doublePerSecond;
            Console.WriteLine($"Finished {iterationsCount} iterations in {ts.Minutes:d2}:{ts.Seconds:d2}.{ts.Milliseconds:d3}.{ts.Microseconds:d3}\n\t{perSecond} iterations per second.\n");

        }

    }

}
