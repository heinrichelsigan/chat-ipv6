﻿using Area23.At.Framework.Library;
using Area23.At.Framework.Library.CqrXs.CqrMsg;
using Area23.At.Framework.Library.Static;
using Area23.At.Framework.Library.Util;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
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
            FullSrvMsg<string> fullSrvMsgOut;

            JsonChatRoomNumber = chatRoomId;
            if (!System.IO.File.Exists(JsonChatRoomFileName)) // we need to create chatroom
            {
                SaveJsonChatRoom(fullSrvMsgIn, chatRoomId);
            }

            lock (_lock)
            {
                string jsonText = System.IO.File.ReadAllText(JsonChatRoomFileName);
                fullSrvMsgOut = JsonConvert.DeserializeObject<FullSrvMsg<string>>(jsonText);
                fullSrvMsgOut._message = jsonText;

                HashSet<string> chatRooms = (HttpContext.Current.Application["ChatRooms"] != null)
                    ? (HashSet<string>)HttpContext.Current.Application["ChatRooms"] 
                    : ChatRoomNumbersFromFs();
                if (!chatRooms.Contains(chatRoomId))
                    chatRooms.Add(chatRoomId);                
                HttpContext.Current.Application["ChatRooms"] = chatRooms;

            }

            return fullSrvMsgOut;
        }

        public FullSrvMsg<string> SaveJsonChatRoom(FullSrvMsg<string> fullSrvMsg, string chatRoomId)
        {
            if (!chatRoomId.Equals(this.JsonChatRoomNumber))
                JsonChatRoomNumber = chatRoomId;
            
            if (!JsonChatRoomNumber.EndsWith(".json"))
                JsonChatRoomNumber += ".json";

            fullSrvMsg.ChatRoomNr = JsonChatRoomNumber;
            fullSrvMsg.Sender.ChatRoomId = JsonChatRoomNumber;

            JsonSerializerSettings jsets = new JsonSerializerSettings();
            jsets.Formatting = Formatting.Indented;
            string jsonString = JsonConvert.SerializeObject(fullSrvMsg, Formatting.Indented);
            System.IO.File.WriteAllText(JsonChatRoomFileName, jsonString);            

            HashSet<string> chatRooms = (HttpContext.Current.Application["ChatRooms"] != null)
                    ? (HashSet<string>)HttpContext.Current.Application["ChatRooms"] 
                    : ChatRoomNumbersFromFs();
            if (!chatRooms.Contains(chatRoomId))
                chatRooms.Add(chatRoomId);
            HttpContext.Current.Application["ChatRooms"] = chatRooms;

            fullSrvMsg._message = jsonString;
            
            return fullSrvMsg;
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


        public HashSet<string> ChatRoomNumbersFromFs()
        {
            string[] csr = Directory.GetFiles(LibPaths.SystemDirJsonPath, "room*.json");
            HashSet<string> chatRooms = new HashSet<string>(csr);
            HttpContext.Current.Application["ChatRooms"] = chatRooms;
            return chatRooms;
        }

    }
}