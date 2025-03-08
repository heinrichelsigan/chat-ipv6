using StackExchange.Redis;
using Area23.At.Framework.Library;
using Area23.At.Framework.Library.CqrXs;
using Area23.At.Framework.Library.CqrXs.CqrMsg;
using Area23.At.Framework.Library.CqrXs.CqrSrv;
using Area23.At.Framework.Library.Crypt.EnDeCoding;
using Area23.At.Framework.Library.Net.IpSocket;
using Area23.At.Framework.Library.Static;
using Area23.At.Framework.Library.Util;
using EU.CqrXs.CqrSrv.CqrJd.Util;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Web;
using System.Web.Services;
using System.Net;
using Newtonsoft.Json;
using System.Net.NetworkInformation;


namespace EU.CqrXs.CqrSrv.CqrJd
{

    /// <summary>
    /// CqrService offers a simple chat room service with strong encryption
    /// </summary>
    [WebService(Namespace = "https://cqrjd.eu/cqrsrv/cqrjd/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class CqrService : Util.BaseWebService
    {

        /// <summary>
        /// Send1StSrvMsg sends first registration message of contact
        /// </summary>
        /// <param name="cryptMsg">with sercerkey encrypted message</param>
        /// <returns>with serverkey encrypted responnse of own contact</returns>
        [WebMethod]
        public string Send1StSrvMsg(string cryptMsg)
        {            
            InitMethod();
            HttpContext.Current.Application["lastmsg"] = cryptMsg;
            
            SrvMsg1 srv1stMsg = new SrvMsg1(_serverKey);
            SrvMsg1 srv1stRespMsg = new SrvMsg1(_serverKey);
            
            try
            {
                if (!string.IsNullOrEmpty(cryptMsg) && cryptMsg.Length >= 8)
                {
                    _contact = srv1stMsg.NCqrSrvMsg1(cryptMsg);
                    _decrypted = _contact.ToJson();
                    Area23Log.LogStatic("$Contact decrypted successfully: {_decrypted}\n");
                }
            }
            catch (Exception ex)
            {
                CqrException.SetLastException(ex);
                Area23Log.LogStatic($"Exception {ex.GetType()} when decrypting contact: {ex.Message}\n\t{ex.ToString()}\n");
            }

            _responseString = srv1stRespMsg.CqrBaseMsg("", EncodingType.Base64);

            if (!string.IsNullOrEmpty(_decrypted) && _contact != null && !string.IsNullOrEmpty(_contact.NameEmail))
            {
                Application["lastdecrypted"] = _decrypted;
                CqrContact foundCt = AddContact(_contact);
                _responseString = srv1stRespMsg.CqrSrvMsg1(foundCt, EncodingType.Base64);
            }

            return _responseString;
        }


        /// <summary>
        /// Invites to a chat romm 
        /// with an encrypted <see cref="FullSrvMsg<string>"/>
        /// </summary>
        /// <param name="cryptMsg">encrypted <see cref="FullSrvMsg<string>"/></param>
        /// <returns>encrypted <see cref="FullSrvMsg<string>"/> including chatroom number</returns>
        [WebMethod]        
            
        public string ChatRoomInvite(string cryptMsg)
        {
            InitMethod();

            SrvMsg srvMsg = new SrvMsg(_serverKey, _serverKey);
            FullSrvMsg<string> fullSrvMsg, chatRSrvMsg;

            _responseString = srvMsg.CqrBaseMsg(Constants.NACK);

            try
            {
                if (!string.IsNullOrEmpty(cryptMsg) && cryptMsg.Length >= 8)
                {
                    fullSrvMsg = srvMsg.NCqrSrvMsg<string>(cryptMsg);                  
                    _contact = AddContact(fullSrvMsg.Sender);
                    chatRSrvMsg = InviteToChatRoom(fullSrvMsg);

                    string chatRoomId = chatRSrvMsg.ChatRoomNr;
                    DateTime now = DateTime.Now;
                    Dictionary<DateTime, string> dict = new Dictionary<DateTime, string>();
                    dict.Add(now, "");

                    if (UseAmazonElasticCache)
                    {
                        string dictJson = JsonConvert.SerializeObject(dict);
                        RedIs.Db.StringSet(chatRoomId, dictJson);
                    }
                        
                    
                    if (UseApplicationState)
                        HttpContext.Current.Application[chatRoomId] = dict;

                    chatRSrvMsg.Sender.LastPolled = now;
                    chatRSrvMsg.Sender.PolledMsgDates.Add(now);
                    _contact.LastPolled = now;
                    _contact.PolledMsgDates.Add(now);
                    UpdateContacts(_contact, chatRSrvMsg, chatRoomId);

                    _responseString = srvMsg.CqrSrvMsg<string>(chatRSrvMsg);
                }
            }
            catch (Exception ex)
            {
                CqrException.SetLastException(ex);
                Area23Log.LogStatic(ex);
            }


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
        [WebMethod]
        public string ChatRoomPoll(string cryptMsg)
        {
            InitMethod();

            Dictionary<DateTime, string> dict = new Dictionary<DateTime, string>();
            bool isValid = false;
            SrvMsg srvMsg = new SrvMsg(_serverKey, _serverKey);
            FullSrvMsg<string> fullSrvMsg;

            _responseString = srvMsg.CqrBaseMsg(Constants.NACK);

            try
            {
                if (!string.IsNullOrEmpty(cryptMsg) && cryptMsg.Length >= 8)
                {
                    fullSrvMsg = srvMsg.NCqrSrvMsg<string>(cryptMsg);
                    _contact = fullSrvMsg.Sender;
                    _chatRoomNumber = (!string.IsNullOrEmpty(fullSrvMsg.ChatRoomNr)) ? fullSrvMsg.ChatRoomNr : fullSrvMsg.Sender.ChatRoomId;

                    FullSrvMsg<string> chatRoomMsg = (new JsonChatRoom(_chatRoomNumber)).LoadJsonChatRoom(fullSrvMsg, _chatRoomNumber);
                    isValid = ValidateChatRoomNr(fullSrvMsg, chatRoomMsg, _chatRoomNumber);
                    if (isValid)
                    {
                        if (UseApplicationState && (HttpContext.Current.Application[_chatRoomNumber] != null))
                        {
                            dict = (Dictionary<DateTime, string>)HttpContext.Current.Application[_chatRoomNumber];
                        }
                        if (UseAmazonElasticCache)
                        {
                            string dictJson = RedIs.Db.StringGet(_chatRoomNumber);
                            dict = (Dictionary<DateTime, string>)JsonConvert.DeserializeObject<Dictionary<DateTime, string>>(dictJson);                            
                        }

                        List<DateTime> pollKeys = dict.Keys.Where(k => k.Ticks > fullSrvMsg.Sender.LastPolled.Ticks).ToList();
                        DateTime polledMsgDate = DateTime.MinValue;
                        if (pollKeys.Count > 0)
                        {
                            polledMsgDate = pollKeys[0];
                            string firstPollClientMsg = dict[pollKeys[0]];
                            if (string.IsNullOrEmpty(firstPollClientMsg))
                            {
                                if (pollKeys.Count > 1)
                                {
                                    polledMsgDate = pollKeys[1];
                                    firstPollClientMsg = dict[pollKeys[1]];
                                }
                            }

                            chatRoomMsg.TContent = firstPollClientMsg;

                            _contact.LastPolled = polledMsgDate;
                            _contact.PolledMsgDates.Add(polledMsgDate);
                            chatRoomMsg.Sender.LastPolled = polledMsgDate;
                            chatRoomMsg.Sender.PolledMsgDates.Add(polledMsgDate);

                            UpdateContacts(_contact, chatRoomMsg, _chatRoomNumber);
                            chatRoomMsg = (new JsonChatRoom(_chatRoomNumber)).SaveJsonChatRoom(chatRoomMsg, _chatRoomNumber);
                            chatRoomMsg.Sender.LastPolled = polledMsgDate;
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


            return _responseString;

        }

        /// <summary>
        /// Pushes a new message for chatroom to the server
        /// </summary>
        /// <param name="cryptMsg">encrypted <see cref="FullSrvMsg<string>"/> with chat room number and last polled dates</param>
        /// <param name="chatRoomMembersCrypted">with client key encrypted message, that is stored in proc of server, but server can't decrypt</param>
        /// <returns>encrypted <see cref="FullSrvMsg<string>"/> with chat room number and last polled date changed to now</returns>
        [WebMethod]
        public string ChatRoomPushMessage(string cryptMsg, string chatRoomMembersCrypted)
        {
            InitMethod();
            bool isValid = false;
            Dictionary<DateTime, string> dict;
            SrvMsg srvMsg = new SrvMsg(_serverKey, _serverKey);
            FullSrvMsg<string> fullSrvMsg;

            _responseString = srvMsg.CqrBaseMsg(Constants.NACK);

            try
            {
                if (!string.IsNullOrEmpty(cryptMsg) && cryptMsg.Length >= 8)
                {
                    fullSrvMsg = srvMsg.NCqrSrvMsg<string>(cryptMsg);
                    _contact = fullSrvMsg.Sender;
                    _chatRoomNumber = (!string.IsNullOrEmpty(fullSrvMsg.ChatRoomNr)) ? fullSrvMsg.ChatRoomNr : fullSrvMsg.Sender.ChatRoomId;

                    FullSrvMsg<string> chatRoomMsg = (new JsonChatRoom(_chatRoomNumber)).LoadJsonChatRoom(fullSrvMsg, _chatRoomNumber);
                    isValid = ValidateChatRoomNr(fullSrvMsg, chatRoomMsg, _chatRoomNumber);
                    if (isValid)
                    {
                        DateTime now = DateTime.Now;

                        dict = new Dictionary<DateTime, string>();                      
                        if (this.UseApplicationState && HttpContext.Current.Application[_chatRoomNumber] != null)
                            dict = (Dictionary<DateTime, string>)HttpContext.Current.Application[_chatRoomNumber]; 
                        if (UseAmazonElasticCache)
                        {
                            string dictJson = RedIs.Db.StringGet(_chatRoomNumber);
                            dict = (Dictionary<DateTime, string>)JsonConvert.DeserializeObject<Dictionary<DateTime, string>>(dictJson);
                        }                       

                        dict.Add(now, chatRoomMembersCrypted);
                        
                        if (UseApplicationState)
                            HttpContext.Current.Application[_chatRoomNumber] = dict;
                        if (UseAmazonElasticCache)
                        {
                            string dictJson = JsonConvert.SerializeObject(dict);
                            RedIs.Db.StringSet(_chatRoomNumber, dictJson);
                        }

                        _contact.LastPolled = now;
                        _contact.PolledMsgDates.Add(now);
                        chatRoomMsg.Sender.LastPolled = now;
                        chatRoomMsg.Sender.PolledMsgDates.Add(now);
                        
                        UpdateContacts(_contact, chatRoomMsg, _chatRoomNumber);
                        chatRoomMsg = (new JsonChatRoom(_chatRoomNumber)).SaveJsonChatRoom(chatRoomMsg, _chatRoomNumber);
                        chatRoomMsg.Sender.LastPolled = now;
                    
                    }

                    _responseString = srvMsg.CqrSrvMsg<string>(chatRoomMsg);

                }
            }
            catch (Exception ex)
            {
                CqrException.SetLastException(ex);
                Area23Log.LogStatic(ex);
            }

            return _responseString;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="cryptMsg"></param>
        /// <returns></returns>
        [WebMethod]
        public string ChatRoomClose(string cryptMsg)
        {
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
                    _chatRoomNumber = (!string.IsNullOrEmpty(fullSrvMsg.ChatRoomNr)) ? fullSrvMsg.ChatRoomNr : fullSrvMsg.Sender.ChatRoomId;
                    JsonChatRoom jschatRoom = new JsonChatRoom(_chatRoomNumber);
                    FullSrvMsg<string> chatRoomMsg = jschatRoom.LoadJsonChatRoom(fullSrvMsg, _chatRoomNumber);
                    isValid = ValidateChatRoomNr(fullSrvMsg, chatRoomMsg, _chatRoomNumber);
                    if (isValid)
                    {
                        jschatRoom.DeleteJsonChatRoom(_chatRoomNumber);
                    }

                    _responseString = srvMsg.CqrSrvMsg<string>(chatRoomMsg);

                }
            }
            catch (Exception ex)
            {
                CqrException.SetLastException(ex);
                Area23Log.LogStatic(ex);
            }


            return _responseString;

        }



        [WebMethod]
        public override string TestService()
        {
            return base.TestService();
        }

        [WebMethod]
        public override string GetIPAddress()
        {
            return base.GetIPAddress();
        }



        [WebMethod]
        public virtual string TestCache()
        {
            string testReport = $"{DateTime.Now.Area23DateTimeMilliseconds()}:TestCache() started.\n";
            try
            {
                InitMethod(); 
            }
            catch (Exception ex1)
            {
                testReport += $"{DateTime.Now.Area23DateTimeMilliseconds()}: Exception {ex1.GetType()}: {ex1.Message}\n\t{ex1}\n";
            }
            
            testReport += $"{DateTime.Now.Area23DateTimeMilliseconds()}: InitMethod() completed.\n";

            Dictionary<Guid, CqrContact> dictCacheTest = new Dictionary<Guid, CqrContact>();
            foreach (CqrContact c in _contacts)
            {
                if (c != null && c.Cuid != null && c.Cuid != Guid.Empty && 
                    !dictCacheTest.Keys.Contains(c.Cuid))
                    dictCacheTest.Add(c.Cuid, c);
            }
            testReport += $"{DateTime.Now.Area23DateTimeMilliseconds()}: Added {dictCacheTest.Count} count contacts to Dictionary<Guid, CqrContact>...\n";
            if (UseAmazonElasticCache)
            {
                
                try
                {
                    if (UseAmazonElasticCache)
                    {
                        testReport += $"{DateTime.Now.Area23DateTimeMilliseconds()}: Ready to connect to {ConfigurationManager.AppSettings[Constants.VALKEY_CACHE_HOST_PORT]}\n";
                        string status = RedIs.ConnMux.GetStatus();
                        testReport += $"{DateTime.Now.Area23DateTimeMilliseconds()}: ConnectionMulitplexer.Status = {status}\n";

                        string dictJson = JsonConvert.SerializeObject(dictCacheTest);
                        testReport += $"{DateTime.Now.Area23DateTimeMilliseconds()}: Serialized Dictionary<Guid, CqrContact> to json string.\n";
                        RedIs.Db.StringSet("TestCache", dictJson);
                        testReport += $"{DateTime.Now.Area23DateTimeMilliseconds()}: Added serialized json string to cache.\n";

                        string jsonOut = RedIs.Db.StringGet("TestCache");
                        testReport += $"{DateTime.Now.Area23DateTimeMilliseconds()}: Got json serialized string from cache: {jsonOut}.\n";
                        Dictionary<Guid, CqrContact> outdict = (Dictionary<Guid, CqrContact>)JsonConvert.DeserializeObject<Dictionary<Guid, CqrContact>>(jsonOut);
                        testReport += $"{DateTime.Now.Area23DateTimeMilliseconds()}: Deserialized json sring to (Dictionary<Guid, CqrContact> with {outdict.Keys.Count} keys."; 
                    }                    
                }
                catch (Exception ex2)
                {
                    testReport += $"{DateTime.Now.Area23DateTimeMilliseconds()}: Exception {ex2.GetType()}: {ex2.Message}\n\t{ex2}\n";
                }
            }

            return testReport;
        }


    }


}
