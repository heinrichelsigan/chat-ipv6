using Area23.At.Framework.Library.CqrXs.CqrMsg;
using Area23.At.Framework.Library.Static;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;

namespace EU.CqrXs.CqrSrv.CqrJd.Util
{
    public static class JsonContacts
    {
        static object _lock = new object();
        static HashSet<CqrContact> _contacts;
        internal static string JsonContactsFileName { get => Area23.At.Framework.Library.Static.JsonHelper.JsonContactsFile; }

        static JsonContacts()
        {
            _contacts = LoadJsonContacts();
        }

        internal static HashSet<CqrContact> LoadJsonContacts()
        {
            lock (_lock)
            {
                if (!System.IO.File.Exists(JsonContactsFileName))
                    System.IO.File.Create(JsonContactsFileName);
            }
            Thread.Sleep(8);
            lock (_lock)
            {
                string jsonText = System.IO.File.ReadAllText(JsonContactsFileName);
                _contacts = JsonConvert.DeserializeObject<HashSet<CqrContact>>(jsonText);
                if (_contacts == null || _contacts.Count == 0)
                    _contacts = new HashSet<CqrContact>();
                HttpContext.Current.Application[Constants.JSON_CONTACTS] = _contacts;
            }
            return _contacts;
        }

        internal static void SaveJsonContacts(HashSet<CqrContact> contacts)
        {
            lock (_lock)
            {
                if (contacts != null && contacts.Count > 0 && contacts.Count > _contacts.Count)
                    _contacts = contacts;
                JsonSerializerSettings jsets = new JsonSerializerSettings();
                jsets.Formatting = Formatting.Indented;
                string jsonString = JsonConvert.SerializeObject(contacts, Formatting.Indented);
                System.IO.File.WriteAllText(JsonContactsFileName, jsonString);
                HttpContext.Current.Application[Constants.JSON_CONTACTS] = contacts;
            }
        }


        internal static HashSet<CqrContact> GetContacts()
        {
            bool loadJson = false;

            if (_contacts == null || _contacts.Count < 1)
            {
                if (HttpContext.Current.Application[Constants.JSON_CONTACTS] != null)
                {
                    _contacts = (HashSet<CqrContact>)(HttpContext.Current.Application[Constants.JSON_CONTACTS]);
                    if (_contacts == null || _contacts.Count < 2)
                        loadJson = true;
                }
            }
            else 
                loadJson = true;
            if (loadJson)
                _contacts = JsonContacts.LoadJsonContacts();
            
            return _contacts;
        }

        public static CqrContact FindContactByNameEmail(HashSet<CqrContact> contacts, CqrContact searchContact)
        {
            CqrContact foundC = FindContactByNameEmail(contacts, searchContact.Name, searchContact.Email, searchContact.Mobile);
            return foundC;
        }
        

        public static CqrContact FindContactByNameEmail(HashSet<CqrContact> contacts, string cName, string cEmail, string cMobile)
        {

            if (!string.IsNullOrEmpty(cName) || !string.IsNullOrEmpty(cEmail))
            {
                string cNameEmail = string.IsNullOrEmpty(cEmail) ? cName : $"{cName} <{cEmail}>";
                string cPhone = cMobile ?? string.Empty;

                foreach (CqrContact c in contacts)
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