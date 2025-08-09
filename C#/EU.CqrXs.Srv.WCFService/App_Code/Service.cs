using Area23.At.Framework.Library.Cache;
using Area23.At.Framework.Library.Cqr;
using Area23.At.Framework.Library.Cqr.Msg;
using Area23.At.Framework.Library.Crypt.EnDeCoding;
using Area23.At.Framework.Library.Static;
using Area23.At.Framework.Library.Util;
using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Web;
using System.Web.Services;

// NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service" in code, svc and config file together.
public class Service : BaseService, IService
{

    /// <summary>
    /// Send1StSrvMsg sends first registration message of contact
    /// </summary>
    /// <param name="cryptMsg">with sercerkey encrypted message</param>
    /// <returns>with serverkey encrypted responnse of own contact</returns>
    public string Send1StSrvMsg(string cryptMsg)
    {
        Area23Log.LogStatic("Send1StSrvMsg(string cryptMsg) called.  cryptMsg.Length = " + cryptMsg.Length + ".\n");
        InitMethod();

        MemoryCache.CacheDict.SetValue<string>("lastmsg", cryptMsg);
        
        CContact aContact = new CContact() { _hash = cqrFacade.PipeString };

        try
        {
            if (!string.IsNullOrEmpty(cryptMsg) && cryptMsg.Length >= 8)
            {
                _contact = aContact.DecryptFromJson(_serverKey, cryptMsg);
                _decrypted = _contact.ToJson();
                Area23Log.LogStatic("Contact decrypted successfully: " + _decrypted + "\n");
            }
        }
        catch (Exception ex)
        {
            CqrException.SetLastException(ex);
            Area23Log.LogStatic("Exception " + ex.GetType() + " when decrypting contact: " + ex.Message + "\n\t" + ex.ToString() + "\n");
        }

        _responseString = _contact.EncryptToJson(_serverKey);

        if (!string.IsNullOrEmpty(_decrypted) && _contact != null && !string.IsNullOrEmpty(_contact.NameEmail))
        {
            MemoryCache.CacheDict.SetValue<string>("lastdecrypted", _decrypted);

            CContact foundCt = JsonContacts.AddContact(_contact);
            _responseString = foundCt.EncryptToJson(_serverKey);
        }

        Area23Log.LogStatic("Send1StSrvMsg(string cryptMsg) finished.  _contact.Cuid = " + _contact.Cuid + ".\n");
        return _responseString;
    }

    /// <summary>
    /// Invites to a chat romm 
    /// with an encrypted <see cref="FullSrvMsg<string>"/>
    /// </summary>
    /// <param name="cryptMsg">encrypted <see cref="FullSrvMsg<string>"/></param>
    /// <returns>encrypted <see cref="FullSrvMsg<string>"/> including chatroom number</returns>
    public string ChatRoomInvite(string cryptMsg)
    {
        Area23Log.LogStatic("ChatRoomInvite(string cryptMsg) called.  cryptMsg.Length = " + cryptMsg.Length + ".\n");
        InitMethod();

        _chatRoomNumber = "";
        CSrvMsg<string> chatRoomMsg, cSrvMsg, aSrvMsg = new CSrvMsg<string>() { _hash = cqrFacade.PipeString };

        _responseString = "";

        try
        {
            if (!string.IsNullOrEmpty(cryptMsg) && cryptMsg.Length >= 8)
            {
                cSrvMsg =  aSrvMsg.DecryptFromJson(_serverKey, cryptMsg);   // decrypt FullSrvMsg<string>            
                _contact = JsonContacts.AddContact(cSrvMsg.Sender);         // add contact from FullSrvMsg<string>   
                chatRoomMsg = InviteToChatRoom(cSrvMsg);                    // generate a FullSrvMsg<string> chatserver message by inviting                           

                _responseString = chatRoomMsg.EncryptToJson(_serverKey);
            }
        }
        catch (Exception ex)
        {
            CqrException.SetLastException(ex);
            Area23Log.LogStatic(ex);
        }

        Area23Log.LogStatic("ChatRoomInvite(string cryptMsg) finished. ChatRoomNr = " + _chatRoomNumber + ".\n");
        return _responseString;

    }

