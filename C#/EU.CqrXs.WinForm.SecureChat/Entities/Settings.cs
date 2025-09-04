using Area23.At.Framework.Core.Cqr;
using Area23.At.Framework.Core.Cqr.Msg;
using Area23.At.Framework.Core.Crypt.Cipher;
using Area23.At.Framework.Core.Crypt.Cipher.Symmetric;
using Area23.At.Framework.Core.Crypt.EnDeCoding;
using Area23.At.Framework.Core.Crypt.Hash;
using Area23.At.Framework.Core.Static;
using Area23.At.Framework.Core.Zfx;
using Newtonsoft.Json;

namespace EU.CqrXs.WinForm.SecureChat.Entities
{

    [Serializable]
    public class Settings : CqrSettings, IDisposable
    {
        protected new static readonly Lazy<Settings> _instance =
            new Lazy<Settings>(() => new Settings());

        private static bool _disposed = false;
        private static bool _destructed = false;

        #region properties

        [JsonIgnore]
        public static Settings Singleton { get => (Settings)_instance.Value; }

        public bool ClearAllOnClose { get; set; }

        public bool OnlyPeer2PeerChat { get; set; }

        public bool ZipBeforeSend { get; set; }

        public bool OnlySecureFileTypes { get; set; }

        public bool RegisterUser {  get; set; } 

        public List<string> SecretKeysCrypted { get; set; }

        public CChatRoom? ChatRoom { get; set; }

        #endregion properties

        #region ctor Settings() Settings(DateTime timeStamp) => Load()

        /// <summary>
        /// Settings constructor maybe needed public for NewTonSoftJson serializing object
        /// </summary>
        public Settings() : base()
        {
            SecretKeysCrypted = new List<string>();
            ClearAllOnClose = false;
            OnlyPeer2PeerChat = false;
            ZipBeforeSend = false;
            OnlySecureFileTypes = false;
            RegisterUser = false;
            ChatRoom = null;
        }

        public Settings(DateTime timeStamp) : this()
        {
            TimeStamp = timeStamp;            
            ClearAllOnClose = false;
            OnlyPeer2PeerChat = false;
        }

        #endregion ctor Settings() Settings(DateTime timeStamp) => Load()

        #region member functions

        protected virtual void Load() => LoadSettings(LibPaths.SystemDirPath + Constants.JSON_SETTINGS_FILE);

        protected virtual bool Save() => SaveSettings(this, false, LibPaths.SystemDirPath + Constants.JSON_SETTINGS_FILE);

        #endregion member functions

        #region static members Load() Save(Settings? settings)

        /// <summary>
        /// loads json serialized Settings data string from 
        /// <see cref="LibPaths.AppDirPath"/> + <see cref="Constants.JSON_SAVE_FILE"/>
        /// and deserialize it to singleton instance <see cref="Settings"/> of <seealso cref="Lazy{Settings}"/>
        /// </summary>
        /// <returns>singelton <see cref="Settings.Instance"/></returns>
        public new static Settings? LoadSettings(string? jsonFileName = null, bool forceCreateNewJsonFile = true)
        {            

            string settingsJsonString = string.Empty;
            Settings? settings = null;
            jsonFileName = jsonFileName ?? LibPaths.SystemDirPath + Constants.JSON_SETTINGS_FILE;
            try
            {
                if (Directory.Exists(LibPaths.SystemDirPath) && !File.Exists(jsonFileName) && forceCreateNewJsonFile)
                {
                    File.CreateText(jsonFileName);
                }

                settingsJsonString = File.ReadAllText(jsonFileName);
                settings = JsonConvert.DeserializeObject<Settings>(settingsJsonString);
            }
            catch (Exception ex)
            {
                CqrException.SetLastException(ex);
            }

            if (settings != null)
            {
                _instance.Value.Contacts = settings.Contacts;
                if (_instance.Value.Contacts == null || _instance.Value.Contacts.Count == 0)
                    _instance.Value.Contacts = new List<CContact>(JsonContacts.LoadJsonContacts());
                _instance.Value.MyContact = settings.MyContact;
                _instance.Value.FriendIPs = settings.FriendIPs;
                _instance.Value.MyIPs = settings.MyIPs;
                _instance.Value.Proxies = settings.Proxies;
                _instance.Value.TimeStamp = settings.TimeStamp;
                _instance.Value.SaveStamp = settings.SaveStamp;
                _instance.Value.ClearAllOnClose = settings.ClearAllOnClose;
                _instance.Value.OnlyPeer2PeerChat = settings.OnlyPeer2PeerChat;
                _instance.Value.ZipBeforeSend = settings.ZipBeforeSend;
                _instance.Value.OnlySecureFileTypes = settings.OnlySecureFileTypes;
                _instance.Value.SecretKeysCrypted = new List<string>(settings.SecretKeysCrypted);
                _instance.Value.ChatRoom = settings.ChatRoom;
                _instance.Value.SecretKeys = new List<string>();

                KeyHash keyHash = KeyHash.BCrypt;
                CipherPipe cPipe = new CipherPipe(Constants.AES_KEY);

                foreach (string skey in settings.SecretKeysCrypted)
                {
                    string secretKey = cPipe.DecryptTextRoundsGo(skey, Constants.AES_KEY, Constants.AES_IV, EncodingType.Base64, ZipType.None, keyHash);                 
                    if (!_instance.Value.SecretKeys.Contains(secretKey))
                        _instance.Value.SecretKeys.Add(secretKey);
                }
            }
            return settings;
        }

