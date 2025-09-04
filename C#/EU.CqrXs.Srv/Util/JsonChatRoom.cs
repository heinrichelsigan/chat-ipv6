using Area23.At.Framework.Library.Cache;
using Area23.At.Framework.Library.Cqr.Msg;
using Area23.At.Framework.Library.Static;
using Area23.At.Framework.Library.Util;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace EU.CqrXs.Srv.Util
{

    /// <summary>
    /// JsonChatRoom 
    /// </summary>
    public static class JsonChatRoom
    {

        #region static fields

        private static object _lock = new object();
        private static HashSet<string> _chatRooms;
        private static string _jsonChatRoomNumber;
        #endregion static fields

        #region static ctor

        /// <summary>
        /// static parameterless connstructor
        /// </summary>
        static JsonChatRoom()
        {
            List<string> jsonChatRooms = new List<string>();
            try
            {
                jsonChatRooms = ChatRoomNumbersFromFs();
            }
            catch (Exception exCtor)
            {
                Area23Log.LogOriginMsgEx("JsonChatRooms", "JsonChatRoom()", exCtor);
            }
            _chatRooms = new HashSet<string>(jsonChatRooms);
            _jsonChatRoomNumber = System.DateTime.Now.ToString();
        }

        #endregion static ctor

        #region static Load Save Delete

        /// <summary>
        /// Static LoadChatRoom
        /// </summary>
        /// <param name="cSrvMsgIn"><see cref="CSrvMsg{TC}"/></param>
        /// <returns>><see cref="CSrvMsg{TC}"/></returns>
        public static CSrvMsg<string> LoadChatRoom(ref CSrvMsg<string> cSrvMsgIn)
        {
            string chatRoomName = "";
            if (cSrvMsgIn != null && cSrvMsgIn.CRoom != null && !string.IsNullOrEmpty(cSrvMsgIn.CRoom.ChatRoomNr))
            {
                chatRoomName = cSrvMsgIn.CRoom.ChatRoomNr;
            }
            string jsonCRoomFileName = GetJsonChatRoomFullPath(chatRoomName);

            string jsonText = null;
            if (!File.Exists(jsonCRoomFileName)) // we need to a create chatroom
            {
                SaveChatRoom(ref cSrvMsgIn);
            }

            CSrvMsg<string> chatRoomMsg;
            lock (_lock)
            {
                jsonText = File.ReadAllText(jsonCRoomFileName);
                chatRoomMsg = JsonConvert.DeserializeObject<CSrvMsg<string>>(jsonText);
            }

            CChatRoom cRoom = JsonChatRoom.GetCachedChatRoom(cSrvMsgIn.CRoom.ChatRoomNr);
            if (cRoom != null && !string.IsNullOrEmpty(cRoom.ChatRoomNr))
                chatRoomMsg.CRoom = cRoom;
            else if (!chatRoomMsg.SerializedMsg.Equals(cSrvMsgIn.SerializedMsg))
            {
                if (chatRoomMsg.CRoom != null && chatRoomMsg.CRoom != null &&
                    // chatRoomMsg.CRoom.ChatRoomNr.Equals(cSrvMsgIn.CRoom.ChatRoomNr, StringComparison.InvariantCultureIgnoreCase) &&
                    !chatRoomMsg.CRoom.SerializedMsg.Equals(cSrvMsgIn.CRoom.SerializedMsg))
                {
                    chatRoomMsg.CRoom.LastPolled = cSrvMsgIn.CRoom.LastPolled;
                    chatRoomMsg.CRoom.LastPushed = cSrvMsgIn.CRoom.LastPushed;
                    chatRoomMsg.CRoom.MsgDict = new Dictionary<long, string>(cSrvMsgIn.CRoom.MsgDict);
                }
            }


            return chatRoomMsg;
        }

        /// <summary>
        /// static SaveChatRoom
        /// </summary>
        /// <param name="cSrvMsg"><see cref="CSrvMsg<TC>"/></param>
        /// <returns></returns>
        public static CSrvMsg<string> SaveChatRoom(ref CSrvMsg<string> cSrvMsg)
        {
            if (cSrvMsg == null || cSrvMsg.CRoom == null)
                throw new ArgumentNullException("cSrvMsg.CRoom");

            CChatRoom chatRoom = cSrvMsg.CRoom;
            string chatRoomNumber = cSrvMsg.CRoom.ChatRoomNr;

            lock (_lock)
            {
                if (string.IsNullOrEmpty(chatRoomNumber) || chatRoomNumber.Length < 6)
                {
                    string restMail = cSrvMsg.Sender.Email.Contains("@") ? (cSrvMsg.Sender.Email.Substring(0, cSrvMsg.Sender.Email.IndexOf("@"))) : cSrvMsg.Sender.Email.Trim();
                    restMail = restMail.Replace("@", "_").Replace(".", "_");

                    if (!string.IsNullOrEmpty(restMail))
                        chatRoomNumber = String.Format("room_{0:MMddHHmm}_{1}.json", DateTime.Now, restMail);
                    else
                        chatRoomNumber = $"room_{DateTime.Now:MMddHHmm}.json";
                }

                if (!chatRoomNumber.EndsWith(".json"))
                    chatRoomNumber += ".json";

                cSrvMsg.CRoom.ChatRoomNr = chatRoomNumber;
                cSrvMsg.CRoom.Message = chatRoomNumber;

                if (cSrvMsg.CRoom != null && !string.IsNullOrEmpty(cSrvMsg.CRoom.ChatRoomNr))
                    cSrvMsg.Sender.Message = cSrvMsg.CRoom.ChatRoomNr;

                string jsonCRoomFileName = GetJsonChatRoomFullPath(chatRoomNumber);
                string serialized = SerializeCSrvMsg(ref cSrvMsg, true);
            }

            AddJsonChatRoomToCache(chatRoomNumber);
            SetCachedChatRoom(chatRoomNumber, cSrvMsg.CRoom);

            return cSrvMsg;
        }

        /// <summary>
        /// Deletes chatroom from cache and skeleton without messages from filesystem 
        /// </summary>
        /// <param name="cSrvMsg">full <see cref="CSrvMsg{string}"/> including <see cref="CSrvMsg{string}.CRoom"/></param>
        /// <returns></returns>
        public static bool DeleteChatRoom(ref CSrvMsg<string> cSrvMsg)
        {

            if (cSrvMsg.CRoom != null && !string.IsNullOrEmpty(cSrvMsg.CRoom.ChatRoomNr))
            {

                string jsonChatRoomFileName = GetJsonChatRoomFullPath(cSrvMsg.CRoom.ChatRoomNr);

                foreach (var keyEntry in cSrvMsg.CRoom.MsgDict.Keys)
                {
                    MemoryCache.CacheDict.RemoveKey(keyEntry.ToString());
                }

                DeleteJsonChatRoomFromCache(cSrvMsg.CRoom.ChatRoomNr);

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
                            Area23Log.LogOriginMsgEx("JsonChatRooms",
                                $"DeleteChatRoom(string chatRoomNr = {cSrvMsg.CRoom.ChatRoomNr}): Error deleting chat room ", exDelChatRoomFromFs);
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        #endregion static Load Save Delete

        #region basic static members

        /// <summary>
        /// SerializeCSrvMsg serializes a <see cref="CSrvMsg{TC}"/> to file system 
        /// TODO: we need only <see cref="CChatRoom"/> to merialize => FIX-IT
        /// </summary>
        /// <param name="cSrvMsg">a full CSrvMsg</param>
        /// <param name="wrtieJsonToFs">if true, serialize it to fs, default false</param>
        /// <returns>serialized strin</returns>
        public static string SerializeCSrvMsg(ref CSrvMsg<string> cSrvMsg, bool wrtieJsonToFs = false)
        {
            // TODO: we need only<see cref = "CChatRoom" /> to merialize => FIX  IT!!!
            string serializedJsonString = string.Empty;
            if (cSrvMsg != null)
            {
                // cSrvMsg.SerializedMsg = string.Empty;
                JsonSerializerSettings jsets = new JsonSerializerSettings();
                jsets.Formatting = Formatting.Indented;
                jsets.MaxDepth = 16;
                serializedJsonString = JsonConvert.SerializeObject(cSrvMsg, Formatting.Indented);

                if (wrtieJsonToFs && cSrvMsg.CRoom != null && !string.IsNullOrEmpty(cSrvMsg.CRoom.ChatRoomNr))
                {
                    string fullPath = JsonChatRoom.GetJsonChatRoomFullPath(cSrvMsg.CRoom.ChatRoomNr);
                    File.WriteAllText(fullPath, serializedJsonString);
                }
            }

            // cSrvMsg.SerializedMsg = serializedJsonString;
            return serializedJsonString;
        }

        /// <summary>
        /// GetJsonChatRoomFullPath gets full path in filesystem for json chat room skeleton file
        /// </summary>
        /// <param name="jsonChatRoomNr">name or number of chat room</param>
        /// <returns></returns>
        public static string GetJsonChatRoomFullPath(string jsonChatRoomNr)
        {
            string fullPath = Path.Combine(LibPaths.SystemDirJsonPath, jsonChatRoomNr);

            return fullPath;
        }

        /// <summary>
        /// ChatRoomCheckPermission
        /// Validates, if a user has permission to poll or to push messages in the chat room by the following steps:
        /// 1. chat room number in encrypted, now decrypted msg from webservice must be ident with chat room number from json file
        /// 2. sender email from  encrypted, now decrypted msg from webservice must much
        /// 2.a. either creator invitor of chat room
        /// 2.b. on of the invited persons in invitation
        /// </summary>
        /// <param name="cSrvMsg"><see cref="CSrvMsg{string}"/> decoded from <see cref="CqrService.CqrService"/> Webservice</param>
        /// <param name="isClosingRequest"><see cref="bool"/>default false, true when closing and deleting chat room</param>
        /// <returns>>true, if person is allowed to push or receive msg from / to chat room</returns>        
        public static bool CheckPermission(ref CSrvMsg<string> cSrvMsg, bool isClosingRequest = false)
        {
            bool isValid = false;
            string chatRoomNr = cSrvMsg.CRoom.ChatRoomNr;
            CSrvMsg<string> chatRoomMsg = JsonChatRoom.LoadChatRoom(ref cSrvMsg);
            if (chatRoomNr.Equals(chatRoomMsg.CRoom.ChatRoomNr, StringComparison.CurrentCultureIgnoreCase)) // validate chat number
            {
                // chatRoomMsg.TContent = string.Empty;
                chatRoomMsg.Message = chatRoomNr;

                if ((!string.IsNullOrEmpty(cSrvMsg.Sender.Email) && cSrvMsg.Sender.Email.Equals(chatRoomMsg.Sender.Email, StringComparison.CurrentCultureIgnoreCase)) ||
                    (!string.IsNullOrEmpty(cSrvMsg.Sender.NameEmail) && cSrvMsg.Sender.NameEmail.Equals(chatRoomMsg.Sender.NameEmail, StringComparison.InvariantCultureIgnoreCase)))
                {
                    isValid = true;
                    return isValid;
                }
                if (!isClosingRequest)
                {
                    foreach (CContact c in chatRoomMsg.Recipients)
                    {
                        if (cSrvMsg.Sender.NameEmail.Equals(c.NameEmail, StringComparison.CurrentCultureIgnoreCase) ||
                            cSrvMsg.Sender.Email.Equals(c.Email, StringComparison.CurrentCultureIgnoreCase) ||
                            (cSrvMsg.Sender.Name.Equals(c.Name, StringComparison.CurrentCultureIgnoreCase) && cSrvMsg.Sender.Cuid == c.Cuid))
                        {
                            isValid = true;
                            return isValid;
                        }
                    }
                }
            }

            return isValid;
        }

        #endregion basic static members

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
                Area23Log.LogOriginMsgEx("JsonChatRooms", "ChatRoomNumbersFromFs()", exChatRoomFs);
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
                Area23Log.LogOriginMsgEx("JsonChatRooms", "GetJsonChatRoomsFromCache(): Failed to load chatrooms from cache", exLoadFromCache);
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


        /// <summary>
        /// AddJsonChatRoomToCache adds chatRoomName to cache variable ChatRooms, which contains all chat room names 
        /// </summary>
        /// <param name="chatRoomNr">name of chat room to add to ChatRooms array in cache</param>
        public static void AddJsonChatRoomToCache(string chatRoomNr)
        {
            List<string> chatRooms = GetJsonChatRoomsFromCache();
            if (!chatRooms.Contains(chatRoomNr))
                chatRooms.Add(chatRoomNr);

            SetJsonChatRoomsToCache(chatRooms);
        }


        /// <summary>
        /// GetCachedMessageDict returns one chat room message dictionary
        /// either from Application State in proc or from Valkey Elastic Cache on AWS
        /// </summary>
        /// <param name="chatRoomNumber">json chat room number</param>
        /// <returns>one chat room message dictionary</returns>
        public static CChatRoom GetCachedChatRoom(string chatRoomNumber)
        {
            CChatRoom chatRoomt = (CChatRoom)MemoryCache.CacheDict.GetValue<CChatRoom>(chatRoomNumber);

            // TODO: implement filesystem 

            return chatRoomt;

        }

        /// <summary>
        /// SetCachedMessageDict saves the mesage dictionary for chat room in 
        /// either application state in proc or Amazon Valkey Elastic cache
        /// </summary>
        /// <param name="chatRoomNumber">json chat room number</param>
        /// <param name="dict">the mesage dictionary for chat room </param>
        public static void SetCachedChatRoom(string chatRoomNumber, CChatRoom chatRoom)
        {

            Area23Log.LogOriginMsg("CqrService.asmx", $"SetCachedChatRoom(string chatroomNumber = " + chatRoomNumber +
                        $") chatRoom.MsgDict.Count = {chatRoom.MsgDict.Count}.\r\n");

            MemoryCache.CacheDict.SetValue<CChatRoom>(chatRoomNumber, chatRoom);

            return;
        }


        /// <summary>
        /// Delete Json ChatRoom from cache
        /// - gets current cache key ChatRooms, which contains a array of all chat room names form cache
        /// - removes key with chat room name from cahce (with entire chatroom messages)
        /// - removes chatroom name from cache key Chatrooms
        /// - sets cache key ChatRooms without the deleted chatroom name back to cachg
        /// </summary>
        /// <param name="chatRoomNr">chatroom name</param>
        public static void DeleteJsonChatRoomFromCache(string chatRoomNr)
        {
            List<string> chatRooms = GetJsonChatRoomsFromCache();

            if (MemoryCache.CacheDict.ContainsKey(chatRoomNr))
                MemoryCache.CacheDict.RemoveKey(chatRoomNr);

            if (chatRooms.Contains(chatRoomNr))
                chatRooms.Remove(chatRoomNr);

            SetJsonChatRoomsToCache(chatRooms);
        }

        #endregion static cache operations

    }

}