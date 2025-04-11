using Area23.At.Framework.Library.Cache;
using Area23.At.Framework.Library.Static;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Web;


/// <summary>
/// Redis AWS elastic valkey cache singelton connector
/// </summary>
public class RedIs
{
    private static readonly Lazy<RedIs> _instance = new Lazy<RedIs>(() => new RedIs());
    
    public static RedIs ValKey { get { return _instance.Value; } }

    private static HashSet<string> _allKeys = new HashSet<string>();
    public static string[] AllKeys { get { return _allKeys.ToArray(); } }   

    public static StackExchange.Redis.IDatabase Db
    {
        get
        {
            return RedIstatic.Db;
        }
    }

    public static StackExchange.Redis.ConnectionMultiplexer ConnMux
    {
        get
        {
            return RedIstatic.ConnMux;
        }
    }


    /// <summary>
    /// default parameterless constructor for RedIsValKey cache singleton 
    /// </summary>
    public RedIs()
    {
       
    }



    /// <summary>
    /// GetString gets a string value by redis key
    /// </summary>
    /// <param name="redIsKey">key</param>
    /// <param name="flags"><see cref="CommandFlags"/></param>
    /// <returns>(<see cref="string"/>) value for key redIsKey</returns>
    public string GetString(string redIsKey, CommandFlags flags = CommandFlags.None)
    {
        string redIsString = RedIstatic.GetString(redIsKey, flags);
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
        RedIstatic.SetString(redIsKey, redIsString, expiry, keepTtl, when, flags);
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
        RedIstatic.SetKey<T>(redIsKey, tValue, expiry, keepTtl, when, flags);         
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
        var tValue = RedIstatic.GetKey<T>(redIsKey, flags);        
        return tValue;
    }

    /// <summary>
    /// DeleteKey delete entry referenced at key
    /// </summary>
    /// <param name="redIsKey">key</param>
    /// <param name="flags"><see cref="CommandFlags.FireAndForget"/> as default</param>
    public void DeleteKey(string redIsKey, CommandFlags flags = CommandFlags.FireAndForget)
    {
        RedIstatic.DeleteKey(redIsKey, flags);
    }


}
