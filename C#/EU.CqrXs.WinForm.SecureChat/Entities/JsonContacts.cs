using Area23.At.Framework.Core.Cqr;
using Area23.At.Framework.Core.Cqr.Msg;
using Area23.At.Framework.Core.Static;
using Area23.At.Framework.Core.Util;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading;
using System.Web;

namespace EU.CqrXs.WinForm.SecureChat.Entities
{

    /// <summary>
    /// Static class for JsonContacts
    /// providing load, save, get, find
    /// </summary>
    public static class JsonContacts
    {

        static object _lock = new object();
        static HashSet<CContact> _contacts;
        
        internal static string JsonContactsFile
        {
            get
            {
                string loadFileName = Path.Combine(LibPaths.SystemDirPath, Constants.JSON_CONTACTS_FILE);
                if (!File.Exists(loadFileName))
                {
                    loadFileName = AppContext.BaseDirectory.
                        Replace(LibPaths.SepChar + Constants.BIN_DIR, "").Replace(LibPaths.SepChar + Constants.OBJ_DIR, "").
                        Replace(LibPaths.SepChar + Constants.RELEASE_DIR, "").Replace(LibPaths.SepChar + Constants.DEBUG_DIR, "");
                    loadFileName += loadFileName.EndsWith(LibPaths.SepChar) ? "" : LibPaths.SepChar;
                    loadFileName += Constants.JSON_CONTACTS_FILE;
                }
                if (!File.Exists(loadFileName))
                {
                    loadFileName = AppDomain.CurrentDomain.BaseDirectory.
                        Replace(LibPaths.SepChar + Constants.BIN_DIR, "").Replace(LibPaths.SepChar + Constants.OBJ_DIR, "").
                        Replace(LibPaths.SepChar + Constants.RELEASE_DIR, "").Replace(LibPaths.SepChar + Constants.DEBUG_DIR, "");
                    loadFileName += loadFileName.EndsWith(LibPaths.SepChar) ? "" : LibPaths.SepChar;
                    loadFileName += Constants.JSON_CONTACTS_FILE;
                }
                return loadFileName;
            }
        }

        internal static HashSet<CContact> Contacts { get => GetContacts(); set => SetContacts(value); }

        /// <summary>
        /// Constructor for static class
        /// </summary>
        static JsonContacts()
        {
            _contacts = LoadJsonContacts();
            if (_contacts == null || _contacts.Count == 0)
                _contacts = new HashSet<CContact>(CqrSettings.Instance.Contacts);
        }

        #region json load save

        internal static HashSet<CContact> LoadJsonContacts()
        {
            lock (_lock)
            {
                if (!File.Exists(JsonContactsFile))
                    try
                    {
                        File.Create(JsonContactsFile);
                    }
                    catch (Exception exCreateJsonFile)
                    {
                        Area23Log.LogOriginMsgEx("JsonContacts", $"Exception when creating JsonContactsFile {JsonContactsFile}", exCreateJsonFile);
                    }
            }

            string jsonText = "";
            lock (_lock)
            {
                try
                {
                    jsonText = File.ReadAllText(JsonContactsFile);
                    _contacts = JsonConvert.DeserializeObject<HashSet<CContact>>(jsonText);
                }
                catch (Exception exReadJsonFile)
                {
                    Area23Log.LogOriginMsgEx("JsonContacts", $"Exception, when reading all text from JsonContactsFile {JsonContactsFile}", exReadJsonFile);
                }
            }

            if (_contacts == null || _contacts.Count == 0)
                _contacts = new HashSet<CContact>();

            return _contacts;
        }

        internal static void SaveJsonContacts()
        {
            _contacts = (_contacts == null || _contacts.Count == 0) ? new HashSet<CContact>() : _contacts;
            JsonSerializerSettings jsets = new JsonSerializerSettings();
            jsets.Formatting = Formatting.Indented;
            jsets.SerializationBinder = new Newtonsoft.Json.Serialization.DefaultSerializationBinder();
            jsets.MaxDepth = 16;

            lock (_lock)
            {
                string jsonString = JsonConvert.SerializeObject(_contacts, jsets);
                try
                {
                    File.WriteAllText(JsonContactsFile, jsonString);
                }
                catch (Exception exWriteJsonFile)
                {
                    Area23Log.LogOriginMsgEx("JsonContacts", $"Exception, when writing all serialized text to JsonContactsFile {JsonContactsFile}", exWriteJsonFile);
                }
            }
        }

        #endregion json load save

        #region get set

        internal static HashSet<CContact> GetContacts()
        {
            _contacts = (_contacts == null || _contacts.Count == 0) ? LoadJsonContacts() : _contacts;
            return _contacts;
        }

        internal static void SetContacts(HashSet<CContact> contacts)
        {
            _contacts = _contacts ?? new HashSet<CContact>();
            if (contacts != null && contacts.Count > 0 && contacts.Count > _contacts.Count)
                _contacts = contacts;
            SaveJsonContacts();
        }

        #endregion get set

        #region static helpers

        public static CContact FindContactByNameEmail(List<CContact> contacts, CContact searchContact)
        {
            CContact foundC = FindContactByNameEmail(contacts, searchContact.Name, searchContact.Email, searchContact.Mobile);
            return foundC;
        }

        public static CContact FindContactByNameEmail(List<CContact> contacts, string cName, string cEmail, string cMobile)
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
                                !string.IsNullOrEmpty(c.Mobile) &&
                                    c.Mobile.Equals(cPhone, StringComparison.CurrentCultureIgnoreCase) &&
                                    c.Name.Equals(cName, StringComparison.CurrentCultureIgnoreCase))
                        {
                            return c;
                        }
                    }
                }
            }

            return null;
        }

        [Obsolete("ChatRoomNumbersFromFs only exist in CqrSrv.CqrJd", true)]
        internal static List<string> ChatRoomNumbersFromFs()
        {

            List<string> chatRooms = new List<string>();
            string[] csr = Directory.GetFiles(LibPaths.SystemDirPath, "room*.json");
            string file = "";
            foreach (string filedir in csr)
            {
                file = Path.GetFileName(filedir);
                chatRooms.Add(file);
            }


            return chatRooms;
        }

        #endregion static helpers

    }
}