    /// <summary>
    /// Polls a chat room for new messages
    /// </summary>
    /// <param name="cryptMsg">encrypted <see cref="FullSrvMsg<string>"/> with chat room number and last polled dates</param>
    /// <returns>
    /// encrypted <see cref="FullSrvMsg<string>"/> including chatroom number 
    /// with encrypted clientmsg with clientkey.
    /// Server doesn't know client key and always delivers encrypted encrypted messages
    /// Server can only read and decrypt outer envelope message encrypted with server key
    /// </returns>
    public string ChatRoomPoll(string cryptMsg)
    {
        Area23Log.LogStatic("ChatRoomPoll(string cryptMsg) called.  cryptMsg.Length = " + cryptMsg.Length + ".\n");
        InitMethod();

        Dictionary<long, string> dict = new Dictionary<long, string>();
        bool isValid = false;

        CSrvMsg<string> cSrvMsg, aSrvMsg = new CSrvMsg<string>() { _hash = cqrFacade.PipeString };

        _responseString = "";

        try
        {
            if (!string.IsNullOrEmpty(cryptMsg) && cryptMsg.Length >= 8)
            {
                cSrvMsg = aSrvMsg.DecryptFromJson(_serverKey, cryptMsg);           // decrypt FullSrvMsg<string>
                _contact = cSrvMsg.Sender;
                _chatRoomNumber = (cSrvMsg.CRoom != null && !string.IsNullOrEmpty(cSrvMsg.CRoom.ChatRoomNr)) ? cSrvMsg.CRoom.ChatRoomNr : "";

                CSrvMsg<string> chatRoomMsg = JsonChatRoom.LoadChatRoom(cSrvMsg, _chatRoomNumber);
                cSrvMsg = JsonChatRoom.CheckPermission(cSrvMsg, chatRoomMsg, _chatRoomNumber, out isValid);

                if (isValid)
                {
                    dict = GetCachedMessageDict(_chatRoomNumber);

                    List<long> pollKeys = GetNewMessageIndices(dict.Keys.ToList(), cSrvMsg);

                    long polledPtr = -1;
                    if (pollKeys.Count > 0)
                    {
                        polledPtr = pollKeys[0];
                        string firstPollClientMsg = dict[polledPtr];
                        if (string.IsNullOrEmpty(firstPollClientMsg) && pollKeys.Count > 1)
                        {
                            chatRoomMsg = AddLastDate(chatRoomMsg, polledPtr, false);
                            polledPtr = pollKeys[1];
                            firstPollClientMsg = dict[polledPtr];
                        }

                        chatRoomMsg.TContent = firstPollClientMsg;

                        chatRoomMsg = AddLastDate(chatRoomMsg, polledPtr, false);

                        JsonContacts.UpdateContact(chatRoomMsg.Sender);
                        chatRoomMsg = JsonChatRoom.SaveChatRoom(chatRoomMsg, chatRoomMsg.CRoom);
                    }

                }

                _responseString = chatRoomMsg.EncryptToJson(_serverKey);

            }
        }
        catch (Exception ex)
        {
            CqrException.SetLastException(ex);
            Area23Log.LogStatic(ex);
        }

        Area23Log.LogStatic("ChatRoomPushMessage(string cryptMsg, string chatRoomMembersCrypted) finihed. ChatRoomNr =  " + _chatRoomNumber + ".\n");
        return _responseString;
    }

