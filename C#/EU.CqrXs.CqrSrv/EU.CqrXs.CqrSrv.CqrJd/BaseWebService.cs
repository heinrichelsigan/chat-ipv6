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

namespace EU.CqrXs.CqrSrv.CqrJd
{

    [WebService(Namespace = "https://cqrjd.eu/cqrsrv/cqrjd/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class BaseWebService : WebService
    {
        protected internal static HashSet<CqrContact> _contacts;
        protected internal CqrContact _contact;
        protected internal string _literalServerIPv4, _literalServerIPv6, _literalClientIp;
        protected internal string myServerKey = string.Empty;
        protected internal string _decrypted = string.Empty, _encrypted = string.Empty;

        public BaseWebService()
        {
            InitMethod();
        }


        public void InitMethod()
        {

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


        }

        public CqrContact HandleContact(CqrContact contact)
        {
            CqrContact foundCt = JsonContacts.FindContactByNameEmail(_contacts, contact);
            if (foundCt != null)
            {
                foundCt.ContactId = contact.ContactId;
                if (foundCt.Cuid == null || foundCt.Cuid == Guid.Empty)
                    foundCt.Cuid = new Guid();
                if (!string.IsNullOrEmpty(contact.Address))
                    foundCt.Address = contact.Address;
                if (!string.IsNullOrEmpty(_contact.Mobile))
                    foundCt.Mobile = contact.Mobile;

                if (contact.ContactImage != null && !string.IsNullOrEmpty(contact.ContactImage.ImageFileName) &&
                    !string.IsNullOrEmpty(contact.ContactImage.ImageBase64))
                    foundCt.ContactImage = contact.ContactImage;


                return foundCt;
            }
            else
            {
                if (contact.Cuid == null || contact.Cuid == Guid.Empty)
                    contact.Cuid = new Guid();
                _contacts.Add(contact);
                JsonContacts.SaveJsonContacts(_contacts);

                return _contact;
            }
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

    }
}