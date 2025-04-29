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
using StackExchange.Redis;


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
    // protected internal ConnectionMultiplexer redis;
    // protected internal ConfigurationOptions options;
    protected internal static bool useAWSCache = false, useAppState = true;
    // protected internal string endpoint = "cqrcachecqrxseu-53g0xw.serverless.eus2.cache.amazonaws.com:6379";
    // protected internal StackExchange.Redis.IDatabase db;

    /// <summary>
    /// Persist encrypted messages in chat rooms in application state
    /// use this option only for testing, because you will you get soon an out of memory error
    /// </summary>
    public static bool PersistMsgInApplicationState { get { return (PersistInCache.CacheType == PersistType.ApplicationState); } }

    /// <summary>
    /// Use Amazon elastic cache to persist encrypted messages in chat rooms
    /// Fast option, but expensive, when we have a lot of huge size messages
    /// </summary>
    public static bool PersistMsgInAmazonElasticCache { get { return (PersistInCache.CacheType == PersistType.Redis); } }

    /// <summary>
    /// Use file system to encrypted messages in chat rooms
    /// Fast option, but expensive, when we have a lot of huge size messages
    /// </summary>
    public static bool PersistMsgInFileSystem { get { return (PersistInCache.CacheType == PersistType.JsonFile); } }


    public BaseService() 
    {
        //
        // TODO: Add constructor logic here
        //
        InitMethod();
    }

    public virtual void InitMethod()
    {
        _contacts = GetContacts();
        GetServerKey();
        cqrFacade = new CqrFacade(_serverKey);
        _decrypted = string.Empty;
        _responseString = string.Empty;
        _contact = null;


        if (PersistMsgInAmazonElasticCache)
        {
            string status = RedisCache.ConnMux.GetStatus();

            //config = new ElastiCacheClusterConfig("cachecqrxseu-53g0xw.serverless.eus2.cache.amazonaws.com", 11211);
            //// ClusterConfigSettings clusterConfig = new ClusterConfigSettings("cachecqrxseu-53g0xw.serverless.eus2.cache.amazonaws.com", 11211);
            //memClient = new MemcachedClient(config);
        }
    }

    protected string GetServerKey()
    {
        // _serverKey = Constants.AUTHOR_EMAIL;            

        if (ConfigurationManager.AppSettings["ExternalClientIP"] != null)
        {
            _serverKey = (string)ConfigurationManager.AppSettings["ExternalClientIP"];
        }
        else
            _serverKey = HttpContext.Current.Request.UserHostAddress;
        _serverKey += Constants.APP_NAME;

        return _serverKey;
    }

    internal HashSet<CContact> GetContacts()
    {
        _contacts = JsonContacts.GetContacts();
        return _contacts;
    }

    internal CContact AddContact(CContact cContact)
    {
        _contacts = JsonContacts.GetContacts();
        CContact foundCt = JsonContacts.FindContactByNameEmail(_contacts, cContact);
        if (foundCt != null)
        {
            foundCt.ContactId = cContact.ContactId;
            if (foundCt.Cuid == null || foundCt.Cuid == Guid.Empty)
                foundCt.Cuid = Guid.NewGuid();
            if (!string.IsNullOrEmpty(cContact.Address))
                foundCt.Address = cContact.Address;
            if (cContact.Mobile != null && cContact.Mobile.Length > 1)
                foundCt.Mobile = cContact.Mobile;
            //if (cContact.CRoom != null && !string.IsNullOrEmpty(cContact.CRoom.ChatRoomNr))
            //    foundCt.CRoom = new CChatRoom(cContact.CRoom);            

            //if (cqrContact != null && cqrContact.ContactImage != null &&
            //    !string.IsNullOrEmpty(cqrContact.ContactImage.ImageFileName) &&
            //    cqrContact.ContactImage.ImageBase64 != null &&
            //    !string.IsNullOrEmpty(cqrContact.ContactImage.ImageBase64))
            foundCt.ContactImage = null;
        }
        else
        {
            if (cContact.Cuid == null || cContact.Cuid == Guid.Empty)
                cContact.Cuid = Guid.NewGuid();
            _contacts.Add(cContact);
            foundCt = cContact;
            foundCt.ContactImage = null;

        }

        UpdateContact(foundCt);

        return foundCt;
    }

    internal void UpdateContact(CContact cContact)
    {
        CContact toAddContact = null;
        if (cContact == null || string.IsNullOrEmpty(cContact.Email))
            return;
        HashSet<CContact> contacts = new HashSet<CContact>();
        string chatRoomNr = (cContact._message != null && !string.IsNullOrEmpty(cContact._message)) ? cContact._message : _chatRoomNumber;

        foreach (CContact ct in _contacts)
        {
            if ((ct.Cuid == cContact.Cuid && ct.Email.Equals(cContact.Email, StringComparison.CurrentCultureIgnoreCase)) ||
                (ct.NameEmail.Equals(cContact.NameEmail, StringComparison.InvariantCultureIgnoreCase)))
            {
                toAddContact = new CContact(cContact, chatRoomNr, cContact.Hash);
                toAddContact.Mobile = cContact.Mobile;
                toAddContact.ContactImage = null;
                toAddContact.Cuid = (cContact.Cuid != null && cContact.Cuid != Guid.Empty) ? cContact.Cuid : Guid.NewGuid();
                contacts.Add(toAddContact);
            }
            else
                contacts.Add(ct);
        }
        _contacts = contacts;
        JsonContacts.SaveJsonContacts(_contacts);
    }

    internal void UpdateContacts(CContact contact, CSrvMsg<string> chatRoomMsg, string chatRoomNr)
    {
        bool foundCt = false;
        CContact toDelContact = null;
        if (contact == null || string.IsNullOrEmpty(contact.Email))
            return;

        if ((chatRoomMsg.Sender.Cuid == contact.Cuid && chatRoomMsg.Sender.Email.Equals(contact.Email, StringComparison.CurrentCultureIgnoreCase)) ||
            (chatRoomMsg.Sender.NameEmail.Equals(contact.NameEmail, StringComparison.CurrentCultureIgnoreCase)))
        {
            chatRoomMsg.Sender = new CContact(contact, chatRoomNr, contact.Hash);
        }
        for (int i = 0; i < chatRoomMsg.Recipients.Count; i++)
        {
            if ((chatRoomMsg.Recipients.ElementAt(i).Cuid == contact.Cuid) &&
                (chatRoomMsg.Recipients.ElementAt(i).Name.Equals(contact.Name) ||
                chatRoomMsg.Recipients.ElementAt(i).NameEmail.Equals(contact.NameEmail)))
            {
                toDelContact = chatRoomMsg.Recipients.ElementAt(i);
                foundCt = true;
                break;
            }
        }
        if (foundCt)
            if (chatRoomMsg.Recipients.Remove(toDelContact))
                chatRoomMsg.Recipients.Add(new CContact(contact, chatRoomNr, contact._hash));

        JsonChatRoom.SaveChatRoom(chatRoomMsg, chatRoomMsg.CRoom);        

        foundCt = false;
        GetContacts();
        for (int j = 0; j < _contacts.Count; j++)
        {
            if ((_contacts.ElementAt(j).Cuid == contact.Cuid) &&
                (_contacts.ElementAt(j).Name.Equals(contact.Name) ||
                    _contacts.ElementAt(j).NameEmail.Equals(contact.NameEmail)))
            {
                toDelContact = _contacts.ElementAt(j);
                foundCt = true;
                break;
            }
        }
        if (foundCt)
            if (_contacts.Remove(toDelContact))
                _contacts.Add(new CContact(contact, chatRoomNr, contact.Hash));

        JsonContacts.SaveJsonContacts(_contacts);

    }

    /// <summary>
    /// Generates a chat room with a new ChatRoomNr, containing sender and recpients
    /// </summary>
    /// <param name="CSrvMsg"><see cref="CSrvMsg{string}"/></param>
    /// <returns><see cref="CSrvMsg{string}"/></returns>
    internal CSrvMsg<string> InviteToChatRoom(CSrvMsg<string> cSrvMsg)
    {
        string ChatRoomNr = string.Empty;
        DateTime now = DateTime.Now; // now1 = now.AddMilliseconds(10);
        List<CContact> _invited = new List<CContact>();

        if (string.IsNullOrEmpty(ChatRoomNr))
            ChatRoomNr = String.Format("room_{0:MMddHH}_{1}.json", DateTime.Now,
                cSrvMsg.Sender.Email.Replace("@", "_").Replace(".", "_"));

        if (string.IsNullOrEmpty(ChatRoomNr))
            ChatRoomNr = String.Format("room_{0}_0.json", DateTime.Now.ToString("MMddHHmm"));

        Dictionary<long, string> dict = new Dictionary<long, string>();
        dict.Add(now.Ticks, "");

        cSrvMsg.CRoom = new CChatRoom(ChatRoomNr, Guid.NewGuid(), now, now);
        cSrvMsg.CRoom.TicksLong = dict.Keys.ToList();


        cSrvMsg.Sender._message = ChatRoomNr;
        
        bool addSender = true;
        foreach (CContact cr in cSrvMsg.Recipients)
        {
            cr._message = ChatRoomNr;
            _invited.Add(cr);
            
            if ((!string.IsNullOrEmpty(cr.NameEmail) && cr.NameEmail == cSrvMsg.Sender.NameEmail) ||
                (cr.Cuid != null && cr.Cuid != Guid.Empty && cr.Cuid == cSrvMsg.Sender.Cuid))
                addSender = false;
        }
        if (addSender)
            _invited.Add(cSrvMsg.Sender);

        SetCachedMessageDict(ChatRoomNr, dict);

        CSrvMsg<string> chatRSrvMsg = new CSrvMsg<string>();
        chatRSrvMsg.Sender = new CContact(cSrvMsg.Sender, cSrvMsg.CRoom.ChatRoomNr, cSrvMsg._hash);
        chatRSrvMsg.Recipients = new HashSet<CContact>(_invited);
        chatRSrvMsg._hash = cSrvMsg._hash;       
        chatRSrvMsg.MsgType = CType.Json;

        chatRSrvMsg = (new JsonChatRoom(ChatRoomNr)).SaveJsonChatRoom(chatRSrvMsg, cSrvMsg.CRoom);
        _chatRoomNumber = chatRSrvMsg.CRoom.ChatRoomNr;
        JsonChatRoom.AddJsonChatRoomToCache(_chatRoomNumber);

        // serialize chat room in msg later then saving
        chatRSrvMsg._message = _chatRoomNumber;
        chatRSrvMsg.SerializedMsg = chatRSrvMsg.ToJson();

        return chatRSrvMsg;
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
    public bool ChatRoomCheckPermission(CSrvMsg<string> cSrvMsg, CSrvMsg<string> chatRoomMsg, string chatRoomNr, bool isClosingRequest = false)
    {

        bool isValid = false;
        if (chatRoomNr.Equals(chatRoomMsg.CRoom.ChatRoomNr))
        {
            if ((cSrvMsg.Sender.NameEmail == chatRoomMsg.Sender.NameEmail) ||
                (!string.IsNullOrEmpty(cSrvMsg.Sender.Email) && cSrvMsg.Sender.Email == chatRoomMsg.Sender.Email))
            {
                _contact = chatRoomMsg.Sender;
                isValid = true;
                return isValid;
            }
            if (!isClosingRequest)
            {
                foreach (CContact c in chatRoomMsg.Recipients)
                {
                    if (cSrvMsg.Sender.NameEmail == c.NameEmail ||
                        cSrvMsg.Sender.Email == c.Email)
                    {
                        _contact = c;
                        isValid = true;
                        return isValid;
                    }
                }
            }
        }

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
    [Obsolete("No more chatroom in contact",true)]
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