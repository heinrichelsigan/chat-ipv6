using Area23.At.Framework.Library;
using Area23.At.Framework.Library.Static;
using EU.CqrXs.Srv.Util;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace EU.CqrXs.Srv.Util
{
    /// <summary>
    /// JsonHelper class for reading and writing json serialized store file.
    /// </summary>
    public static class JsonHelper
    {
        internal static string JsonFileName { get => Area23.At.Framework.Library.Static.JsonHelper.JsonFileName; }


        public static string JsonContactsFile { get => Area23.At.Framework.Library.Static.JsonHelper.JsonContactsFile; }


        internal static Dictionary<string, Uri> ShortenMapJson
        { 
            get => Area23.At.Framework.Library.Static.JsonHelper.ShortenMapJson;
            set => Area23.At.Framework.Library.Static.JsonHelper.ShortenMapJson = value;
        }


        internal static void SaveDictionaryToJson(Dictionary<string, Uri> saveDict) => Area23.At.Framework.Library.Static.JsonHelper.SaveDictionaryToJson(saveDict);
        
    }

}