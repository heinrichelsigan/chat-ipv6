using Area23.At.Framework.Library;
using Area23.At.Framework.Library.CqrXs.CqrMsg;
using Area23.At.Framework.Library.Static;
using Area23.At.Framework.Library.Util;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Web;


namespace EU.CqrXs.CqrSrv.CqrJd.Util
{
    /// <summary>
    /// JsonChatRoom 
    /// </summary>
    public class JsonChatRoom
    {        

        static object _lock = new object();
        static HashSet<string> _chatRooms;
        
        public string JsonChatRoomNumber { get; set; } = System.DateTime.Now.ToString();

        internal string JsonChatRoomFileName { get => LibPaths.SystemDirJsonPath + JsonChatRoomNumber; }

        static JsonChatRoom()
        {
            _chatRooms = new HashSet<string>(ChatRoomNumbersFromFs());
        }

        public JsonChatRoom()
        {
            if (_chatRooms == null)
                _chatRooms = new HashSet<string>(ChatRoomNumbersFromFs());
        }

        public JsonChatRoom(string jsonChatRoomNumber) : this()
        {
            if (string.IsNullOrEmpty(jsonChatRoomNumber))
                jsonChatRoomNumber = "room_unknown_" + System.DateTime.Now.Area23DateTimeWithMillis() + ".json";

            JsonChatRoomNumber = (jsonChatRoomNumber.Equals(".json")) ? jsonChatRoomNumber : jsonChatRoomNumber + ".json";
        }


        public FullSrvMsg<string> LoadJsonChatRoom(FullSrvMsg<string> fullSrvMsgIn, string ChatRoomNr)
        {
            JsonChatRoomNumber = ChatRoomNr;
            FullSrvMsg<string> fullServerMessage = null;
            string jsonText = null;
            if (!System.IO.File.Exists(JsonChatRoomFileName)) // we need to create chatroom
            {
                SaveJsonChatRoom(fullSrvMsgIn, ChatRoomNr);
            }

            lock (_lock)
            {
                jsonText = System.IO.File.ReadAllText(JsonChatRoomFileName);
                fullServerMessage = JsonConvert.DeserializeObject<FullSrvMsg<string>>(jsonText);
            }
            fullServerMessage._message = jsonText;

            return fullServerMessage;
        }

        
        public FullSrvMsg<string> SaveJsonChatRoom(FullSrvMsg<string> fullSrvMsg, string ChatRoomNr)
        {
            string jsonString = "";
            lock (_lock)
            {
                if (!ChatRoomNr.Equals(this.JsonChatRoomNumber))
                    JsonChatRoomNumber = ChatRoomNr;

                if (!JsonChatRoomNumber.EndsWith(".json"))
                    JsonChatRoomNumber += ".json";

                fullSrvMsg.ChatRoomNr = JsonChatRoomNumber;
                fullSrvMsg.Sender.ChatRoomNr = JsonChatRoomNumber;
                fullSrvMsg.Sender.ChatRuid = fullSrvMsg.ChatRuid;
                fullSrvMsg.RawMessage = "";
                fullSrvMsg._message = "";

                JsonSerializerSettings jsets = new JsonSerializerSettings();
                jsets.Formatting = Formatting.Indented;
                jsonString = JsonConvert.SerializeObject(fullSrvMsg, Formatting.Indented);
                System.IO.File.WriteAllText(JsonChatRoomFileName, jsonString);
            }
            
            fullSrvMsg._message = jsonString; 
            
            return fullSrvMsg;
        }


        public bool DeleteJsonChatRoom(string ChatRoomNr)
        {
            JsonChatRoomNumber = ChatRoomNr;
           
            if (BaseWebService.PersistMsgInApplicationState)
            {
                if (HttpContext.Current.Application.AllKeys.Contains(JsonChatRoomNumber))
                    HttpContext.Current.Application.Remove(JsonChatRoomNumber);                    
            }
            if (BaseWebService.PersistMsgInAmazonElasticCache)
            {
                RedIs.Db.StringGetDelete(JsonChatRoomNumber, StackExchange.Redis.CommandFlags.FireAndForget);
            }
            DeleteJsonChatRoomFromCache(JsonChatRoomNumber);

            lock (_lock)
            {
                if (System.IO.File.Exists(JsonChatRoomFileName)) // we need to create chatroom
                {
                    try
                    {
                        System.IO.File.Delete(JsonChatRoomFileName);
                    }
                    catch (Exception e)
                    {
                        Area23Log.LogStatic($"Error deleting chat room {e.Message}");
                        return false;
                    }
                }
            }

            return true;
        }


        public static FullSrvMsg<string> LoadChatRoom(FullSrvMsg<string> fullSrvMsgIn, string chatRoomNr)
        {
            JsonChatRoom jsonChatRoom = new JsonChatRoom(chatRoomNr);
            jsonChatRoom.JsonChatRoomNumber = chatRoomNr;
            
            FullSrvMsg<string> fullServerMessage = null;
            string jsonText = null;
            if (!System.IO.File.Exists(jsonChatRoom.JsonChatRoomFileName)) // we need to create chatroom
            {
                jsonChatRoom.SaveJsonChatRoom(fullSrvMsgIn, chatRoomNr);
            }

            lock (_lock)
            {
                jsonText = System.IO.File.ReadAllText(jsonChatRoom.JsonChatRoomFileName);
                fullServerMessage = JsonConvert.DeserializeObject<FullSrvMsg<string>>(jsonText);
            }
            fullServerMessage._message = jsonText;
            // fullSrvMsgOut.RawMessage = jsonText;

            return fullServerMessage;
        }

       

        /// <summary>
        /// Loads a list of json chat rooms from fs
        /// </summary>
        /// <returns><see cref="List{string}"/></returns>
        public static List<string> ChatRoomNumbersFromFs()
        {

            List<string> chatRooms = new List<string>();
            string[] csr = Directory.GetFiles(LibPaths.SystemDirJsonPath, "room*.json");
            string file = "";
            foreach (string filedir in csr)
            {
                file = Path.GetFileName(filedir);
                chatRooms.Add(file);
            }

            SetJsonChatRoomsToCache(chatRooms);

            return chatRooms;
        }



        public static List<string> GetJsonChatRoomsFromCache()
        {
            List<string> chatRooms = new List<string>();
            if (BaseWebService.PersistMsgInApplicationState)
                chatRooms = (List<string>)HttpContext.Current.Application[Constants.CHATROOMS];
            if (BaseWebService.PersistMsgInAmazonElasticCache)
            {
                string chatRoomsJson = RedIs.Db.StringGet(Constants.CHATROOMS);
                chatRooms = JsonConvert.DeserializeObject<List<string>>(chatRoomsJson);
            }

            if (chatRooms == null || chatRooms.Count < 1)
                chatRooms = ChatRoomNumbersFromFs();

            return chatRooms;

        }


        public static void SetJsonChatRoomsToCache(List<string> chatRooms)
        {
            if (BaseWebService.PersistMsgInApplicationState)
                HttpContext.Current.Application[Constants.CHATROOMS] = chatRooms;
            if (BaseWebService.PersistMsgInAmazonElasticCache)
            {
                string chatRoomsJson = JsonConvert.SerializeObject(chatRooms);
                RedIs.Db.StringSet(Constants.CHATROOMS, chatRoomsJson);
            }
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
            if (chatRooms.Contains(chatRoom))
                chatRooms.Remove(chatRoom);
            SetJsonChatRoomsToCache(chatRooms);
        }

    }
}