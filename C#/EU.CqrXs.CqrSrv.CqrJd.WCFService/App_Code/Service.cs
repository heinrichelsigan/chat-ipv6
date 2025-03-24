using Area23.At.Framework.Library.CqrXs;
using Area23.At.Framework.Library.CqrXs.CqrMsg;
using Area23.At.Framework.Library.CqrXs.CqrSrv;
using Area23.At.Framework.Library.Crypt.EnDeCoding;
using Area23.At.Framework.Library.Static;
using Area23.At.Framework.Library.Util;
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

        if (PersistMsgInApplicationState)
            HttpContext.Current.Application["lastmsg"] = cryptMsg;
        if (PersistMsgInAmazonElasticCache)
            RedIs.Db.StringSet("lastmsg", cryptMsg);

        SrvMsg1 srv1stMsg = new SrvMsg1(_serverKey);
        SrvMsg1 srv1stRespMsg = new SrvMsg1(_serverKey);

        try
        {
            if (!string.IsNullOrEmpty(cryptMsg) && cryptMsg.Length >= 8)
            {
                _contact = srv1stMsg.NCqrSrvMsg1(cryptMsg);
                _decrypted = _contact.ToJson();
                Area23Log.LogStatic("Contact decrypted successfully: " + _decrypted + "\n");
            }
        }
        catch (Exception ex)
        {
            CqrException.SetLastException(ex);
            Area23Log.LogStatic("Exception " + ex.GetType() + " when decrypting contact: " + ex.Message + "\n\t" + ex.ToString() + "\n");
        }

        _responseString = srv1stRespMsg.CqrBaseMsg("", EncodingType.Base64);

        if (!string.IsNullOrEmpty(_decrypted) && _contact != null && !string.IsNullOrEmpty(_contact.NameEmail))
        {
            if (PersistMsgInApplicationState)
                HttpContext.Current.Application["lastdecrypted"] = _decrypted;
            if (PersistMsgInAmazonElasticCache)
                RedIs.Db.StringSet("lastdecrypted", _decrypted);

            CqrContact foundCt = AddContact(_contact);
            _responseString = srv1stRespMsg.CqrSrvMsg1(foundCt, EncodingType.Base64);
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
        SrvMsg srvMsg = new SrvMsg(_serverKey, _serverKey);
        FullSrvMsg<string> fullSrvMsg, chatRSrvMsg;

        _responseString = srvMsg.CqrBaseMsg(Constants.NACK);

        try
        {
            if (!string.IsNullOrEmpty(cryptMsg) && cryptMsg.Length >= 8)
            {
                fullSrvMsg = srvMsg.NCqrSrvMsg<string>(cryptMsg);           // decrypt FullSrvMsg<string>            
                _contact = AddContact(fullSrvMsg.Sender);                   // add contact from FullSrvMsg<string>   
                chatRSrvMsg = InviteToChatRoom(fullSrvMsg);                 // generate a FullSrvMsg<string> chatserver message by inviting                           

                _responseString = srvMsg.CqrSrvMsg<string>(chatRSrvMsg);
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
        SrvMsg srvMsg = new SrvMsg(_serverKey, _serverKey);
        FullSrvMsg<string> fullSrvMsg;

        _responseString = srvMsg.CqrBaseMsg(Constants.NACK);

        try
        {
            if (!string.IsNullOrEmpty(cryptMsg) && cryptMsg.Length >= 8)
            {
                fullSrvMsg = srvMsg.NCqrSrvMsg<string>(cryptMsg);           // decrypt FullSrvMsg<string>
                _contact = fullSrvMsg.Sender;
                _chatRoomNumber = (!string.IsNullOrEmpty(fullSrvMsg.ChatRoomNr)) ? fullSrvMsg.ChatRoomNr : fullSrvMsg.Sender.ChatRoomNr;

                FullSrvMsg<string> chatRoomMsg = JsonChatRoom.LoadChatRoom(fullSrvMsg, _chatRoomNumber);
                isValid = ChatRoomCheckPermission(fullSrvMsg, chatRoomMsg, _chatRoomNumber);
                chatRoomMsg.TContent = string.Empty;

                if (isValid)
                {
                    dict = GetCachedMessageDict(_chatRoomNumber);

                    List<long> pollKeys = GetNewMessageIndices(dict.Keys.ToList(), fullSrvMsg.Sender);

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
                        chatRoomMsg = (new JsonChatRoom(_chatRoomNumber)).SaveJsonChatRoom(chatRoomMsg, _chatRoomNumber);
                    }

                }

                _responseString = srvMsg.CqrSrvMsg<string>(chatRoomMsg);

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
        SrvMsg srvMsg = new SrvMsg(_serverKey, _serverKey);
        FullSrvMsg<string> fullSrvMsg;

        _responseString = ""; // set empty response string per default
        FullSrvMsg<string> chatRoomMsg = new FullSrvMsg<string>(); // construct an empty message

        try
        {
            if (!string.IsNullOrEmpty(cryptMsg) && cryptMsg.Length >= 8)
            {
                fullSrvMsg = srvMsg.NCqrSrvMsg<string>(cryptMsg);
                _contact = fullSrvMsg.Sender;
                _chatRoomNumber = (!string.IsNullOrEmpty(fullSrvMsg.ChatRoomNr)) ? fullSrvMsg.ChatRoomNr : fullSrvMsg.Sender.ChatRoomNr;

                chatRoomMsg = (new JsonChatRoom(_chatRoomNumber)).LoadJsonChatRoom(fullSrvMsg, _chatRoomNumber);
                chatRoomMsg.TContent = ""; // set string empty, if no message

                isValid = ChatRoomCheckPermission(fullSrvMsg, chatRoomMsg, _chatRoomNumber);
                if (isValid)
                {
                    DateTime now = DateTime.Now;

                    dict = GetCachedMessageDict(_chatRoomNumber);

                    dict.Add(now.Ticks, chatRoomMembersCrypted);
                    chatRoomMsg.TicksLong.Add(now.Ticks);
                    // chatRoomMsg.Sender.TicksLong.Add(now.Ticks);

                    SetCachedMessageDict(_chatRoomNumber, dict);

                    _contact = AddPollDate(_contact, now, true);
                    chatRoomMsg.Sender = AddPollDate(chatRoomMsg.Sender, now, true);

                    UpdateContact(_contact);
                    chatRoomMsg = (new JsonChatRoom(_chatRoomNumber)).SaveJsonChatRoom(chatRoomMsg, _chatRoomNumber);
                    chatRoomMsg.Sender.LastPushed = now;

                }

                _responseString = srvMsg.CqrSrvMsg<string>(chatRoomMsg);

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
        SrvMsg srvMsg = new SrvMsg(_serverKey, _serverKey);
        FullSrvMsg<string> fullSrvMsg;
        List<CqrContact> _invited = new List<CqrContact>();

        _responseString = srvMsg.CqrBaseMsg(Constants.NACK);

        try
        {
            if (!string.IsNullOrEmpty(cryptMsg) && cryptMsg.Length >= 8)
            {
                fullSrvMsg = srvMsg.NCqrSrvMsg<string>(cryptMsg);
                _contact = AddContact(fullSrvMsg.Sender);
                _chatRoomNumber = (!string.IsNullOrEmpty(fullSrvMsg.ChatRoomNr)) ? fullSrvMsg.ChatRoomNr : fullSrvMsg.Sender.ChatRoomNr;
                JsonChatRoom jsonChatRoom = new JsonChatRoom(_chatRoomNumber);
                FullSrvMsg<string> chatRoomMsg = JsonChatRoom.LoadChatRoom(fullSrvMsg, _chatRoomNumber);
                isValid = ChatRoomCheckPermission(fullSrvMsg, chatRoomMsg, _chatRoomNumber);
                if (isValid)
                {
                    jsonChatRoom.DeleteJsonChatRoom(_chatRoomNumber);
                }

                _responseString = srvMsg.CqrSrvMsg<string>(chatRoomMsg);

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


    public string TestCache()
    {
        string testReport = DateTime.Now.Area23DateTimeMilliseconds() + ":TestCache() started.\n";
        try
        {
            InitMethod();
        }
        catch (Exception ex1)
        {
            testReport += DateTime.Now.Area23DateTimeMilliseconds() + ": Exception " +
                ex1.GetType() + ": " +  ex1.Message + "\n\t" + ex1 + "\n";
        }

        testReport += DateTime.Now.Area23DateTimeMilliseconds() + ": InitMethod() completed.\n";

        Dictionary<Guid, CqrContact> dictCacheTest = new Dictionary<Guid, CqrContact>();
        foreach (CqrContact c in _contacts)
        {
            if (c != null && c.Cuid != null && c.Cuid != Guid.Empty &&
                !dictCacheTest.Keys.Contains(c.Cuid))
                dictCacheTest.Add(c.Cuid, c);
        }
        testReport += DateTime.Now.Area23DateTimeMilliseconds() + ": Added {dictCacheTest.Count} count contacts to Dictionary<Guid, CqrContact>...\n";
        if (PersistMsgInAmazonElasticCache)
        {
            try
            {
                if (PersistMsgInAmazonElasticCache)
                {
                    testReport += DateTime.Now.Area23DateTimeMilliseconds() + ": Ready to connect to {ConfigurationManager.AppSettings[Constants.VALKEY_CACHE_HOST_PORT]}\n";
                    string status = RedIs.ConnMux.GetStatus();
                    testReport += DateTime.Now.Area23DateTimeMilliseconds() + ": ConnectionMulitplexer.Status = {status}" + Environment.NewLine;

                    string dictJson = JsonConvert.SerializeObject(dictCacheTest);
                    testReport += DateTime.Now.Area23DateTimeMilliseconds() + ": Serialized Dictionary<Guid, CqrContact> to json string." + Environment.NewLine;
                    RedIs.Db.StringSet("TestCache", dictJson);
                    testReport += DateTime.Now.Area23DateTimeMilliseconds() + ": Added serialized json string to cache." + Environment.NewLine;

                    string jsonOut = RedIs.Db.StringGet("TestCache");
                    testReport += DateTime.Now.Area23DateTimeMilliseconds() + ": Got json serialized string from cache: {jsonOut}." + Environment.NewLine;
                    Dictionary<Guid, CqrContact> outdict = (Dictionary<Guid, CqrContact>)JsonConvert.DeserializeObject<Dictionary<Guid, CqrContact>>(jsonOut);
                    testReport += DateTime.Now.Area23DateTimeMilliseconds() + ": Deserialized json sring to (Dictionary<Guid, CqrContact> with {outdict.Keys.Count} keys.";

                    List<string> chatRooms = JsonChatRoom.GetJsonChatRoomsFromCache();
                    testReport += DateTime.Now.Area23DateTimeMilliseconds() + ":Found {chatRooms.Count} chat room keys in cache." + Environment.NewLine;
                    foreach (string room in chatRooms)
                    {
                        Dictionary<long, string> dicTest = GetCachedMessageDict(room);
                        testReport += DateTime.Now.Area23DateTimeMilliseconds() + ": chat room {room} with keys {dicTest.Keys.Count} messages." + Environment.NewLine;
                    }
                }
            }
            catch (Exception ex2)
            {
                testReport += DateTime.Now.Area23DateTimeMilliseconds() + ": Exception " + 
                    ex2.GetType() + ": " + ex2.Message + "\n\t" + ex2 + "\n";
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
