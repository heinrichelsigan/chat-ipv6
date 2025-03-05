using Amazon.ElastiCacheCluster;
using Enyim.Caching;
using Enyim.Caching.Memcached;
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


namespace EU.CqrXs.CqrSrv.CqrJd
{

    /// <summary>
    /// Summary description for CqrService
    /// </summary>
    [WebService(Namespace = "https://cqrjd.eu/cqrsrv/cqrjd/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class CqrService : Util.BaseWebService
    {        
        
        [WebMethod]
        public string Send1StSrvMsg(string cryptMsg)
        {            
            InitMethod();
            HttpContext.Current.Application["lastmsg"] = cryptMsg;
            
            SrvMsg1 srv1stMsg = new SrvMsg1(_serverKey);
            SrvMsg1 srv1stRespMsg = new SrvMsg1(_serverKey);
            SrvMsg responseSrvMsg = new SrvMsg(_serverKey, _serverKey);
            
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


        [WebMethod]
        public string ChatRoomInvite(string cryptMsg)
        {
            InitMethod();

            SrvMsg srvMsg = new SrvMsg(_serverKey, _serverKey);
            FullSrvMsg<string> fullSrvMsg, chatRSrvMsg;

            SrvMsg responseSrvMsg = new SrvMsg(_serverKey, _serverKey);
            _responseString = responseSrvMsg.CqrBaseMsg(Constants.NACK);

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
                        memClient.Store(StoreMode.Add, chatRoomId, dict, new TimeSpan(23, 23, 23, 23));
                    
                    if (UseApplicationState)
                        HttpContext.Current.Application[chatRoomId] = dict;

                    chatRSrvMsg.Sender.LastPolled = now;
                    chatRSrvMsg.Sender.PolledMsgDates.Add(now);
                    _contact.LastPolled = now;
                    _contact.PolledMsgDates.Add(now);
                    UpdateContacts(_contact, chatRSrvMsg, chatRoomId);

                    _responseString = responseSrvMsg.CqrSrvMsg<string>(chatRSrvMsg);
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
        public string ChatRoomPoll(string cryptMsg)
        {
            InitMethod();

            Dictionary<DateTime, string> dict = new Dictionary<DateTime, string>();
            bool isValid = false;
            SrvMsg srvMsg = new SrvMsg(_serverKey, _serverKey);
            FullSrvMsg<string> fullSrvMsg;
            SrvMsg responseSrvMsg = new SrvMsg(_serverKey, _serverKey);

            _responseString = responseSrvMsg.CqrBaseMsg(Constants.NACK);

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
                            dict = (Dictionary<DateTime, string>)memClient.Get(_chatRoomNumber);
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

                    _responseString = responseSrvMsg.CqrSrvMsg<string>(chatRoomMsg);

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
        public string ChatRoomPushMessage(string cryptMsg, string chatRoomMembersCrypted)
        {
            InitMethod();
            bool isValid = false;
            Dictionary<DateTime, string> dict;
            SrvMsg srvMsg = new SrvMsg(_serverKey, _serverKey);
            FullSrvMsg<string> fullSrvMsg;
            SrvMsg responseSrvMsg = new SrvMsg(_serverKey, _serverKey);

            _responseString = responseSrvMsg.CqrBaseMsg(Constants.NACK);

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
                            dict = (Dictionary<DateTime, string>)memClient.Get(_chatRoomNumber);

                        dict.Add(now, chatRoomMembersCrypted);
                        
                        if (UseApplicationState)
                            HttpContext.Current.Application[_chatRoomNumber] = dict;
                        if (UseAmazonElasticCache)
                            memClient.Store(StoreMode.Replace, _chatRoomNumber, dict, new TimeSpan(23, 23, 23, 23));

                        _contact.LastPolled = now;
                        _contact.PolledMsgDates.Add(now);
                        chatRoomMsg.Sender.LastPolled = now;
                        chatRoomMsg.Sender.PolledMsgDates.Add(now);
                        
                        UpdateContacts(_contact, chatRoomMsg, _chatRoomNumber);
                        chatRoomMsg = (new JsonChatRoom(_chatRoomNumber)).SaveJsonChatRoom(chatRoomMsg, _chatRoomNumber);
                            chatRoomMsg.Sender.LastPolled = now;
                    }
                    _responseString = responseSrvMsg.CqrSrvMsg<string>(chatRoomMsg);

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
        public string ChatRoomClose(string cryptMsg)
        {
            InitMethod();
            bool isValid = false;
            SrvMsg srvMsg = new SrvMsg(_serverKey, _serverKey);
            FullSrvMsg<string> fullSrvMsg;
            List<CqrContact> _invited = new List<CqrContact>();
            SrvMsg responseSrvMsg = new SrvMsg(_serverKey);

            _responseString = responseSrvMsg.CqrBaseMsg(Constants.NACK);

            try
            {
                if (!string.IsNullOrEmpty(cryptMsg) && cryptMsg.Length >= 8)
                {
                    fullSrvMsg = srvMsg.NCqrSrvMsg<string>(cryptMsg);
                    _contact = AddContact(fullSrvMsg.Sender);
                    _invited = fullSrvMsg.Recipients.ToList();
                    fullSrvMsg.Recipients.Add(_contact);
                    _invited.Add(_contact);

                    ;
                    if (string.IsNullOrEmpty(fullSrvMsg.TContent))
                        throw new Exception();

                    FullSrvMsg<string> chatRoomMsg = (new JsonChatRoom(fullSrvMsg.TContent)).LoadJsonChatRoom(fullSrvMsg, fullSrvMsg.TContent);
                    if (fullSrvMsg.TContent.Equals(chatRoomMsg.TContent))
                    {
                        if (fullSrvMsg.Sender == chatRoomMsg.Sender)
                            isValid = true;
                    }
                    if (isValid)
                    {
                        if (HttpContext.Current.Application[fullSrvMsg.TContent] != null)
                        {
                            ConcurrentBag<string> bag = (ConcurrentBag<string>)HttpContext.Current.Application[fullSrvMsg.TContent];
                            chatRoomMsg.TContent =
                                $"Closing chat room {fullSrvMsg.TContent} number of msg remaining {bag.ToArray().Length}.";
                            HttpContext.Current.Application.Remove(fullSrvMsg.TContent);
                        }

                    }
                    _responseString = responseSrvMsg.CqrSrvMsg<string>(chatRoomMsg);

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
        public virtual string TestCache()
        {
            string testReport = $"TestCache() started {DateTime.Now.Area23DateTimeWithMillis()}\n";
            try
            {
                InitMethod(); 
            }
            catch (Exception ex1)
            {
                testReport += $"Exception {ex1.GetType()}: {ex1.Message}\n\t{ex1}\n";
            }
            
            testReport += "InitMethod() completed.\n";

            Dictionary<Guid, CqrContact> dictCacheTest = new Dictionary<Guid, CqrContact>();
            foreach (CqrContact c in _contacts)
            {
                if (c != null && c.Cuid != null && c.Cuid != Guid.Empty && 
                    !dictCacheTest.Keys.Contains(c.Cuid))
                    dictCacheTest.Add(c.Cuid, c);
            }
            testReport += $"Added {dictCacheTest.Count} count contacts to Dictionary<Guid, CqrContact>...\n";
            if (UseAmazonElasticCache)
            {
                try
                {
                    memClient.Store(StoreMode.Add, "TestCache", dictCacheTest, new TimeSpan(23, 23, 23, 23));
                }
                catch (Exception ex2)
                {
                    testReport += $"Exception {ex2.GetType()}: {ex2.Message}\n\t{ex2}\n";
                }
            }

            return testReport;
        }


    }


}
