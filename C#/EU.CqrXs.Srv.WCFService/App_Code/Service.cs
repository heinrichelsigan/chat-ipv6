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

            CContact foundCt = AddContact(_contact);
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
                _contact = AddContact(cSrvMsg.Sender);                      // add contact from FullSrvMsg<string>   
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
                isValid = ChatRoomCheckPermission(cSrvMsg, chatRoomMsg, _chatRoomNumber);
                chatRoomMsg.TContent = string.Empty;

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

                        UpdateContact(chatRoomMsg.Sender);
                        chatRoomMsg = (new JsonChatRoom(_chatRoomNumber)).SaveJsonChatRoom(chatRoomMsg, chatRoomMsg.CRoom);
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
    /// <param name="cryptMsg">encrypted <see cref="FullSrvMsg<string>"/> with chat room number and last polled dates</param>
    /// <param name="chatRoomMembersCrypted">with client key encrypted message, that is stored in proc of server, but server can't decrypt</param>
    /// <returns>encrypted <see cref="FullSrvMsg<string>"/> with chat room number and last polled date changed to now</returns>
    public string ChatRoomPushMessage(string cryptMsg, string chatRoomMembersCrypted)
    {
        Area23Log.LogStatic("ChatRoomPushMessage(string cryptMsg, string chatRoomMembersCrypted) called. len = " + chatRoomMembersCrypted.Length + ".\n");
        InitMethod();
        bool isValid = false;
        Dictionary<long, string> dict;

        CSrvMsg<string> cSrvMsg, aSrvMsg = new CSrvMsg<string>() { _hash = cqrFacade.PipeString };

        _responseString = ""; // set empty response string per default
        CSrvMsg<string> chatRoomMsg = new CSrvMsg<string>(); // construct an empty message

        try
        {
            if (!string.IsNullOrEmpty(cryptMsg) && cryptMsg.Length >= 8)
            {
                cSrvMsg = aSrvMsg.DecryptFromJson(_serverKey, cryptMsg);
                _contact = cSrvMsg.Sender;
                _chatRoomNumber = (cSrvMsg.CRoom != null && !string.IsNullOrEmpty(cSrvMsg.CRoom.ChatRoomNr)) ? cSrvMsg.CRoom.ChatRoomNr : "";

                chatRoomMsg = JsonChatRoom.LoadChatRoom(cSrvMsg, _chatRoomNumber);
                chatRoomMsg.TContent = ""; // set string empty, if no message

                isValid = ChatRoomCheckPermission(cSrvMsg, chatRoomMsg, _chatRoomNumber);
                if (isValid)
                {
                    DateTime now = DateTime.Now;

                    dict = GetCachedMessageDict(_chatRoomNumber);

                    dict.Add(now.Ticks, chatRoomMembersCrypted);
                    chatRoomMsg.CRoom.TicksLong.Add(now.Ticks);
                    chatRoomMsg.CRoom.LastPolled = now;

                    SetCachedMessageDict(_chatRoomNumber, dict);                    

                    UpdateContact(_contact);
                    chatRoomMsg = (new JsonChatRoom(_chatRoomNumber)).SaveJsonChatRoom(chatRoomMsg, chatRoomMsg.CRoom);
                    chatRoomMsg.Sender._message = _chatRoomNumber;

                }

                _responseString = chatRoomMsg.EncryptToJson(_serverKey);

            }
        }
        catch (Exception ex)
        {
            CqrException.SetLastException(ex);
            Area23Log.LogStatic(ex);
        }

        Area23Log.LogStatic("ChatRoomPushMessage(string cryptMsg, string chatRoomMembersCrypted) finished. ChatRoomNr =  " + _chatRoomNumber + ".\n");
        return _responseString;
    }

    /// <summary>
    /// ChatRoomClose
    /// </summary>
    /// <param name="cryptMsg"></param>
    /// <returns></returns>
    public string ChatRoomClose(string cryptMsg)
    {
        Area23Log.LogStatic("ChatRoomClose(string cryptMsg) started. cryptMsg.Length =  " + cryptMsg.Length + ".\n");
        InitMethod();
        bool isValid = false;
        
        CSrvMsg<string> cSrvMsg, aSrvMsg = new CSrvMsg<string>() { _hash = cqrFacade.PipeString };
        List<CContact> _invited = new List<CContact>();

        _responseString = "";

        try
        {
            if (!string.IsNullOrEmpty(cryptMsg) && cryptMsg.Length >= 8)
            {
                cSrvMsg = aSrvMsg.DecryptFromJson(_serverKey, cryptMsg);
                _contact = AddContact(cSrvMsg.Sender);
                _chatRoomNumber = (cSrvMsg.CRoom != null && !string.IsNullOrEmpty(cSrvMsg.CRoom.ChatRoomNr)) ? cSrvMsg.CRoom.ChatRoomNr : "";
                JsonChatRoom jsonChatRoom = new JsonChatRoom(_chatRoomNumber);
                CSrvMsg<string> chatRoomMsg = JsonChatRoom.LoadChatRoom(cSrvMsg, _chatRoomNumber);
                isValid = ChatRoomCheckPermission(cSrvMsg, chatRoomMsg, _chatRoomNumber);
                if (isValid)
                {
                    jsonChatRoom.DeleteJsonChatRoom(_chatRoomNumber);
                }

                _responseString = chatRoomMsg.EncryptToJson(_serverKey);

            }
        }
        catch (Exception ex)
        {
            CqrException.SetLastException(ex);
            Area23Log.LogStatic(ex);
        }

        Area23Log.LogStatic("ChatRoomClose(string cryptMsg) finished. deleted chat room ChatRoomNr = " + _chatRoomNumber + ".\n");

        return _responseString;

    }

    public string GetData(int value)
	{
		return string.Format("You entered: {0}", value);
	}

	public CompositeType GetDataUsingDataContract(CompositeType composite)
	{
		if (composite == null)
		{
			throw new ArgumentNullException("composite");
		}
		if (composite.BoolValue)
		{
			composite.StringValue += "Suffix";
		}
		return composite;
	}

    public string GetIPAddress()
    {
        string userHostAddr = HttpContext.Current.Request.UserHostAddress;
        return userHostAddr;
    }



    [WebMethod]
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
        if (PersistMsgInAmazonElasticCache)
        {
            try
            {
                testReport += DateTime.Now.Area23DateTimeMilliseconds() + ": Ready to connect to " + 
                    ConfigurationManager.AppSettings[Constants.VALKEY_CACHE_HOST_PORT_KEY] + "\n";
                string status = RedisCache.ConnMux.GetStatus();
                testReport += DateTime.Now.Area23DateTimeMilliseconds() + ": ConnectionMulitplexer.Status = " + status + Environment.NewLine;

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

                List<string> chatRooms = JsonChatRoom.GetJsonChatRoomsFromCache();
                testReport += DateTime.Now.Area23DateTimeMilliseconds() + ": Found " + chatRooms.Count + " chat room keys in cache." + Environment.NewLine;
                foreach (string room in chatRooms)
                {
                    try
                    {
                        Dictionary<long, string> dicTest = GetCachedMessageDict(room);
                        testReport += DateTime.Now.Area23DateTimeMilliseconds() +": chat room " + room + " with keys " + dicTest.Keys.Count + ": messages." + Environment.NewLine;
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
