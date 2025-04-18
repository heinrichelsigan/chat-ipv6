using Area23.At.Framework.Core.Cqr.Msg;
using Area23.At.Framework.Core.Cqr;
using Area23.At.Framework.Core.Static;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web;

namespace EU.CqrXs.Srv.Svc.Swashbuckle.Util
{

    /// <summary>
    /// JsonContacts 
    /// </summary>
    public static class JsonContacts
    {

        static object _lock = new object();
        static HashSet<CContact> _contacts;
        internal static string JsonContactsFileName { get => EU.CqrXs.Srv.Svc.Swashbuckle.Util.JsonHelper.JsonContactsFile; }
        
        
        /// <summary>
        /// JsonContacts
        /// </summary>
        static JsonContacts()
        {
            _contacts = LoadJsonContacts();            
        }

        /// <summary>
        /// LoadJsonContacts
        /// </summary>
        /// <returns><see cref="HashSet{CqrContact}"/></returns>
        internal static HashSet<CContact> LoadJsonContacts()
        {
            lock (_lock)
            {
                if (!System.IO.File.Exists(JsonContactsFileName))
                    System.IO.File.Create(JsonContactsFileName);
            }
            //Thread.Sleep(8);
            lock (_lock)
            {
                string jsonText = System.IO.File.ReadAllText(JsonContactsFileName);
                _contacts = JsonConvert.DeserializeObject<HashSet<CContact>>(jsonText);
            }
            if (_contacts == null || _contacts.Count == 0)
                _contacts = new HashSet<CContact>();

            //if (BaseWebService.UseApplicationState)
            //        HttpContext.Current.Application[Constants.JSON_CONTACTS] = _contacts;
            //if (BaseWebService.UseAmazonElasticCache)
            //{
            //    string dictContactsJson = JsonConvert.SerializeObject(_contacts);
            //    RedIs.Db.StringSet(Constants.JSON_CONTACTS, dictContactsJson);                   
            //}

            return _contacts;
        }


        /// <summary>
        /// Method to persist Json Contacts
        /// </summary>
        /// <param name="contacts">contacts to save</param>
        internal static void SaveJsonContacts(HashSet<CContact> contacts)
        {
            _contacts = _contacts ?? new HashSet<CContact>();
            if (contacts != null && contacts.Count > 0 && contacts.Count > _contacts.Count)
                _contacts = contacts;
            JsonSerializerSettings jsets = new JsonSerializerSettings();
            jsets.Formatting = Formatting.Indented;
            lock (_lock)
            {
                string jsonString = JsonConvert.SerializeObject(contacts, Formatting.Indented);
                System.IO.File.WriteAllText(JsonContactsFileName, jsonString);
            }

            //if (BaseWebService.UseApplicationState)
            //    HttpContext.Current.Application[Constants.JSON_CONTACTS] = contacts;
            //if (BaseWebService.UseAmazonElasticCache)
            //{
            //    string dictContactsJson = JsonConvert.SerializeObject(_contacts);
            //    RedIs.Db.StringSet(Constants.JSON_CONTACTS, dictContactsJson);
            //}

        }


        /// <summary>
        /// GetContacts get contacts from json file
        /// </summary>
        /// <returns><see cref="HashSet{CqrContact}"/></returns>
        internal static HashSet<CContact> GetContacts()
        {
            if (_contacts == null || _contacts.Count < 1)
                _contacts = JsonContacts.LoadJsonContacts();
            
            return _contacts;
        }

        /// <summary>
        /// FindContactByNameEmail
        /// </summary>
        /// <param name="contacts">contacts to search</param>
        /// <param name="searchContact">contact to find</param>
        /// <returns></returns>
        public static CContact FindContactByNameEmail(HashSet<CContact> contacts, CContact searchContact)
        {
            CContact foundC = FindContactByNameEmail(contacts, searchContact.Name, searchContact.Email, searchContact.Mobile);
            return foundC;
        }

        /// <summary>
        /// FindContactByNameEmail find contact by name email
        /// </summary>
        /// <param name="contacts">contacts to search</param>
        /// <param name="cName">name to find</param>
        /// <param name="cEmail">email to find</param>
        /// <param name="cMobile">mobile to find</param>
        /// <returns><see cref="CqrContact"/></returns>
        public static CContact FindContactByNameEmail(HashSet<CContact> contacts, string cName, string cEmail, string cMobile)
        {

            if (!string.IsNullOrEmpty(cName) || !string.IsNullOrEmpty(cEmail))
            {
                string cNameEmail = string.IsNullOrEmpty(cEmail) ? cName : $"{cName} <{cEmail}>";
                string cPhone = cMobile ?? string.Empty;

                foreach (CContact c in contacts)
                {
                    if (c != null && !string.IsNullOrEmpty(c.NameEmail))
                    {
                        if (c.NameEmail.Equals(cNameEmail, StringComparison.CurrentCultureIgnoreCase) ||
                            c.Email.Equals(cEmail, StringComparison.CurrentCultureIgnoreCase) ||
                                (!string.IsNullOrEmpty(c.Mobile) &&
                                    c.Mobile.Equals(cPhone, StringComparison.CurrentCultureIgnoreCase) &&
                                    c.Name.Equals(cName, StringComparison.CurrentCultureIgnoreCase)))
                        {
                            return c;
                        }
                    }
                }
            }

            return null;
        }


    }

}