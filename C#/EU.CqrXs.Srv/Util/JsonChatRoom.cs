using Area23.At.Framework.Library;
using Area23.At.Framework.Library.Cache;
using Area23.At.Framework.Library.Cqr.Msg;
using Area23.At.Framework.Library.Cqr;
using Area23.At.Framework.Library.Static;
using Area23.At.Framework.Library.Util;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Web;

namespace EU.CqrXs.Srv.Util
{
    /// <summary>
    /// JsonChatRoom 
    /// </summary>
    public class JsonChatRoom
    {

        #region fields
        static object _lock = new object();
        static HashSet<string> _chatRooms;
        private string _jsonChatRoomNumber;
        #endregion fields
        #region properties
        public string JsonChatRoomNumber { get { return _jsonChatRoomNumber; } set { _jsonChatRoomNumber = value; } }

        internal string JsonChatRoomFileName { get { return LibPaths.SystemDirJsonPath + JsonChatRoomNumber; } }
        #endregion properties

        #region ctors
        static JsonChatRoom()
        {
            List<string> jsonChatRooms = new List<string>();
            try
            {
                jsonChatRooms = ChatRoomNumbersFromFs();
            } 
            catch (Exception exCtor)
            {
                Area23Log.LogStatic("static JsonChatRoom()", exCtor, "");
            }
            _chatRooms = new HashSet<string>(jsonChatRooms);
        }

        public JsonChatRoom()
        {
            _jsonChatRoomNumber = System.DateTime.Now.ToString();
            if (_chatRooms == null)
                _chatRooms = new HashSet<string>(ChatRoomNumbersFromFs());
        }

        public JsonChatRoom(string jsonChatRoomNumber) : this()
        {
            if (string.IsNullOrEmpty(jsonChatRoomNumber))
                jsonChatRoomNumber = "room_unknown_" + System.DateTime.Now.Area23DateTimeWithMillis() + ".json";

            JsonChatRoomNumber = (jsonChatRoomNumber.EndsWith(".json", StringComparison.CurrentCultureIgnoreCase)) ? jsonChatRoomNumber : jsonChatRoomNumber + ".json";
        }
        #endregion ctors

        #region instance members
        public CSrvMsg<string> LoadJsonChatRoom(CSrvMsg<string> cSrvMsgIn, string chatRoomNr)
        {
            JsonChatRoomNumber = chatRoomNr;
            CSrvMsg<string> cServerMessage = null;
            string jsonText = null;
            if (!System.IO.File.Exists(JsonChatRoomFileName)) // we need to create chatroom
            {
                SaveJsonChatRoom(cSrvMsgIn, cSrvMsgIn.CRoom);
            }

            lock (_lock)
            {
                jsonText = System.IO.File.ReadAllText(JsonChatRoomFileName);
                cServerMessage = JsonConvert.DeserializeObject<CSrvMsg<string>>(jsonText);
            }

            return SerializeCSrvMsg(cServerMessage, out string serJsonString);
        }

        public CSrvMsg<string> SaveJsonChatRoom(CSrvMsg<string> cSrvMsg, CChatRoom chatRoom)
        {
            string jsonString = "";
            string chatRoomNumber = chatRoom.ChatRoomNr;
            lock (_lock)
            {
                if (!chatRoomNumber.Equals(this.JsonChatRoomNumber))
                    JsonChatRoomNumber = chatRoomNumber;

                if (!JsonChatRoomNumber.EndsWith(".json", StringComparison.InvariantCultureIgnoreCase))
                    JsonChatRoomNumber += ".json";

                cSrvMsg.CRoom = new CChatRoom(JsonChatRoomNumber, chatRoom.ChatRuid, chatRoom.LastPushed, chatRoom.LastPolled)
                {
                    TicksLong = chatRoom.TicksLong,
                    _message = JsonChatRoomNumber,
                    _hash = chatRoom.Hash,
                    Md5Hash = chatRoom.Md5Hash
                };
                cSrvMsg.Sender._message = (cSrvMsg.CRoom != null && !string.IsNullOrEmpty(cSrvMsg.CRoom.ChatRoomNr)) ? cSrvMsg.CRoom.ChatRoomNr : cSrvMsg.Sender._message;
                // cSrvMsg.SerializedMsg = "";

                SerializeCSrvMsg(cSrvMsg, out jsonString);

                //JsonSerializerSettings jsets = new JsonSerializerSettings();
                //jsets.Formatting = Formatting.Indented;
                //jsets.MaxDepth = 16;
                //jsonString = JsonConvert.SerializeObject(cSrvMsg, Formatting.Indented);
                System.IO.File.WriteAllText(JsonChatRoomFileName, jsonString);
            }

            cSrvMsg.SerializedMsg = jsonString;

            return cSrvMsg;
        }

