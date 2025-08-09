using Area23.At.Framework.Library.Cache;
using Area23.At.Framework.Library.Cqr;
using Area23.At.Framework.Library.Cqr.Msg;
using Area23.At.Framework.Library.Net.IpSocket;
using Area23.At.Framework.Library.Static;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;


/// <summary>
/// Summary description for BaseService
/// </summary>
public class BaseService
{
    protected internal static HashSet<CContact> _contacts;
    protected internal CqrFacade cqrFacade;
    protected internal CContact _contact;
    // protected internal string _literalServerIPv4, _literalServerIPv6;
    protected internal string _serverKey = string.Empty;
    protected internal string _decrypted = string.Empty, _encrypted = string.Empty;
    protected internal string _responseString = string.Empty;
    protected internal string _chatRoomNumber = string.Empty;
    protected internal static bool useAWSCache = false, useAppState = true;
    // protected internal string endpoint = "cqrcachecqrxseu-53g0xw.serverless.eus2.cache.amazonaws.com:6379";   
   
    public BaseService() 
    {
        //
        // TODO: Add constructor logic here
        //
        InitMethod();
    }

    public virtual void InitMethod()
    {
        _contacts = JsonContacts.GetContacts();
        GetServerKey();
        cqrFacade = new CqrFacade(_serverKey);
        _decrypted = string.Empty;
        _responseString = string.Empty;
        _contact = null;


        // if (PersistMsgInAmazonElasticCache)
        // {
            // string status = RedisCache.ConnMux.GetStatus();

            //config = new ElastiCacheClusterConfig("cachecqrxseu-53g0xw.serverless.eus2.cache.amazonaws.com", 11211);
            //// ClusterConfigSettings clusterConfig = new ClusterConfigSettings("cachecqrxseu-53g0xw.serverless.eus2.cache.amazonaws.com", 11211);
            //memClient = new MemcachedClient(config);
        // }
    }

    protected string GetServerKey()
    {
        // _serverKey = Constants.AUTHOR_EMAIL;            

        if (ConfigurationManager.AppSettings[Constants.EXTERNAL_CLIENT_IP] != null)
        {
            _serverKey = (string)ConfigurationManager.AppSettings[Constants.EXTERNAL_CLIENT_IP];
        }
        else
            _serverKey = HttpContext.Current.Request.UserHostAddress;
        _serverKey += Constants.APP_NAME;

        return _serverKey;
    }