    /// <summary>
    /// Pushes a new message for chatroom to the server
    /// </summary>
    /// <param name="cryptMsg">encrypted <see cref="CSrvMsg{string}"/> with chat room number and last polled dates</param>
    /// <param name="chatRoomMembersCrypted">with client key encrypted message, that is stored in proc of server, but server can't decrypt</param>
    /// <returns>encrypted <see cref="CSrvMsg{string}"/> with chat room number and last polled date changed to now</returns>
    public string ChatRoomPush(string cryptMsg)
    {
        Area23Log.LogStatic("ChatRoomPushMessage(string cryptMsg) called.\n");
        InitMethod();
        string chatRoomMembersCrypted = "";
        bool isValid = false;
        Dictionary<long, string> dict;

        CSrvMsg<string> cSrvMsg, aSrvMsg = new CSrvMsg<string>(cryptMsg, CType.Json) { _hash = cqrFacade.PipeString, SerializedMsg = cryptMsg };
        aSrvMsg = aSrvMsg.FromJson(cryptMsg);

        _responseString = ""; // set empty response string per default
        CSrvMsg<string> chatRoomMsg = new CSrvMsg<string>(); // construct an empty message

        try
        {
            if (!string.IsNullOrEmpty(cryptMsg) && cryptMsg.Length >= 8)
            {
                // cSrvMsg = CSrvMsg<string>.FromJsonDecrypt(_serverKey, cryptMsg);            // decrypt FullSrvMsg<string>
                cSrvMsg = aSrvMsg.DecryptFromJson(_serverKey, cryptMsg);                    // decrypt FullSrvMsg<string>
                _contact = cSrvMsg.Sender;
                _chatRoomNumber = (cSrvMsg.CRoom != null && !string.IsNullOrEmpty(cSrvMsg.CRoom.ChatRoomNr))
                    ? cSrvMsg.CRoom.ChatRoomNr : "";                                        // get chat room number
                chatRoomMembersCrypted = cSrvMsg.TContent;                                  // set chatRoomMembersCrypted to cSrvMsg.TContent

                Area23Log.LogStatic(String.Format("string chatRoomMembersCrypted = cSrvMsg.TContent; \r\n\tchatRoomMembersCrypted len = {0}.\n", chatRoomMembersCrypted.Length));
                chatRoomMsg = JsonChatRoom.LoadChatRoom(cSrvMsg, _chatRoomNumber);          // Load json chat room from file system json file                                                                                                                  
                cSrvMsg = JsonChatRoom.CheckPermission(cSrvMsg, chatRoomMsg,                // Check sender's permission to access chat room (must be creator or invited)
                    _chatRoomNumber, out isValid);

                if (isValid)
                {
                    DateTime now = DateTime.Now;                                            // Determine DateTime.Now

                    dict = GetCachedMessageDict(_chatRoomNumber);                           // Get chatroom message dictionary out of cache

                    dict.Add(now.Ticks, chatRoomMembersCrypted);                            // Add new entry to cached chatroom message dictionary with DateTime.Now
                    chatRoomMsg.CRoom.TicksLong.Add(now.Ticks);
                    chatRoomMsg.CRoom.LastPushed = now;
                    SetCachedMessageDict(_chatRoomNumber, dict);                            // Saves chatroom msg dict back to cache (Amazon valkey or ApplicationState)

                    // UpdateContact(_contact);        
                    chatRoomMsg.TContent = "";                                              // set TContent empty, because we don't want a same huge response as request                                             
                    chatRoomMsg = JsonChatRoom.SaveChatRoom(
                        chatRoomMsg, chatRoomMsg.CRoom);                                    // saves chat room back to json file

                    chatRoomMsg.CRoom.LastPushed = now;
                    chatRoomMsg.CRoom.TicksLong.Remove(now.Ticks);                          // TODO: Delete later, with that, you get your own message in sended queue
                    chatRoomMsg.Sender._message = _chatRoomNumber;
                }
                else
                    chatRoomMsg.TContent = cSrvMsg.Sender.NameEmail + " has no permission for chat room " + _chatRoomNumber;

                _responseString = chatRoomMsg.EncryptToJson(_serverKey);

            }
        }
        catch (Exception ex)
        {
            CqrException.SetLastException(ex);
            Area23Log.LogStatic(ex);
        }

        Area23Log.LogStatic(String.Format("ChatRoomPushMessage(string cryptMsg, string chatRoomMembersCrypted) finished. ChatRoomNr =  {0}.\n", _chatRoomNumber));
        return _responseString;
    }

