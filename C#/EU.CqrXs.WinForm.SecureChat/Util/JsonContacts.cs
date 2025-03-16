using Area23.At.Framework.Core.CqrXs.CqrMsg;
using Area23.At.Framework.Core.Static;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading;
using System.Web;

namespace EU.CqrXs.WinForm.SecureChat.Util
{
    public static class JsonContacts
    {
        static object _lock = new object();
        static HashSet<CqrContact> _contacts;
        internal static string JsonContactsFileName { get => JsonHelper.JsonContactsFile; }

        static JsonContacts()
        {
            try
            {
                _contacts = LoadJsonContacts();
            }
            catch { }
            if (_contacts == null || _contacts.Count == 0)
            {
                _contacts = new HashSet<CqrContact>(Entities.Settings.Instance.Contacts);
            }
        }

        internal static HashSet<CqrContact> LoadJsonContacts()
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
                _contacts = JsonConvert.DeserializeObject<HashSet<CqrContact>>(jsonText);
            }
            if (_contacts == null || _contacts.Count == 0)
                _contacts = new HashSet<CqrContact>();

            //if (BaseWebService.UseApplicationState)
            //        HttpContext.Current.Application[Constants.JSON_CONTACTS] = _contacts;
            //if (BaseWebService.UseAmazonElasticCache)
            //{
            //    string dictContactsJson = JsonConvert.SerializeObject(_contacts);
            //    RedIs.Db.StringSet(Constants.JSON_CONTACTS, dictContactsJson);                   
            //}

            return _contacts;
        }

        internal static void SaveJsonContacts(HashSet<CqrContact> contacts)
        {

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


        internal static HashSet<CqrContact> GetContacts()
        {
            bool loadJson = false;

            if (_contacts == null || _contacts.Count < 1)
            {
                //if (BaseWebService.UseApplicationState && HttpContext.Current.Application[Constants.JSON_CONTACTS] != null)
                //    _contacts = (HashSet<CqrContact>)(HttpContext.Current.Application[Constants.JSON_CONTACTS]);                    
                //if (BaseWebService.UseAmazonElasticCache)
                //{
                //    string dictContactsJson = RedIs.Db.StringGet(Constants.JSON_CONTACTS);
                //    _contacts = (HashSet<CqrContact>)JsonConvert.DeserializeObject<HashSet<CqrContact>>(dictContactsJson);
                //}
                //if (_contacts == null || _contacts.Count < 2)
                //    loadJson = true;
                _contacts = JsonContacts.LoadJsonContacts();
            }
            //else 
            //    loadJson = true;
            //if (loadJson)
               
            
            return _contacts;
        }

        public static CqrContact FindContactByNameEmail(List<CqrContact> contacts, CqrContact searchContact)
        {
            CqrContact foundC = FindContactByNameEmail(contacts, searchContact.Name, searchContact.Email, searchContact.Mobile);
            return foundC;
        }
        

        public static CqrContact FindContactByNameEmail(List<CqrContact> contacts, string cName, string cEmail, string cMobile)
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



        public static List<string> ChatRoomNumbersFromFs()
        {

            List<string> chatRooms = new List<string>();
            string[] csr = Directory.GetFiles(LibPaths.SystemDirPath, "room*.json");
            string file = "";
            foreach (string filedir in csr)
            {
                file = Path.GetFileName(filedir);
                chatRooms.Add(file);
            }

            // if (BaseWebService.UseApplicationState)
            //     HttpContext.Current.Application["ChatRooms"] = chatRooms;


            return chatRooms;
        }
    }
}