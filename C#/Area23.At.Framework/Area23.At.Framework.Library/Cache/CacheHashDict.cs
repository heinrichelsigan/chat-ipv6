using Area23.At.Framework.Library.Static;
using Area23.At.Framework.Library.Util;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Area23.At.Framework.Library.Cache
{

    /// <summary>
    /// CacheHashDict an application cache implemented with a <see cref="ConcurrentDictionary{string, CacheValue}"/> serialized with json    
    /// </summary>
    public class CacheHashDict : MemCacheDict
    {

        const int INIT_SEM_COUNT = 1;
        const int MAX_SEM_COUNT = 1;

        internal static SemaphoreSlim ReadWriteSemaphore = new SemaphoreSlim(INIT_SEM_COUNT, MAX_SEM_COUNT);

        protected static string JsonFile { get => System.IO.Path.Combine(LibPaths.SystemDirJsonPath, Constants.JSON_APPDICT_FILE); }

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
        protected static new ConcurrentDictionary<string, CacheValue> AppDict
        {
            get
            {
                int semCnt = 0;
                try
                {
                    semCnt = ReadWriteSemaphore.CurrentCount;
                    ReadWriteSemaphore.Wait(128);

                    string jsonSerializedAppDict = (System.IO.File.Exists(JsonFile)) ? System.IO.File.ReadAllText(JsonFile) : "";
                    if (!string.IsNullOrEmpty(jsonSerializedAppDict))
                        _appDict = (ConcurrentDictionary<string, CacheValue>)JsonConvert.DeserializeObject<ConcurrentDictionary<string, CacheValue>>(jsonSerializedAppDict);

                    if (_appDict == null || _appDict.Count == 0)
                    {
                        _appDict = new ConcurrentDictionary<string, CacheValue>();
                        // where to set it _appDict
                        string jsonDeserializedAppDict = JsonConvert.SerializeObject(_appDict, Formatting.Indented, JsonSettings);
                        System.IO.File.WriteAllText(JsonFile, jsonDeserializedAppDict, Encoding.UTF8);  // set it where to set it _appDict
                    }
                }
                catch (Exception exGetRead)
                {
                    Area23Log.LogStatic(exGetRead);
                }
                finally
                {
                    semCnt = ReadWriteSemaphore.Release();
                }
                return _appDict;
            }
            set
            {
                int semCnt = 0;
                try
                {
                    semCnt = ReadWriteSemaphore.CurrentCount;
                    ReadWriteSemaphore.Wait();

                    string jsonDeserializedAppDict = "";
                    if (value != null && value.Count > 0)
                    {
                        _appDict = value;

                        // set it where to set it _appDict
                        jsonDeserializedAppDict = JsonConvert.SerializeObject(_appDict, Formatting.Indented, JsonSettings);
                        System.IO.File.WriteAllText(JsonFile, jsonDeserializedAppDict, Encoding.UTF8);  // set it where to set it _appDict
                    }
                }
                catch (Exception exSetWrite)
                {
                    Area23Log.LogStatic(exSetWrite);
                }
                finally
                {
                    semCnt = ReadWriteSemaphore.Release();
                }
            }
        }

    }

}
