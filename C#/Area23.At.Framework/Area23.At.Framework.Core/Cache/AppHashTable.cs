using Area23.At.Framework.Core.Static;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Area23.At.Framework.Core.Cache
{
    public class AppTypeValue 
    {
        public Type? AppTyp { get; set; }
        public object? AppVal { get; set; }


        public AppTypeValue()
        {
            AppTyp = null;
            AppVal = null;
        }

        public AppTypeValue(object val, Type typ)
        {
            AppTyp = typ;
            AppVal = val;
        }

        public T? GetValue<T>()
        {
            if (typeof(T) == AppTyp)
                return (T?)AppVal;
            throw new InvalidOperationException($"typeof(T) = {typeof(T)} while AppTyp = {AppTyp}");
        }


        public void SetValue<T>(T val)        
        {
            AppTyp = typeof(T);
            AppVal = (object)val;
        }
    }


    public static class AppHashTable
    {
        private static ConcurrentDictionary<string, AppTypeValue> _appDict = new ConcurrentDictionary<string, AppTypeValue>();

        public static ConcurrentDictionary<string, AppTypeValue> AppDict => _appDict;

        public static T? GetValue<T>(string key)
        {
            if (_appDict == null || _appDict.Count == 0)
                _appDict = (ConcurrentDictionary<string, AppTypeValue>)AppDomain.CurrentDomain.GetData(Constants.APP_CONCURRENT_DICT);

            if (_appDict.TryGetValue(key, out var val))
                return val.GetValue<T>();
            return default(T);
        }

        public static void SetValue<T>(string key, T val)
        {
            AppTypeValue appTypeValue = new AppTypeValue(val, typeof(T));
            appTypeValue.SetValue<T>(val);
            if (_appDict.ContainsKey(key))
                _appDict.TryRemove(key, out AppTypeValue value);
            
            if (_appDict.TryAdd(key, appTypeValue))
            {
                AppDomain.CurrentDomain.SetData(Constants.APP_CONCURRENT_DICT, _appDict);
            }
                       
        }

        
    }
}
