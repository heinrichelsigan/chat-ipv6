using Area23.At.Framework.Core;
using Area23.At.Framework.Core.Static;
using EU.CqrXs.Srv.Svc.Swashbuckle.Util;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace EU.CqrXs.Srv.Svc.Swashbuckle.Util
{
    /// <summary>
    /// JsonHelper class for reading and writing json serialized store file.
    /// </summary>
    public static class JsonHelper
    {
        internal static string JsonFileName { get => Area23.At.Framework.Core.Static.JsonHelper.JsonFileName; }


        public static string JsonContactsFile
        {
            get
            {
                string loadFileName = System.IO.Path.Combine(LibPaths.SystemDirJsonPath, Constants.JSON_CONTACTS_FILE);
                if (!File.Exists(loadFileName))
                {
                    loadFileName = AppContext.BaseDirectory.
                        Replace(LibPaths.SepChar + Constants.BIN_DIR, "").Replace(LibPaths.SepChar + Constants.OBJ_DIR, "").
                        Replace(LibPaths.SepChar + Constants.RELEASE_DIR, "").Replace(LibPaths.SepChar + Constants.DEBUG_DIR, "");
                    loadFileName += (loadFileName.EndsWith(LibPaths.SepChar)) ? "" : LibPaths.SepChar;
                    loadFileName += Constants.RES_DIR + LibPaths.SepChar + Constants.JSON_CONTACTS_FILE;
                }
                if (!File.Exists(loadFileName))
                {
                    loadFileName = AppDomain.CurrentDomain.BaseDirectory.
                        Replace(LibPaths.SepChar + Constants.BIN_DIR, "").Replace(LibPaths.SepChar + Constants.OBJ_DIR, "").
                        Replace(LibPaths.SepChar + Constants.RELEASE_DIR, "").Replace(LibPaths.SepChar + Constants.DEBUG_DIR, "");
                    loadFileName += (loadFileName.EndsWith(LibPaths.SepChar)) ? "" : LibPaths.SepChar;
                    loadFileName += Constants.RES_DIR + LibPaths.SepChar + Constants.JSON_CONTACTS_FILE;
                }
                return loadFileName;
            }
        }

        internal static Dictionary<string, Uri> ShortenMapJson
        { 
            get => Area23.At.Framework.Core.Static.JsonHelper.ShortenMapJson;
            set => Area23.At.Framework.Core.Static.JsonHelper.ShortenMapJson = value;
        }


        internal static void SaveDictionaryToJson(Dictionary<string, Uri> saveDict) => Area23.At.Framework.Core.Static.JsonHelper.SaveDictionaryToJson(saveDict);
        
    }

}