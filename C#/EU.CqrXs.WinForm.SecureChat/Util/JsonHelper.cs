using Area23.At.Framework.Core.Static;

namespace EU.CqrXs.WinForm.SecureChat.Util
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
                string loadFileName = System.IO.Path.Combine(LibPaths.SystemDirSecureChatFilesPath, Constants.JSON_CONTACTS_FILE);
                if (!File.Exists(loadFileName))
                {
                    loadFileName = AppContext.BaseDirectory.
                        Replace(LibPaths.SepChar + Constants.BIN_DIR, "").Replace(LibPaths.SepChar + Constants.OBJ_DIR, "").
                        Replace(LibPaths.SepChar + Constants.RELEASE_DIR, "").Replace(LibPaths.SepChar + Constants.DEBUG_DIR, "");
                    loadFileName += (loadFileName.EndsWith(LibPaths.SepChar)) ? "" : LibPaths.SepChar;
                    loadFileName += Constants.JSON_CONTACTS_FILE;
                }
                if (!File.Exists(loadFileName))
                {
                    loadFileName = AppDomain.CurrentDomain.BaseDirectory.
                        Replace(LibPaths.SepChar + Constants.BIN_DIR, "").Replace(LibPaths.SepChar + Constants.OBJ_DIR, "").
                        Replace(LibPaths.SepChar + Constants.RELEASE_DIR, "").Replace(LibPaths.SepChar + Constants.DEBUG_DIR, "");
                    loadFileName += (loadFileName.EndsWith(LibPaths.SepChar)) ? "" : LibPaths.SepChar;
                    loadFileName += Constants.JSON_CONTACTS_FILE;
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