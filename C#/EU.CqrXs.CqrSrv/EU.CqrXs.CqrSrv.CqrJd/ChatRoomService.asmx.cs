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



namespace EU.CqrXs.CqrSrv.CqrJd
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


        
        string responseMsg = string.Empty;

        [WebMethod]       
        public string IniviteToChatRoom(string cryptMsg)
        {            
            myServerKey = string.Empty;
            _decrypted = string.Empty;
            responseMsg = string.Empty;
            _contact = null;




            SrvMsg srvMsg = new SrvMsg(myServerKey, myServerKey);
            FullSrvMsg<string> fullSrvMsg;
            List<CqrContact> _invited = new List<CqrContact>();

            SrvMsg1 cqrSrvResponseMsg = new SrvMsg1(myServerKey);
            SrvMsg responseSrvMsg = new SrvMsg(myServerKey);

            try
            {
                if (!string.IsNullOrEmpty(cryptMsg) && cryptMsg.Length >= 8)
                {
                    fullSrvMsg = srvMsg.NCqrSrvMsg<string>(cryptMsg);
                    _contact = fullSrvMsg.Sender;
                    _invited = fullSrvMsg.Recipients;
                }
            }
            catch (Exception ex)
            {
                CqrException.SetLastException(ex);
                Area23Log.LogStatic(ex);
            }

            responseMsg = responseSrvMsg.CqrBaseMsg("", EncodingType.Base64);

            if (!string.IsNullOrEmpty(_decrypted) && _contact != null && !string.IsNullOrEmpty(_contact.NameEmail))
            {
                _contact = HandleContact(_contact);

                responseMsg = cqrSrvResponseMsg.CqrSrvMsg1(_contact, EncodingType.Base64);

                
            }


            return responseMsg;
        
        }

        [WebMethod]
        public string PollMyInbox(string cryptMsg)
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
            SrvMsg responseSrvMsg = new SrvMsg(myServerKey);
            HttpContext.Current.Application["lastmsg"] = cryptMsg;

            try
            {
                if (!string.IsNullOrEmpty(cryptMsg) && cryptMsg.Length >= 8)
                {
                    _contact = srv1stMsg.NCqrSrvMsg1(cryptMsg);
                    _decrypted = _contact.ToJson();
                }
            }
            catch (Exception ex)
            {
                CqrException.SetLastException(ex);
                Area23Log.LogStatic(ex);
            }

            responseMsg = responseSrvMsg.CqrBaseMsg("", EncodingType.Base64);
            string dummyContent = "";
            if (!string.IsNullOrEmpty(_decrypted) && _contact != null && !string.IsNullOrEmpty(_contact.NameEmail))
            {
                Application["lastdecrypted"] = _decrypted;

                CqrContact foundCt = JsonContacts.FindContactByNameEmail(_contacts, _contact);
                if (foundCt != null)
                {
                    if (foundCt.Cuid == _contact.Cuid &&
                        foundCt.NameEmail == _contact.NameEmail &&
                        foundCt.Mobile == _contact.Mobile)
                    // && foundCt.Address == _contact.Address && foundCt.Mobile == _contact.Mobile)
                    {
                        string dir = ConfigurationManager.AppSettings["SpoolerDirectory"].ToString();
                        if (Directory.Exists(dir))
                        {                            
                            string userDir = Path.Combine(dir, _contact.Cuid.ToString());
                            if (Directory.Exists(userDir))
                            {
                                responseSrvMsg = new SrvMsg(foundCt, foundCt, myServerKey);

                                foreach (var dirs in Directory.GetDirectories(userDir))
                                {
                                    dummyContent += dirs + Environment.NewLine;
                                    Area23Log.LogStatic("dir = " + dirs);
                                    // Todo: GET  contact uid for directory and
                                    // if contact guid == dirname exists
                                    foreach (var files in Directory.GetFiles(dirs))
                                    {
                                        Area23Log.LogStatic(files);
                                        dummyContent += files + Environment.NewLine;
                                    }
                                }
                            }

                        }
                        
                        // todo read messages
                    }
                }

                responseMsg = responseSrvMsg.CqrBaseMsg(dummyContent);


            }

            return responseMsg;

        }




        [WebMethod]
        public string SendValidatedSrvMsg(Guid from, Guid to, string cryptMsgSrv, string cryptMsgPartner)
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

            SrvMsg cqrServerMsg = new SrvMsg(myServerKey);
            SrvMsg cqrResponseMsg = new SrvMsg(myServerKey);
            SrvMsg1 cqrSrvResponseMsg = new SrvMsg1(myServerKey);

            HttpContext.Current.Application["lastmsg"] = cryptMsgSrv;

            try
            {
                if (!string.IsNullOrEmpty(cryptMsgSrv) && cryptMsgSrv.Length >= 8)
                {
                    MsgContent msgCt = cqrServerMsg.NCqrBaseMsg(cryptMsgSrv, EncodingType.Base64);
                    _decrypted = msgCt.Message;
                }
            }
            catch (Exception ex)
            {
                CqrException.SetLastException(ex);
                Area23Log.LogStatic(ex);
            }

            responseMsg = cqrResponseMsg.CqrBaseMsg(Constants.ACK, EncodingType.Base64);

            if (!string.IsNullOrEmpty(_decrypted))
            {
                Application["lastdecrypted"] = _decrypted;
                CqrContact ctret = _contacts.ToList().Where(c => c.Cuid == to || c.Cuid == from).ToList().FirstOrDefault();
                string reStr = cqrSrvResponseMsg.CqrSrvMsg1(ctret, EncodingType.Base64);

                return reStr;
            }


            return responseMsg;

        }


        [WebMethod]
        public string SendSrvMsg(string cryptMsgSrv, string cryptMsgPartner)
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

            SrvMsg cqrServerMsg = new SrvMsg(myServerKey);
            SrvMsg cqrResponseMsg = new SrvMsg(myServerKey);
            SrvMsg1 cqrSrvResponseMsg = new SrvMsg1(myServerKey);

            HttpContext.Current.Application["lastmsg"] = cryptMsgSrv;

            try
            {
                if (!string.IsNullOrEmpty(cryptMsgSrv) && cryptMsgSrv.Length >= 8)
                {
                    MsgContent msgCt = cqrServerMsg.NCqrBaseMsg(cryptMsgSrv, EncodingType.Base64);
                    _decrypted = msgCt.Message;
                }
            }
            catch (Exception ex)
            {
                CqrException.SetLastException(ex);
                Area23Log.LogStatic(ex);
            }

            responseMsg = cqrResponseMsg.CqrBaseMsg(Constants.ACK, EncodingType.Base64);

           // TODO:

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
            ret += "\n" +Assembly.GetExecutingAssembly().GetName().Version.ToString();

            object[] oattributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
            if (oattributes.Length > 0)            
                ret += "\n" +((AssemblyDescriptionAttribute)oattributes[0]).Description;

            object[] pattributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
            if (pattributes.Length > 0)
                ret += "\n" + ((AssemblyProductAttribute)pattributes[0]).Product;

            object[] crattributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
            if (crattributes.Length > 0)
                ret += "\n" + ((AssemblyCopyrightAttribute)crattributes[0]).Copyright;

            object[] cpattributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
            if (cpattributes.Length > 0)
                ret += "\n"  + ((AssemblyCompanyAttribute)cpattributes[0]).Company;


            return ret;

        }

    
    
    
    }


}