    /// <summary>
    /// ChatRoomClose
    /// </summary>
    /// <param name="cryptMsg"></param>
    /// <returns></returns>
    public string ChatRoomClose(string cryptMsg)
    {
        Area23Log.LogStatic(String.Format("ChatRoomClose(string cryptMsg) started. cryptMsg.Length =  {0}.\n", cryptMsg.Length));
        InitMethod();
        bool isValid = false;

        CSrvMsg<string> cSrvMsg, aSrvMsg = new CSrvMsg<string>(cryptMsg, CType.Json) { _hash = cqrFacade.PipeString, SerializedMsg = cryptMsg };
        aSrvMsg = aSrvMsg.FromJson(cryptMsg);
        List<CContact> _invited = new List<CContact>();

        _responseString = "";

        try
        {
            if (!string.IsNullOrEmpty(cryptMsg) && cryptMsg.Length >= 8)
            {
                cSrvMsg = CSrvMsg<string>.FromJsonDecrypt(_serverKey, cryptMsg);
                cSrvMsg = aSrvMsg.DecryptFromJson(_serverKey, cryptMsg);                    // decrypt FullSrvMsg<string>
                _contact = JsonContacts.AddContact(cSrvMsg.Sender);
                _chatRoomNumber = (cSrvMsg.CRoom != null && !string.IsNullOrEmpty(cSrvMsg.CRoom.ChatRoomNr)) ? cSrvMsg.CRoom.ChatRoomNr : "";

                CSrvMsg<string> chatRoomMsg = JsonChatRoom.LoadChatRoom(cSrvMsg, _chatRoomNumber);
                cSrvMsg = JsonChatRoom.CheckPermission(cSrvMsg, chatRoomMsg, _chatRoomNumber, out isValid, true);

                if (isValid)
                {
                    if (JsonChatRoom.DeleteChatRoom(_chatRoomNumber))
                    {
                        chatRoomMsg.CRoom = null;
                        chatRoomMsg.Sender._message = "";
                    }
                }

                _responseString = chatRoomMsg.EncryptToJson(_serverKey);

            }
        }
        catch (Exception ex)
        {
            CqrException.SetLastException(ex);
            Area23Log.LogStatic(ex);
        }

        Area23Log.LogStatic(String.Format("ChatRoomClose(string cryptMsg) finished. deleted chat room ChatRoomNr =  {0}.\n", _chatRoomNumber));

        return _responseString;

    }

    // public string GetData(int value)
	// {
	// 	return string.Format("You entered: {0}", value);
	// }

	//public CompositeType GetDataUsingDataContract(CompositeType composite)
	//{
	//	if (composite == null)
	//		throw new ArgumentNullException("composite");
	//	if (composite.BoolValue)

	//		composite.StringValue += "Suffix";
	//	return composite;
	//}

    public string GetIPAddress()
    {
        string userHostAddr = HttpContext.Current.Request.UserHostAddress;
        return userHostAddr;
    }


