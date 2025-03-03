using Area23.At.Framework.Library;
using Area23.At.Framework.Library.CqrXs.CqrMsg;
using Area23.At.Framework.Library.Static;
using Area23.At.Framework.Library.Util;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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


        public FullSrvMsg<string> LoadJsonChatRoom(string chatRoomNumber)
        {
            FullSrvMsg<string> fullSrvMsg;

            JsonChatRoomNumber = chatRoomNumber;
            if (!System.IO.File.Exists(JsonChatRoomFileName))
                return null;

            lock (_lock)
            {
                string jsonText = System.IO.File.ReadAllText(JsonChatRoomFileName);
                FullSrvMsg<string> srvMsg1 = new FullSrvMsg<string>();
                srvMsg1.FromJson(jsonText);
                fullSrvMsg = JsonConvert.DeserializeObject<FullSrvMsg<string>>(jsonText);
                if (!string.IsNullOrEmpty(fullSrvMsg.Message) && fullSrvMsg.TContent.Equals(JsonChatRoomNumber))
                    HttpContext.Current.Application["ChatRoom"] = fullSrvMsg;
            }
            return fullSrvMsg;
        }

        public void SaveJsonChatRoom(FullSrvMsg<string> fullSrvMsg, string chatRoomId)
        {
            if (!chatRoomId.Equals(this.JsonChatRoomNumber))
                JsonChatRoomNumber = chatRoomId;
            JsonSerializerSettings jsets = new JsonSerializerSettings();
            jsets.Formatting = Formatting.Indented;
            string jsonString = JsonConvert.SerializeObject(fullSrvMsg, Formatting.Indented);
            
            if (!JsonChatRoomNumber.EndsWith(".json"))
                JsonChatRoomNumber += ".json";

            fullSrvMsg.ChatRoomNr = JsonChatRoomNumber;

            System.IO.File.WriteAllText(JsonChatRoomFileName, jsonString);
            HttpContext.Current.Application["ChatRoom"] = fullSrvMsg;
        }

        public bool DeleteJsonChatRoom(string chatRoomNumber)
        {
            FullSrvMsg<string> fullSrvMsg;

            JsonChatRoomNumber = chatRoomNumber;
            if (!System.IO.File.Exists(JsonChatRoomFileName))
                return true;

            lock (_lock)
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

            return true;
        }

    }
}