        public bool DeleteJsonChatRoom(string chatRoomNr)
        {
            JsonChatRoomNumber = chatRoomNr;

            if (MemoryCache.CacheDict.ContainsKey(chatRoomNr))
                MemoryCache.CacheDict.RemoveKey(chatRoomNr);

            DeleteJsonChatRoomFromCache(chatRoomNr);

            lock (_lock)
            {
                if (System.IO.File.Exists(JsonChatRoomFileName)) // we need to create chatroom
                {
                    try
                    {
                        System.IO.File.Delete(JsonChatRoomFileName);
                    }
                    catch (Exception exDelJson)
                    {
                        Area23Log.LogStatic($"DeleteJsonChatRoom(string chatRoomNr = {chatRoomNr}): Error deleting chat room ", exDelJson, "");
                        return false;
                    }
                }
            }

            return true;
        }
        #endregion instance members

        #region static members

        public static CSrvMsg<string> SerializeCSrvMsg(CSrvMsg<string> cSrvMsg, out string serializedJsonString)
        {
            serializedJsonString = string.Empty;
            if (cSrvMsg != null)
            {
                cSrvMsg.SerializedMsg = string.Empty;
                JsonSerializerSettings jsets = new JsonSerializerSettings();
                jsets.Formatting = Formatting.Indented;
                jsets.MaxDepth = 16;
                serializedJsonString = JsonConvert.SerializeObject(cSrvMsg, Formatting.Indented);
            }

            cSrvMsg.SerializedMsg = serializedJsonString;
            return cSrvMsg;
        }

        public static string GetJsonChatRoomFileName(string jsonChatRoomNr)
        {
            return LibPaths.SystemDirJsonPath + jsonChatRoomNr;
        }

        /// <summary>
        /// static SaveChatRoom
        /// </summary>
        /// <param name="cSrvMsg"><see cref="CSrvMsg<string>"/></param>
        /// <param name="chatRoom"><see cref="CChatRoom"/></param>
        /// <returns></returns>
        public static CSrvMsg<string> SaveChatRoom(CSrvMsg<string> cSrvMsg, CChatRoom chatRoom)
        {
            string jsonString = "";
            string chatRoomNumber = chatRoom.ChatRoomNr;
            lock (_lock)
            {
                if (!chatRoomNumber.EndsWith(".json"))
                    chatRoomNumber += ".json";

                cSrvMsg.CRoom = new CChatRoom(chatRoomNumber, chatRoom.ChatRuid, chatRoom.LastPushed, chatRoom.LastPolled)
                {
                    TicksLong = chatRoom.TicksLong,
                    _message = chatRoomNumber,
                    _hash = chatRoom.Hash,
                    Md5Hash = chatRoom.Md5Hash
                };
                if (cSrvMsg.CRoom == null && !string.IsNullOrEmpty(cSrvMsg.CRoom.ChatRoomNr))
                    cSrvMsg.Sender._message = cSrvMsg.CRoom.ChatRoomNr;
                // cSrvMsg.SerializedMsg = "";

                string jsonCRoomFileName = GetJsonChatRoomFileName(chatRoomNumber);
                SerializeCSrvMsg(cSrvMsg, out jsonString);
                //JsonSerializerSettings jsets = new JsonSerializerSettings();
                //jsets.Formatting = Formatting.Indented;
                //jsets.MaxDepth = 16;
                //jsonString = JsonConvert.SerializeObject(cSrvMsg, Formatting.Indented);
                System.IO.File.WriteAllText(jsonCRoomFileName, jsonString);
            }

            cSrvMsg.SerializedMsg = jsonString;

            return cSrvMsg;
        }


        /// <summary>
        /// Static LoadChatRoom
        /// </summary>
        /// <param name="cSrvMsgIn"><see cref="CSrvMsg{TC}"/></param>
        /// <param name="chatRoomNr">chatRoomNr</param>
        /// <returns>><see cref="CSrvMsg{TC}"/></returns>
        public static CSrvMsg<string> LoadChatRoom(CSrvMsg<string> cSrvMsgIn, string chatRoomNr)
        {

            string jsonCRoomFileName = GetJsonChatRoomFileName(chatRoomNr);

            CSrvMsg<string> cServerMessage = null;
            string jsonText = null;
            if (!System.IO.File.Exists(jsonCRoomFileName)) // we need to a create chatroom
            {
                CChatRoom chatRoom = cSrvMsgIn.CRoom ?? new CChatRoom(chatRoomNr, Guid.NewGuid(), DateTime.MinValue, DateTime.MinValue);
                chatRoom.ChatRoomNr = chatRoomNr;
                SaveChatRoom(cSrvMsgIn, chatRoom);
            }

            lock (_lock)
            {
                jsonText = System.IO.File.ReadAllText(jsonCRoomFileName);
                cServerMessage = JsonConvert.DeserializeObject<CSrvMsg<string>>(jsonText);
            }

            return SerializeCSrvMsg(cServerMessage, out string serJsonString);
        }