        /// <summary>
        /// json serializes <see cref="Settings"/> and 
        /// saves json serialized data string to 
        /// <see cref="LibPaths.AppDirPath"/> + <see cref="Constants.JSON_SAVE_FILE"/>
        /// </summary>
        /// <param name="settings">settings to save</param>
        /// <returns>true on successfully save</returns>
        public static bool SaveSettings(Settings? settings = null, bool forceSave = false, string ? jsonFileName = null)
        {
            string saveString = string.Empty;
            jsonFileName = jsonFileName ?? LibPaths.SystemDirPath + Constants.JSON_SETTINGS_FILE;
            if (settings == null)
                settings = Settings.Singleton;
            
            JsonSerializerSettings jsets = new JsonSerializerSettings();
            jsets.Formatting = Formatting.Indented;
            jsets.SerializationBinder = new Newtonsoft.Json.Serialization.DefaultSerializationBinder();
            jsets.MaxDepth = 16;

            lock (_lock)
            {
                DateTime lastSaved = (Settings.Singleton.SaveStamp == null || Settings.Singleton.SaveStamp < DateTime.Today) ? DateTime.Now : Settings.Singleton.SaveStamp;
                if (settings.SaveStamp != null && !forceSave && DateTime.Now.Subtract(lastSaved).TotalSeconds <= 2)
                    return true;

                settings.SecretKeys = (settings.SecretKeys != null) ? settings.SecretKeys : new List<string>();
                settings.SecretKeysCrypted = new List<string>();

                KeyHash keyHash = KeyHash.BCrypt;
                CipherPipe cPipe = new CipherPipe(Constants.AES_KEY);

                foreach (string plainKey in settings.SecretKeys)
                {
                    if (!string.IsNullOrEmpty(plainKey))
                    {
                        string cryptedKey = cPipe.EncrpytTextGoRounds(plainKey, Constants.AES_KEY, Constants.AES_IV, EncodingType.Base64, ZipType.None, keyHash);
                        if (!_instance.Value.SecretKeysCrypted.Contains(cryptedKey))
                            _instance.Value.SecretKeysCrypted.Add(cryptedKey);                                             
                    }
                }

                try
                {
                    JsonContacts.SetContacts(new HashSet<CContact>(settings.Contacts));
                    string moveFile = Path.Combine(
                        LibPaths.SystemDirSecureChatFilesPath,
                        DateTime.Now.ToString("yyyy-MM-dd_") + Constants.JSON_SETTINGS_FILE);                    
                    File.Move(jsonFileName, moveFile, true);

                    settings.SaveStamp = DateTime.Now;                    
                    saveString = JsonConvert.SerializeObject(settings, jsets);                    
                    File.WriteAllText(jsonFileName, saveString);
                }
                catch (Exception ex)
                {
                    CqrException.SetLastException(ex);
                    return false;
                }
            }

            return true;
        }

        #endregion static members Load() Save(Settings? settings)

        #region Dispose() Dispose(bool disposing) ~Settings()

        public void Dispose()
        {
            this.Dispose(false);
        }

        public bool Dispose(bool disposing)
        {
            if (!_disposed || disposing)
            {
                lock (_lock)
                {
                    _destructed = true;
                    _disposed = Settings.SaveSettings(Singleton, true);
                }
            }

            return _disposed;
        }

        //~Settings()
        //{
        //    if (!_destructed)
        //    {
        //        if (!Instance.Dispose(true))
        //        {
        //            string fileName = LibPaths.SystemDirPath + Constants.JSON_SAVE_FILE;
        //            throw new CqrException($"~Settings() couldn't save settings to to {fileName}.", CqrException.LastException);
        //        }
        //    }
        //    TimeStamp = DateTime.Now;
        //    _disposed = true;
        //    SaveStamp = null;
        //    this.Contacts.Clear();
        //    this.FriendIPs.Clear();
        //    this.Proxies.Clear();
        //    this.MyIPs.Clear();
        //    // TODO: Only way to destruct a singelton is to set _instance Lazy<T> to null
        //    // think about the risk, that reflection could change a private static non readonly field
        //    // so I decided to let the GC handle this
        //    // _instance = null;            
        //}

        #endregion Dispose() Dispose(bool disposing) ~Settings()

    }

}
