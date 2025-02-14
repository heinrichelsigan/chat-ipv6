using Area23.At.Framework.Library;
using EU.CqrXs.CqrSrv.CqrJd.Util;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace EU.CqrXs.CqrSrv.CqrJd.Util
{
    /// <summary>
    /// JsonHelper class for reading and writing json serialized store file.
    /// </summary>
    public static class JsonHelper
    {
        internal static string JsonFileName { get => Area23.At.Framework.Library.Util.JsonHelper.JsonFileName; }

        internal static Dictionary<string, Uri> ShortenMapJson
        { 
            get => Area23.At.Framework.Library.Util.JsonHelper.ShortenMapJson;
            set => Area23.At.Framework.Library.Util.JsonHelper.ShortenMapJson = value;
        }


        internal static void SaveDictionaryToJson(Dictionary<string, Uri> saveDict)
        {
            Area23.At.Framework.Library.Util.JsonHelper.SaveDictionaryToJson(saveDict);
            return;
        }
        
    }

}