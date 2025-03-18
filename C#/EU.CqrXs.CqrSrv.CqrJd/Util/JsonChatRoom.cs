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
    public class JsonChatRoom
    {        
        static object _lock = new object();
        static HashSet<CqrContact> _contacts;


        public string JsonChatRoomNumber { get; set; } = System.DateTime.Now.ToString();

        internal string JsonChatRoomFileName { get => LibPaths.SystemDirJsonPath + JsonChatRoomNumber; }

        public JsonChatRoom(string jsonChatRoomNumber)
        {
            if (string.IsNullOrEmpty(jsonChatRoomNumber))
                jsonChatRoomNumber = "unknown_" + System.DateTime.Now.Area23DateTimeWithMillis() + ".json";

            JsonChatRoomNumber = (jsonChatRoomNumber.Equals(".json")) ? jsonChatRoomNumber : jsonChatRoomNumber + ".json";
        }


        public FullSrvMsg<string> LoadJsonChatRoom(FullSrvMsg<string> fullSrvMsgIn, string chatRoomId)
        {
            JsonChatRoomNumber = chatRoomId;
            FullSrvMsg<string> fullServerMessage = null;
            string jsonText = null;
            if (!System.IO.File.Exists(JsonChatRoomFileName)) // we need to create chatroom
            {
                SaveJsonChatRoom(fullSrvMsgIn, chatRoomId);
            }

            lock (_lock)
            {
                jsonText = System.IO.File.ReadAllText(JsonChatRoomFileName);
                fullServerMessage = JsonConvert.DeserializeObject<FullSrvMsg<string>>(jsonText);
            }
            fullServerMessage._message = jsonText;
            // fullSrvMsgOut.RawMessage = jsonText;

            return fullServerMessage;
        }

      
        public FullSrvMsg<string> SaveJsonChatRoom(FullSrvMsg<string> fullSrvMsg, string chatRoomId)
        {
            string jsonString = "";
            lock (_lock)
            {
                if (!chatRoomId.Equals(this.JsonChatRoomNumber))
                    JsonChatRoomNumber = chatRoomId;

                if (!JsonChatRoomNumber.EndsWith(".json"))
                    JsonChatRoomNumber += ".json";

                fullSrvMsg.ChatRoomNr = JsonChatRoomNumber;
                fullSrvMsg.Sender.ChatRoomId = JsonChatRoomNumber;
                fullSrvMsg.RawMessage = "";
                fullSrvMsg._message = "";

                JsonSerializerSettings jsets = new JsonSerializerSettings();
                jsets.Formatting = Formatting.Indented;
                jsonString = JsonConvert.SerializeObject(fullSrvMsg, Formatting.Indented);
                System.IO.File.WriteAllText(JsonChatRoomFileName, jsonString);
            }
             
            fullSrvMsg._message = jsonString;           
            // fullSrvMsg.RawMessage = jsonString; 
            
            
            return fullSrvMsg;
        }

        public bool DeleteJsonChatRoom(string chatRoomId)
        {
            JsonChatRoomNumber = chatRoomId;
            lock (_lock)
            {
                if (BaseWebService.PersistMsgInApplicationState)
                {
                    if (HttpContext.Current.Application.AllKeys.Contains(JsonChatRoomNumber))
                        HttpContext.Current.Application.Remove(JsonChatRoomNumber);
                }
                if (BaseWebService.PersistMsgInAmazonElasticCache)
                {
                    RedIs.Db.StringGetDelete(JsonChatRoomNumber, StackExchange.Redis.CommandFlags.FireAndForget);
                }
                

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


        public HashSet<string> ChatRoomNumbersFromFs()
        {
            HashSet<string> chatRooms = JsonContacts.ChatRoomNumbersFromFs().ToHashSet();
            return chatRooms;
        }

    }
}