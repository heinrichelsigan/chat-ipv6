﻿using Area23.At.Framework.Library;
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
            myServerKey = string.Empty;
            _decrypted = string.Empty;
            responseMsg = string.Empty;
            _contact = null;

            if (Application[Constants.JSON_CONTACTS] != null)
                _contacts = (HashSet<CqrContact>)(Application[Constants.JSON_CONTACTS]);
            else
                _contacts = JsonContacts.LoadJsonContacts();

            _literalClientIp = HttpContext.Current.Request.UserHostAddress;

            myServerKey = HttpContext.Current.Request.UserHostAddress;
            myServerKey += Constants.APP_NAME;

            SrvMsg1 srv1stMsg = new SrvMsg1(myServerKey);
            SrvMsg1 cqrSrvResponseMsg = new SrvMsg1(myServerKey);
            SrvMsg responseSrvMsg = new SrvMsg(myServerKey, myServerKey);
            Area23Log.LogStatic("myServerKey = " + myServerKey);
            HttpContext.Current.Application["lastmsg"] = cryptMsg;

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

            responseMsg = responseSrvMsg.CqrBaseMsg("", EncodingType.Base64);

            if (!string.IsNullOrEmpty(_decrypted) && _contact != null && !string.IsNullOrEmpty(_contact.NameEmail))
            {
                Application["lastdecrypted"] = _decrypted;

                CqrContact foundCt = JsonContacts.FindContactByNameEmail(_contacts, _contact);
                if (foundCt != null)
                {
                    foundCt.ContactId = _contact.ContactId;
                    if (foundCt.Cuid == null || foundCt.Cuid == Guid.Empty)
                        foundCt.Cuid = Guid.NewGuid();
                    if (!string.IsNullOrEmpty(_contact.Address))
                        foundCt.Address = _contact.Address;
                    if (!string.IsNullOrEmpty(_contact.Mobile))
                        foundCt.Mobile = _contact.Mobile;

                    if (_contact.ContactImage != null && !string.IsNullOrEmpty(_contact.ContactImage.ImageFileName) &&
                        !string.IsNullOrEmpty(_contact.ContactImage.ImageBase64))
                        foundCt.ContactImage = _contact.ContactImage;

                    Area23Log.LogStatic($"Contact found, updating it and returning it. {foundCt.Cuid} {foundCt.NameEmail}\n");
                    responseMsg = cqrSrvResponseMsg.CqrSrvMsg1(foundCt, EncodingType.Base64);
                }
                else
                {
                    _contact.Cuid = Guid.NewGuid();
                    _contacts.Add(_contact);

                    //try
                    //{
                    //    string processed = Area23.At.Framework.Library.Util.ProcessCmd.Execute(
                    //        "/usr/local/bin/createContact.sh",
                    //        _contact.Name + " " + _contact.Email + " " +
                    //        _contact.Mobile.Replace(" ", "") + " " + _contact.NameEmail + " ", false);
                    //} catch (Exception exCmdFail)
                    //{
                    //    Area23Log.LogStatic(exCmdFail);
                    //}
                    Area23Log.LogStatic($"Contact not found in json, adding contact, giving a new Guid {_contact.Cuid} and returning it.\n");
                    foundCt = _contact;

                    responseMsg = cqrSrvResponseMsg.CqrSrvMsg1(foundCt, EncodingType.Base64);
                }

                JsonContacts.SaveJsonContacts(_contacts);
            }


            return responseMsg;

        }


        [WebMethod]
        public string ChatRoomInvite(string cryptMsg)
        {
            myServerKey = string.Empty;
            _decrypted = string.Empty;
            responseMsg = string.Empty;
            _contact = null;

            SrvMsg srvMsg = new SrvMsg(myServerKey, myServerKey);
            FullSrvMsg<string> fullSrvMsg;
            List<CqrContact> _invited = new List<CqrContact>();
            SrvMsg responseSrvMsg = new SrvMsg(myServerKey);

            responseMsg = responseSrvMsg.CqrBaseMsg(Constants.NACK);
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
                    responseMsg = responseSrvMsg.CqrSrvMsg<string>(fullSrvMsg);
                }
            }
            catch (Exception ex)
            {
                CqrException.SetLastException(ex);
                Area23Log.LogStatic(ex);
            }


            return responseMsg;

        }

        [WebMethod]
        public string ChatRoomPoll(string cryptMsg)
        {
            myServerKey = string.Empty;
            _decrypted = string.Empty;
            responseMsg = string.Empty;
            _contact = null;
            bool isValid = false;
            SrvMsg srvMsg = new SrvMsg(myServerKey, myServerKey);
            FullSrvMsg<string> fullSrvMsg;
            List<CqrContact> _invited = new List<CqrContact>();
            SrvMsg responseSrvMsg = new SrvMsg(myServerKey);

            responseMsg = responseSrvMsg.CqrBaseMsg(Constants.NACK);

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

                    FullSrvMsg<string> chatRoomMsg = (new JsonChatRoom(fullSrvMsg.TContent)).LoadJsonChatRoom(fullSrvMsg.TContent);
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
                    responseMsg = responseSrvMsg.CqrSrvMsg<string>(chatRoomMsg);

                }
            }
            catch (Exception ex)
            {
                CqrException.SetLastException(ex);
                Area23Log.LogStatic(ex);
            }


            return responseMsg;

        }

        [WebMethod]
        public string ChatRoomPushMessage(string cryptMsg, string chatRoomMembersCrypted)
        {
            myServerKey = string.Empty;
            _decrypted = string.Empty;
            responseMsg = string.Empty;
            _contact = null;
            bool isValid = false;
            SrvMsg srvMsg = new SrvMsg(myServerKey, myServerKey);
            FullSrvMsg<string> fullSrvMsg;
            List<CqrContact> _invited = new List<CqrContact>();
            SrvMsg responseSrvMsg = new SrvMsg(myServerKey);

            responseMsg = responseSrvMsg.CqrBaseMsg(Constants.NACK);

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

                    FullSrvMsg<string> chatRoomMsg = (new JsonChatRoom(fullSrvMsg.TContent)).LoadJsonChatRoom(fullSrvMsg.TContent);
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
                    responseMsg = responseSrvMsg.CqrSrvMsg<string>(chatRoomMsg);

                }
            }
            catch (Exception ex)
            {
                CqrException.SetLastException(ex);
                Area23Log.LogStatic(ex);
            }


            return responseMsg;


        }

        [WebMethod]
        public string ChatRoomClose(string cryptMsg)
        {
            myServerKey = string.Empty;
            _decrypted = string.Empty;
            responseMsg = string.Empty;
            _contact = null;
            bool isValid = false;
            SrvMsg srvMsg = new SrvMsg(myServerKey, myServerKey);
            FullSrvMsg<string> fullSrvMsg;
            List<CqrContact> _invited = new List<CqrContact>();
            SrvMsg responseSrvMsg = new SrvMsg(myServerKey);

            responseMsg = responseSrvMsg.CqrBaseMsg(Constants.NACK);

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

                    FullSrvMsg<string> chatRoomMsg = (new JsonChatRoom(fullSrvMsg.TContent)).LoadJsonChatRoom(fullSrvMsg.TContent);
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
                    responseMsg = responseSrvMsg.CqrSrvMsg<string>(chatRoomMsg);

                }
            }
            catch (Exception ex)
            {
                CqrException.SetLastException(ex);
                Area23Log.LogStatic(ex);
            }


            return responseMsg;

        }


        [WebMethod]
        public string UpdateContacts(string cryptMsg)
        {
            return cryptMsg;
        }


    }


}