    /// <summary>
    /// Generates a chat room with a new ChatRoomNr, containing sender and recpients
    /// </summary>
    /// <param name="cSrvMsg"><see cref="CSrvMsg{string}"/></param>
    /// <returns><see cref="CSrvMsg{string}"/></returns>
    internal CSrvMsg<string> InviteToChatRoom(CSrvMsg<string> cSrvMsg)
    {
        string chatRoomNr = string.Empty;
        DateTime now = DateTime.Now; // now1 = now.AddMilliseconds(10);
        List<CContact> _invited = new List<CContact>();

        string restMail = cSrvMsg.Sender.Email.Contains("@") ? (cSrvMsg.Sender.Email.Substring(0, cSrvMsg.Sender.Email.IndexOf("@"))) : cSrvMsg.Sender.Email.Trim();
        restMail = restMail.Replace("@", "_").Replace(".", "_");

        if (!string.IsNullOrEmpty(restMail))
            chatRoomNr = String.Format("room_{0:MMdd}_{1}.json", DateTime.Now, restMail);
        else
            chatRoomNr = String.Format("room_{0:MMddHHmm}.json", DateTime.Now);

        Dictionary<long, string> dict = new Dictionary<long, string>();
        dict.Add(now.Ticks, "");

        if (cSrvMsg.CRoom == null)
            cSrvMsg.CRoom = new CChatRoom(chatRoomNr, Guid.NewGuid(), now, now) { TicksLong = dict.Keys.ToList() };
        else
        {
            cSrvMsg.CRoom.ChatRoomNr = chatRoomNr;
            cSrvMsg.CRoom.ChatRuid = (cSrvMsg.CRoom.ChatRuid == Guid.Empty) ? Guid.NewGuid() : cSrvMsg.CRoom.ChatRuid;
            cSrvMsg.CRoom.LastPolled = now;
            cSrvMsg.CRoom.LastPushed = now;
            if (cSrvMsg.CRoom.TicksLong == null || cSrvMsg.CRoom.TicksLong.Count == 0)
                cSrvMsg.CRoom.TicksLong = dict.Keys.ToList();
            else
                cSrvMsg.CRoom.TicksLong.Add(now.Ticks);
        }

        cSrvMsg._message = (cSrvMsg.CRoom != null && !string.IsNullOrEmpty(cSrvMsg.CRoom.ChatRoomNr)) ? cSrvMsg.CRoom.ChatRoomNr : "";


        bool addSender = true;
        foreach (CContact cr in cSrvMsg.Recipients)
        {
            cr._message = (cSrvMsg.CRoom != null && !string.IsNullOrEmpty(cSrvMsg.CRoom.ChatRoomNr)) ? cSrvMsg.CRoom.ChatRoomNr : "";

            _invited.Add(cr);
            if ((!string.IsNullOrEmpty(cr.NameEmail) && cr.NameEmail == cSrvMsg.Sender.NameEmail) ||
                (cr.Cuid != null && cr.Cuid != Guid.Empty && cr.Cuid == cSrvMsg.Sender.Cuid))
                addSender = false;
        }
        if (addSender)
            _invited.Add(cSrvMsg.Sender);

        SetCachedMessageDict(chatRoomNr, dict);


        CSrvMsg<string> cChatRSrvMsg = JsonChatRoom.SaveChatRoom(cSrvMsg, cSrvMsg.CRoom);
        _chatRoomNumber = cChatRSrvMsg.CRoom.ChatRoomNr;
        cChatRSrvMsg._message = _chatRoomNumber;
        JsonChatRoom.AddJsonChatRoomToCache(_chatRoomNumber);

        // serialize chat room in msg later then saving
        cChatRSrvMsg.SerializedMsg = cChatRSrvMsg.ToJson();

        return cChatRSrvMsg;
    }


    /// <summary>
    /// ChatRoomCheckPermission
    /// Validates, if a user has permission to poll or to push messages in the chat room by the following steps:
    /// 1. chat room number in encrypted, now decrypted msg from webservice must be ident with chat room number from json file
    /// 2. sender email from  encrypted, now decrypted msg from webservice must much
    /// 2.a. either creator invitor of chat room
    /// 2.b. on of the invited persons in invitation
    /// </summary>
    /// <param name="CSrvMsg"><see cref="CSrvMsg{string}"/> decoded from <see cref="CqrService.CqrService"/> Webservice</param>
    /// <param name="chatRoomMsg"><see cref="CSrvMsg{string}"/> generated from chat room json</param>
    /// <param name="ChatRoomNr"><see cref="string"/> chat room number of chat room</param>
    /// <param name="isClosingRequest">default false, only on close, where only creator can close chat room</param>
    /// <returns>true, if person is allowed to push or receive msg from / to chat room</returns>        
    [Obsolete("CheckPermission moved directly to static class JsonChatRoom", true)]
    public bool ChatRoomCheckPermission(CSrvMsg<string> cSrvMsg, CSrvMsg<string> chatRoomMsg, string chatRoomNr, bool isClosingRequest = false)
    {
        bool isValid = false;
        cSrvMsg = JsonChatRoom.CheckPermission(cSrvMsg, chatRoomMsg, chatRoomNr, out isValid, isClosingRequest);
        
        return isValid;
    }