        public static bool DeleteChatRoom(string chatRoomNr)
        {
            string jsonChatRoomFileName = GetJsonChatRoomFileName(chatRoomNr);

            
            DeleteJsonChatRoomFromCache(chatRoomNr);

            lock (_lock)
            {
                if (System.IO.File.Exists(jsonChatRoomFileName)) // we need to create chatroom
                {
                    try
                    {
                        System.IO.File.Delete(jsonChatRoomFileName);
                    }
                    catch (Exception exDelChatRoomFromFs)
                    {
                        Area23Log.LogStatic($"DeleteChatRoom(string chatRoomNr = {chatRoomNr}): Error deleting chat room ", exDelChatRoomFromFs, "");
                        return false;
                    }
                }
            }

            return true;
        }

        #region static cache operations

        /// <summary>
        /// Loads a list of json chat rooms from fs
        /// </summary>
        /// <returns><see cref="List{string}"/></returns>
        public static List<string> ChatRoomNumbersFromFs()
        {

            List<string> chatRooms = new List<string>();
            string[] csr = new string[0];

            try
            {
                csr = Directory.GetFiles(LibPaths.SystemDirJsonPath, "room*.json");
                string file = "";
                foreach (string filedir in csr)
                {
                    file = Path.GetFileName(filedir);
                    chatRooms.Add(file);
                }
            }
            catch (Exception exChatRoomFs)
            {
                Area23Log.LogStatic("ChatRoomNumbersFromFs()", exChatRoomFs, "");
            }

            SetJsonChatRoomsToCache(chatRooms);


            return chatRooms;
        }

        /// <summary>
        /// GetJsonChatRoomsFromCache loads chatroom list from ApplicationState in proc or redis cache
        /// if no entries are found there, GetJsonChatRoomsFromCache() loads chatroom list from filesystem
        /// and stores it either in ApplicationState in proc or in redis elastic cache
        /// </summary>
        /// <returns><see cref="List{string}<">List of chat room names / keys</see></returns>
        public static List<string> GetJsonChatRoomsFromCache()
        {
            List<string> chatRooms = new List<string>();

            try
            {
                chatRooms = (List<string>)MemoryCache.CacheDict.GetValue<List<string>>(Constants.CHATROOMS);
            }
            catch (Exception exLoadFromCache)
            {
                Area23Log.LogStatic("GetJsonChatRoomsFromCache(): Failed to load chatrooms from cache", exLoadFromCache, "");
            }


            if (chatRooms == null || chatRooms.Count < 1)
            {
                chatRooms = ChatRoomNumbersFromFs();
                if (chatRooms != null && chatRooms.Count > 0)
                    SetJsonChatRoomsToCache(chatRooms);
            }

            return chatRooms;

        }

        /// <summary>
        /// SetJsonChatRoomsToCache persist list of chat rooms of 
        /// </summary>
        /// <param name="chatRooms"><see cref="List{string}">list of chat rooms</see></param>
        public static void SetJsonChatRoomsToCache(List<string> chatRooms)
        {
            MemoryCache.CacheDict.SetValue<List<string>>(Constants.CHATROOMS, chatRooms);
        }

        public static void AddJsonChatRoomToCache(string chatRoom)
        {
            List<string> chatRooms = GetJsonChatRoomsFromCache();
            if (!chatRooms.Contains(chatRoom))
                chatRooms.Add(chatRoom);
            SetJsonChatRoomsToCache(chatRooms);
        }

        public static void DeleteJsonChatRoomFromCache(string chatRoom)
        {
            List<string> chatRooms = GetJsonChatRoomsFromCache();
            
            if (MemoryCache.CacheDict.ContainsKey(chatRoom))
                MemoryCache.CacheDict.RemoveKey(chatRoom);

            if (chatRooms.Contains(chatRoom)) 
                chatRooms.Remove(chatRoom);

            SetJsonChatRoomsToCache(chatRooms);
        }

        #endregion static cache operations

        #endregion static members

    }
}