    public virtual string TestCache()
    {
        string testReport = DateTime.Now.Area23DateTimeMilliseconds() + ": TestCache() started.\n";
        try
        {
            InitMethod();
        }
        catch (Exception ex1)
        {
            testReport += DateTime.Now.Area23DateTimeMilliseconds() + ": Exception " + ex1.GetType() + ": " + ex1.Message + "\n\t" + ex1 + "\n";
        }

        testReport += DateTime.Now.Area23DateTimeMilliseconds() + ": InitMethod() completed.\n";

        testReport += DateTime.Now.Area23DateTimeMilliseconds() + ": Persistence in " + PersistInCache.CacheType.ToString() + "\n";

        Dictionary<Guid, CContact> dictCacheTest = new Dictionary<Guid, CContact>();
        foreach (CContact c in _contacts)
        {
            if (c != null && c.Cuid != null && c.Cuid != Guid.Empty &&
                !dictCacheTest.Keys.Contains(c.Cuid))
                dictCacheTest.Add(c.Cuid, c);
        }
        testReport += DateTime.Now.Area23DateTimeMilliseconds() + ": Added " + dictCacheTest.Count + " count contacts to Dictionary<Guid, CqrContact>...\n";

        try
        {
            if (PersistInCache.CacheType == PersistType.RedisValkey)
            {
                testReport += DateTime.Now.Area23DateTimeMilliseconds() + ": Ready to connect to " +
                ConfigurationManager.AppSettings[Constants.VALKEY_CACHE_HOST_PORT_KEY] + "\n";
                string status = RedisValkeyCache.ValKeyInstance.Status;
                testReport += DateTime.Now.Area23DateTimeMilliseconds() + ": ConnectionMulitplexer.Status = " + status + Environment.NewLine;
            }

            testReport += DateTime.Now.Area23DateTimeMilliseconds() + ": Preparing to set Dictionary<Guid, CContact> in cache." + Environment.NewLine;
            MemoryCache.CacheDict.SetValue<Dictionary<Guid, CContact>>("TestCache", dictCacheTest);
            testReport += DateTime.Now.Area23DateTimeMilliseconds() + ": Added serialized json string to cache." + Environment.NewLine;

            Dictionary<Guid, CContact> outdict = (Dictionary<Guid, CContact>)MemoryCache.CacheDict.GetValue<Dictionary<Guid, CContact>>("TestCache");
            testReport += DateTime.Now.Area23DateTimeMilliseconds() + ": Got Dictionary<Guid, CContact> from cache with " +
                outdict.Keys.Count + " keys." + Environment.NewLine;
            foreach (CContact contact in outdict.Values)
            {
                testReport += DateTime.Now.Area23DateTimeMilliseconds() + ": Contact Cuid=" + contact.Cuid + " NameEmail=" +
                    contact.NameEmail + " Mobile=" + contact.Mobile + Environment.NewLine;
            }
            testReport += DateTime.Now.Area23DateTimeMilliseconds() + ": Preparing to delete key \"TestCache\":" + "\r\n";
            MemoryCache.CacheDict.RemoveKey("TestCache");
            testReport += DateTime.Now.Area23DateTimeMilliseconds() + ": Deleted key \"TestCache\"." + "\r\n";

            List<string> chatRooms = JsonChatRoom.GetJsonChatRoomsFromCache();
            testReport += DateTime.Now.Area23DateTimeMilliseconds() + ": Found " + chatRooms.Count + " chat room keys in cache." + Environment.NewLine;
            foreach (string room in chatRooms)
            {
                try
                {
                    Dictionary<long, string> dicTest = GetCachedMessageDict(room);
                    testReport += DateTime.Now.Area23DateTimeMilliseconds() + ": chat room " + room + " with keys " + dicTest.Keys.Count + ": messages." + Environment.NewLine;
                }
                catch (Exception exChatRoom)
                {
                    string exMsg = "loading chat room " + room + " failed. Exception: " + exChatRoom.Message + "." + Environment.NewLine;
                    testReport += DateTime.Now.Area23DateTimeMilliseconds() + ": " + exMsg;
                    Area23Log.LogStatic(exMsg, exChatRoom, "");
                }
            }
        }
        catch (Exception ex2)
        {
            testReport += DateTime.Now.Area23DateTimeMilliseconds() + ": Exception " +
                ex2.GetType() + ": " + ex2.Message + "\n\t" + ex2.ToString() + "\n";
        }
        

        return testReport;
    }


    public string TestService()
    {
        string ret = Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().GetName().FullName) +
                Assembly.GetExecutingAssembly().GetName().Version.ToString();

        object[] sattributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
        if (sattributes.Length > 0)
        {
            AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)sattributes[0];
            if (titleAttribute.Title != "")
                ret = titleAttribute.Title;
        }
        ret += "\n" + Assembly.GetExecutingAssembly().GetName().Version.ToString();

        object[] oattributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
        if (oattributes.Length > 0)
            ret += "\n" + ((AssemblyDescriptionAttribute)oattributes[0]).Description;

        object[] pattributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
        if (pattributes.Length > 0)
            ret += "\n" + ((AssemblyProductAttribute)pattributes[0]).Product;

        object[] crattributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
        if (crattributes.Length > 0)
            ret += "\n" + ((AssemblyCopyrightAttribute)crattributes[0]).Copyright;

        object[] cpattributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
        if (cpattributes.Length > 0)
            ret += "\n" + ((AssemblyCompanyAttribute)cpattributes[0]).Company;


        return ret;
    }

}