    /// <summary>
    /// Add LastPolled to contact and also to <see cref="CqrContact.PolledMsgDates"/>
    /// reduces <see cref="CqrContact.PolledMsgDates"/>, if contact wears a to huge amount of polling history
    /// TODO: implement ring buffer here
    /// </summary>
    /// <param name="contact"><see cref="CContact"/> to modify</param>
    /// <param name="date"></param>
    /// <returns>modified <see cref="CContact"/></returns>
    [Obsolete("No more chatroom in contact", true)]
    public CContact AddPollDate(CContact contact, DateTime date, bool pushed = false)
    {
        if (pushed)
            contact.Md5Hash = date.ToString();
            // contact.CRoom.LastPushed = date;
        // else
           // contact.CRoom.LastPolled = date;

        return contact;
    }

    /// <summary>
    /// AddLastDate adds lastPolled or lastPushed date and tickIndex to TicksLong
    /// </summary>
    /// <param name="chatRoomMsg"><see cref="CSrvMsg{string}"/> chat room msg to be returned to chat client app</param>
    /// <param name="tickIndex">tick long index</param>
    /// <param name="pushed">false for poolled, true for pushed</param>
    /// <returns><see cref="CSrvMsg{string}"/></returns>
    public CSrvMsg<string> AddLastDate(CSrvMsg<string> chatRoomMsg, long tickIndex, bool pushed = false)
    {
        DateTime date = new DateTime(tickIndex);
        if (pushed)
        {
            chatRoomMsg.CRoom.LastPushed = date;
        }
        else
        {
            chatRoomMsg.CRoom.LastPolled = date;
            if (!chatRoomMsg.CRoom.TicksLong.Contains(tickIndex))
                chatRoomMsg.CRoom.TicksLong.Add(tickIndex);
        }

        return chatRoomMsg;
    }

    /// <summary>
    /// GetCachedMessageDict returns one chat room message dictionary
    /// either from Application State in proc or from Valkey Elastic Cache on AWS
    /// </summary>
    /// <param name="chatRoomNumber">json chat room number</param>
    /// <returns>one chat room message dictionary</returns>
    public static Dictionary<long, string> GetCachedMessageDict(string chatRoomNumber)
    {
        Dictionary<long, string> dict = new Dictionary<long, string>();
        
        dict = (Dictionary<long, string>)MemoryCache.CacheDict.GetValue<Dictionary<long, string>>(chatRoomNumber);

        return dict;
    }

    /// <summary>
    /// GetNewMessageIndices get all chat room indices, 
    /// which are newer than last <see cref="CContact.LastPolled">polling date of user</see>
    /// or user hasn't read and that are not in list <see cref="CqrContact.TicksLong"></see>
    /// </summary>
    /// <param name="dictKeys"><see cref="DateTime.Ticks"/> as index key of chat room message dictionary</param>
    /// <param name="cSrvMsg"><see cref=CSrvMsg{string}"/></param>
    /// <returns><see cref="List{long}">key indices of messages, that are new and not already polled</see></returns>
    public static List<long> GetNewMessageIndices(List<long> dictKeys, CSrvMsg<string> cSrvMsg)
    {

        List<long> pollKeys = new List<long>();
        foreach (long tickIndex in dictKeys)
        {
            // if (tickIndex > sender.LastPolled.Ticks)
            if (!cSrvMsg.CRoom.TicksLong.Contains(tickIndex))                
                pollKeys.Add(tickIndex);
        }

        return pollKeys;
    }

    /// <summary>
    /// SetCachedMessageDict saves the mesage dictionary for chat room in 
    /// either application state in proc or Amazon Valkey Elastic cache
    /// </summary>
    /// <param name="chatRoomNumber">json chat room number</param>
    /// <param name="dict">the mesage dictionary for chat room </param>
    public static void SetCachedMessageDict(string chatRoomNumber, Dictionary<long, string> dict)
    {

        MemoryCache.CacheDict.SetValue<Dictionary<long, string>>(chatRoomNumber, dict);
        return;
    }

}