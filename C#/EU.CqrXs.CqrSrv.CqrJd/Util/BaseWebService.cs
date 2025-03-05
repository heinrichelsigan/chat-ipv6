using Amazon.ElastiCacheCluster;
using Enyim.Caching;
using Enyim.Caching.Memcached;
using Area23.At.Framework.Library.CqrXs.CqrMsg;
using Area23.At.Framework.Library;
using EU.CqrXs.CqrSrv.CqrJd.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Services;
using System.Configuration;
using Area23.At.Framework.Library.Crypt.EnDeCoding;
using System.IO;
using Area23.At.Framework.Library.Static;
using System.Diagnostics.Contracts;
using Amazon.ElastiCacheCluster;
using Enyim.Caching;

namespace EU.CqrXs.CqrSrv.CqrJd.Util
{

    [WebService(Namespace = "https://cqrjd.eu/cqrsrv/cqrjd/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class BaseWebService : WebService
    {
        protected internal static HashSet<CqrContact> _contacts;
        protected internal CqrContact _contact;
        protected internal string _literalServerIPv4, _literalServerIPv6, _literalClientIp;
        protected internal string _serverKey = string.Empty;
        protected internal string _decrypted = string.Empty, _encrypted = string.Empty;
        protected internal string _responseString = string.Empty;
        protected internal string _chatRoomNumber = string.Empty;
        protected internal ElastiCacheClusterConfig config;
        protected internal MemcachedClient memClient;
        protected internal bool useAWSCache = true;

        public bool UseApplicationState
        {
            get => ((ConfigurationManager.AppSettings["UseHttpApplicationState"] != null)
                ? Convert.ToBoolean(ConfigurationManager.AppSettings["UseHttpApplicationState"])
                : true);
        }

        public bool UseAmazonElasticCache
        {
            get => ((ConfigurationManager.AppSettings["UseAmazonElasticCache"] != null &&
                    Boolean.TryParse(ConfigurationManager.AppSettings["UseAmazonElasticCache"].ToString(),
                        out useAWSCache)) ? useAWSCache : true);
                
        }



        public BaseWebService()
        {
            InitMethod();
        }


        public virtual void InitMethod()
        {
            config = new ElastiCacheClusterConfig();
            memClient = new MemcachedClient(config);

            GetContacts();
            GetServerKey();
            _literalClientIp = HttpContext.Current.Request.UserHostAddress;
            _decrypted = string.Empty;
            _responseString = string.Empty;
            _contact = null;
        }

        [WebMethod]
        public virtual string TestService()
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



        public CqrContact HandleContact(CqrContact contact)
        {
            return AddContact(contact);
        }

        protected string GetServerKey()
        {
            _literalClientIp = HttpContext.Current.Request.UserHostAddress;

            if (ConfigurationManager.AppSettings["ExternalClientIP"] != null)
                _serverKey = (string)ConfigurationManager.AppSettings["ExternalClientIP"];
            else
                _serverKey = HttpContext.Current.Request.UserHostAddress;
            _serverKey += Constants.APP_NAME;

            return _serverKey;
        }

        internal CqrContact[] GetContacts()
        {
            if (_contacts == null || _contacts.Count < 1)
            {
                _contacts = (Application[Constants.JSON_CONTACTS] != null)
                    ? (HashSet<CqrContact>)(Application[Constants.JSON_CONTACTS])
                    : JsonContacts.LoadJsonContacts();
            }
            return _contacts.ToArray();
        }

        internal CqrContact AddContact(CqrContact cqrContact)
        {
            GetContacts();
            CqrContact foundCt = JsonContacts.FindContactByNameEmail(_contacts, cqrContact);
            if (foundCt != null)
            {
                foundCt.ContactId = cqrContact.ContactId;
                if (foundCt.Cuid == null || foundCt.Cuid == Guid.Empty)
                    foundCt.Cuid = Guid.NewGuid();
                if (!string.IsNullOrEmpty(cqrContact.Address))
                    foundCt.Address = cqrContact.Address;
                if (cqrContact.Mobile != null && cqrContact.Mobile.Length > 1)
                    foundCt.Mobile = cqrContact.Mobile;

                if (cqrContact != null && cqrContact.ContactImage != null &&
                    !string.IsNullOrEmpty(cqrContact.ContactImage.ImageFileName) &&
                    cqrContact.ContactImage.ImageBase64 != null &&
                    !string.IsNullOrEmpty(cqrContact.ContactImage.ImageBase64))
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

        internal void UpdateContacts(CqrContact contact, FullSrvMsg<string> chatRoomMsg, string chatRoomNr)
        {
            bool foundCt = false;
            CqrContact toDelContact = null;
            if (contact == null || string.IsNullOrEmpty(contact.Email) || contact.Cuid == Guid.Empty)
                return;

            if ((chatRoomMsg.Sender.Cuid == contact.Cuid && chatRoomMsg.Sender.Email == contact.Email) ||
                (chatRoomMsg.Sender.NameEmail == contact.NameEmail))
            {
                chatRoomMsg.Sender = new CqrContact(contact, chatRoomNr, contact.LastPolled, contact.Hash);
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
                    chatRoomMsg.Recipients.Add(new CqrContact(contact, chatRoomNr, contact.LastPolled, contact._hash));

            (new JsonChatRoom(_chatRoomNumber)).SaveJsonChatRoom(chatRoomMsg, chatRoomNr);

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
                    _contacts.Add(new CqrContact(contact, chatRoomNr, contact.LastPolled, contact.Hash));

            JsonContacts.SaveJsonContacts(_contacts);

        }


        internal FullSrvMsg<string> InviteToChatRoom(FullSrvMsg<string> fullSrvMsg)
        {
            List<CqrContact> _invited = new List<CqrContact>();
            // fullSrvMsg.Recipients.Add(_contact);
            bool addSender = true;
            foreach (CqrContact c in fullSrvMsg.Recipients)
            {
                if (!_invited.Contains(c))
                    _invited.Add(c);
                if (c.Email == fullSrvMsg.Sender.Email)
                    addSender = false;
            }
            if (addSender)
                _invited.Add(fullSrvMsg.Sender);

            string chatRoomId = string.Empty;
            //if (string.IsNullOrEmpty(fullSrvMsg.TContent))
            //    chatRoomId = String.Format("{0:yyMMdd_HHmm}_{1}.json", DateTime.Now,
            //        fullSrvMsg.TContent.Replace("@", "_").Replace(".", "_"));

            if (string.IsNullOrEmpty(chatRoomId))
                chatRoomId = String.Format("room_{0:yyMMddHH}_{1}.json", DateTime.Now,
                    fullSrvMsg.Sender.Email.Replace("@", "_").Replace(".", "_"));

            if (string.IsNullOrEmpty(chatRoomId))
                chatRoomId = $"room_{DateTime.Now:yyMMddHHmm}_0.json";

            FullSrvMsg<string> chatRSrvMsg = new FullSrvMsg<string>();
            chatRSrvMsg.Sender = fullSrvMsg.Sender;
            chatRSrvMsg.Sender.ChatRoomId = chatRoomId;
            chatRSrvMsg.Recipients = new HashSet<CqrContact>(_invited);
            chatRSrvMsg._hash = fullSrvMsg._hash;
            chatRSrvMsg.ChatRoomNr = chatRoomId;
            chatRSrvMsg.RawMessage = chatRSrvMsg.ToJson();
            chatRSrvMsg._message = chatRSrvMsg.ToJson();
            chatRSrvMsg.MsgType = MsgEnum.Json;
            (new JsonChatRoom(chatRoomId)).SaveJsonChatRoom(chatRSrvMsg, chatRoomId);

            return chatRSrvMsg;
        }


        public bool ValidateChatRoomNr(FullSrvMsg<string> fullSrvMsg, FullSrvMsg<string> chatRoomMsg, string chatRoomId)
        {

            bool isValid = false;
            if (chatRoomId.Equals(chatRoomMsg.ChatRoomNr))
            {
                foreach (CqrContact c in chatRoomMsg.Recipients)
                {
                    if (fullSrvMsg.Sender.NameEmail == c.NameEmail ||
                        fullSrvMsg.Sender.Email == c.Email)
                    {
                        _contact = c;
                        isValid = true;
                    }
                }
                if (fullSrvMsg.Sender == chatRoomMsg.Sender)
                {
                    _contact = chatRoomMsg.Sender;
                    isValid = true;
                }
            }
        
            return isValid;
        }

       

    }
}