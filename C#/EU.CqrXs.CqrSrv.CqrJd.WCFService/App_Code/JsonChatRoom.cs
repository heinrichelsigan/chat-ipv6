using Area23.At.Framework.Library.Cache;
using Area23.At.Framework.Library.CqrXs.CqrMsg;
using Area23.At.Framework.Library.Static;
using Area23.At.Framework.Library.Util;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;


/// <summary>
/// JsonChatRoom 
/// </summary>
public class JsonChatRoom
{

    static object _lock = new object();
    static HashSet<string> _chatRooms;

    private string _jsonChatRoomNumber;
    public string JsonChatRoomNumber { get { return _jsonChatRoomNumber; } set { _jsonChatRoomNumber = value; } }

    internal string JsonChatRoomFileName { get { return LibPaths.SystemDirJsonPath + JsonChatRoomNumber; } }

    static JsonChatRoom()
    {        
        _chatRooms = new HashSet<string>(ChatRoomNumbersFromFs());
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

        if (BaseService.PersistMsgInApplicationState)
        {
            if (HttpContext.Current.Application.AllKeys.Contains(JsonChatRoomNumber))
                HttpContext.Current.Application.Remove(JsonChatRoomNumber);
        }
        if (BaseService.PersistMsgInAmazonElasticCache)
        {
            REdIs.ValKey.DeleteKey(JsonChatRoomNumber, StackExchange.Redis.CommandFlags.FireAndForget);
            // Db.StringGetDelete(JsonChatRoomNumber, StackExchange.Redis.CommandFlags.FireAndForget);
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
                    Area23Log.LogStatic("Error deleting chat room " + e.Message);
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


    /// <summary>
    /// GetJsonChatRoomsFromCache loads chatroom list from ApplicationState in proc or redis cache
    /// if no entries are found there, GetJsonChatRoomsFromCache() loads chatroom list from filesystem
    /// and stores it either in ApplicationState in proc or in redis elastic cache
    /// </summary>
    /// <returns><see cref="List{string}<">List of chat room names / keys</see></returns>
    public static List<string> GetJsonChatRoomsFromCache()
    {
        List<string> chatRooms = new List<string>();
        if (BaseService.PersistMsgInApplicationState)
            chatRooms = (List<string>)HttpContext.Current.Application[Constants.CHATROOMS];
        if (BaseService.PersistMsgInAmazonElasticCache)
        {
            try
            {
                REdIs.ValKey.GetKey<List<string>>(Constants.CHATROOMS);
                // string chatRoomsJson = RedIs.Db.StringGet(Constants.CHATROOMS);
                // chatRooms = JsonConvert.DeserializeObject<List<string>>(chatRoomsJson);
            }
            catch (Exception exLoadFromCache)
            {
                Area23Log.LogStatic("Failed to load chatrooms from cache", exLoadFromCache, "");
            }
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
        if (BaseService.PersistMsgInApplicationState)
            HttpContext.Current.Application[Constants.CHATROOMS] = chatRooms;
        if (BaseService.PersistMsgInAmazonElasticCache)
        {
            REdIs.ValKey.SetKey<List<string>>(Constants.CHATROOMS, chatRooms);            
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