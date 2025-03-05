using Area23.At.Framework.Library;
using Area23.At.Framework.Library.Util;
using Area23.At.Framework.Library.Crypt;
using Area23.At.Framework.Library.Crypt.EnDeCoding;
using EU.CqrXs.CqrSrv.CqrJd.Util;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Services;
using Area23.At;
using Area23.At.Framework.Library.CqrXs;
using Area23.At.Framework.Library.CqrXs.CqrMsg;
using Area23.At.Framework.Library.CqrXs.CqrSrv;
using Newtonsoft.Json;
using System.IO;
using System.Reflection;
using System.Collections.Concurrent;
using Area23.At.Framework.Library.Static;



namespace EU.CqrXs.CqrSrv.CqrJd.Util
{
    /// <summary>
    /// Summary description for ChatRoomService
    /// </summary>
    [WebService(Namespace = "https://cqrjd.eu/cqrsrv/cqrjd/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class ChatRoomService : BaseWebService
    {


        [WebMethod]
        public string Send1StSrvMsg(string cryptMsg)
        {
            InitMethod();
            HttpContext.Current.Application["lastmsg"] = cryptMsg;

            SrvMsg1 srv1stMsg = new SrvMsg1(_serverKey);
            SrvMsg1 srv1RespMsg = new SrvMsg1(_serverKey);
            SrvMsg responseSrvMsg = new SrvMsg(_serverKey, _serverKey);
            Area23Log.LogStatic("_serverKey = " + _serverKey);
            

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

            _responseString = responseSrvMsg.CqrBaseMsg("", EncodingType.Base64);

            if (!string.IsNullOrEmpty(_decrypted) && _contact != null && !string.IsNullOrEmpty(_contact.NameEmail))
            {
                Application["lastdecrypted"] = _decrypted;
                CqrContact foundCt = AddContact(_contact);

                _responseString = srv1RespMsg.CqrSrvMsg1(foundCt, EncodingType.Base64);
            }            

            return _responseString;

        }


        [WebMethod]
        public string ChatRoomInvite(string cryptMsg)
        {
            InitMethod();

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
                    _contact = fullSrvMsg.Sender;
                    _contact = AddContact(fullSrvMsg.Sender);
                    _invited = fullSrvMsg.Recipients.ToList();
                    fullSrvMsg.Recipients.Add(_contact);
                    _invited.Add(_contact);

                    ;
                    string chatRoomId = string.Empty;
                    if (string.IsNullOrEmpty(fullSrvMsg.TContent))
                        chatRoomId = DateTime.Now.Area23DateTimeWithSeconds() + "_" +
                            fullSrvMsg.TContent.Replace("@", "_").Replace(".", "_") + "_.json";

                    if (string.IsNullOrEmpty(chatRoomId))
                        chatRoomId = DateTime.Now.Area23DateTimeWithSeconds() + "_" +
                            fullSrvMsg.Sender.Email.Replace("@", "_").Replace(".", "_") + "_.json";

                    if (string.IsNullOrEmpty(chatRoomId))
                        chatRoomId = DateTime.Now.Area23DateTimeWithSeconds() + "_.json";

                    fullSrvMsg.ChatRoomNr = chatRoomId;
                    (new JsonChatRoom(fullSrvMsg.TContent)).SaveJsonChatRoom(fullSrvMsg, chatRoomId);

                    ConcurrentBag<string> bag = new ConcurrentBag<string>();
                    bag.Add(fullSrvMsg.TContent.ToString());
                    HttpContext.Current.Application[fullSrvMsg.TContent] = bag;
                    _responseString = responseSrvMsg.CqrSrvMsg<string>(fullSrvMsg);
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
                    _contact = fullSrvMsg.Sender;
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
                        foreach (CqrContact c in chatRoomMsg.Recipients)
                        {
                            if (fullSrvMsg.Sender.NameEmail == c.NameEmail ||
                                fullSrvMsg.Sender.Email == c.Email)
                                isValid = true;
                        }
                        if (fullSrvMsg.Sender == chatRoomMsg.Sender)
                            isValid = true;
                    }
                    if (isValid)
                    {
                        if (HttpContext.Current.Application[fullSrvMsg.TContent] != null)
                        {
                            ConcurrentBag<string> bag = (ConcurrentBag<string>)HttpContext.Current.Application[fullSrvMsg.TContent];
                            chatRoomMsg.TContent = bag.ToArray().ToString();
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
                    _contact = fullSrvMsg.Sender;
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
                        foreach (CqrContact c in chatRoomMsg.Recipients)
                        {
                            if (fullSrvMsg.Sender.NameEmail == c.NameEmail ||
                                fullSrvMsg.Sender.Email == c.Email)
                                isValid = true;
                        }
                        if (fullSrvMsg.Sender == chatRoomMsg.Sender)
                            isValid = true;
                    }
                    if (isValid)
                    {
                        ConcurrentBag<string> bag = new ConcurrentBag<string>();
                        if (HttpContext.Current.Application[fullSrvMsg.TContent] != null)
                            bag = (ConcurrentBag<string>)HttpContext.Current.Application[fullSrvMsg.TContent];
                        bag.Add(chatRoomMembersCrypted);
                        HttpContext.Current.Application[fullSrvMsg.TContent] = bag;
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
        public string UpdateContacts(string cryptMsg)
        {
            return cryptMsg;
        }


    }


}
