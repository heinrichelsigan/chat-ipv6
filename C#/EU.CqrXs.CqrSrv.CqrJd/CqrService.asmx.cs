using Area23.At.Framework.Library;
using Area23.At.Framework.Library.Crypt;
using Area23.At.Framework.Library.Crypt.EnDeCoding;
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
using EU.CqrXs.CqrSrv.CqrJd.Util;
using Newtonsoft.Json;
using System.IO;
using Area23.At.Framework.Library.Util;
using System.Reflection;
using System.Collections.Concurrent;
using System.Web.Http.Controllers;


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
    public class CqrService : System.Web.Services.WebService
    {
        static HashSet<CqrContact> _contacts;
        CqrContact _contact;
        string _literalServerIPv4, _literalServerIPv6, _literalClientIp;
        string myServerKey = string.Empty;
        string _decrypted = string.Empty;
        string responseMsg = string.Empty;

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
            
            if (ConfigurationManager.AppSettings["ExternalClientIP"] != null)
                myServerKey = (string)ConfigurationManager.AppSettings["ExternalClientIP"];
            else
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
                    _invited = fullSrvMsg.Recipients;
                    fullSrvMsg.Recipients.Add(_contact);
                    _invited.Add(_contact);

                    ;
                    if (string.IsNullOrEmpty(fullSrvMsg.TContent))
                        fullSrvMsg.TContent = DateTime.Now.Ticks.ToString() + _contact.Email.Replace("@", ".") + "_.json";

                    (new JsonChatRoom(fullSrvMsg.TContent)).SaveJsonChatRoom(fullSrvMsg);

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
                    _invited = fullSrvMsg.Recipients;
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
                    _invited = fullSrvMsg.Recipients;
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
                    _invited = fullSrvMsg.Recipients;
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


        [WebMethod]
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

            string uhaddr = HttpContext.Current.Request.UserHostAddress;
            Area23Log.LogStatic($"Test Method called from {uhaddr}, returning {ret}");
            return ret;

        }




        internal CqrContact AddContact(CqrContact cqrContact)
        {
            CqrContact foundCt = JsonContacts.FindContactByNameEmail(_contacts, cqrContact);
            if (foundCt != null)
            {
                foundCt.ContactId = cqrContact.ContactId;
                if (foundCt.Cuid == null || foundCt.Cuid == Guid.Empty)
                    foundCt.Cuid = Guid.NewGuid();
                if (!string.IsNullOrEmpty(cqrContact.Address))
                    foundCt.Address = cqrContact.Address;
                if (!string.IsNullOrEmpty(_contact.Mobile))
                    foundCt.Mobile = cqrContact.Mobile;

                if (_contact.ContactImage != null && !string.IsNullOrEmpty(cqrContact.ContactImage.ImageFileName) &&
                    !string.IsNullOrEmpty(_contact.ContactImage.ImageBase64))
                    foundCt.ContactImage = cqrContact.ContactImage;
            }
            else
            {
                if (cqrContact.Cuid == null || cqrContact.Cuid == Guid.Empty)
                    cqrContact.Cuid = Guid.NewGuid();
                _contacts.Add(cqrContact);
                foundCt = cqrContact;
            }

            JsonContacts.SaveJsonContacts(_contacts);

            return foundCt;
        }


    }


}
