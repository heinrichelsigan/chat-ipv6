using Area23.At.Framework.Core.CqrXs;
using Area23.At.Framework.Core.Static;
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

        public static Settings Singleton { get => (Settings)_instance.Value; }

        #endregion properties

        #region ctor Settings() Settings(DateTime timeStamp) => Load()

        /// <summary>
        /// Settings constructor maybe needed public for NewTonSoftJson serializing object
        /// </summary>
        public Settings() : base() { }

        public Settings(DateTime timeStamp) : base(timeStamp) { }

        #endregion ctor Settings() Settings(DateTime timeStamp) => Load()

        #region static members Load() Save(Settings? settings)

        /// <summary>
        /// loads json serialized Settings data string from 
        /// <see cref="LibPaths.AppDirPath"/> + <see cref="Constants.JSON_SAVE_FILE"/>
        /// and deserialize it to singleton instance <see cref="Settings"/> of <seealso cref="Lazy{Settings}"/>
        /// </summary>
        /// <returns>singelton <see cref="Settings.Instance"/></returns>
        public static Settings? LoadSettings()
        {
            string settingsJsonString = string.Empty;
            Settings? settings = null;
            string fileName = LibPaths.SystemDirPath + Constants.JSON_SETTINGS_FILE;
            try
            {
                if (!File.Exists(fileName) && Directory.Exists(LibPaths.AppPath))
                {
                    File.CreateText(fileName);
                }

                settingsJsonString = File.ReadAllText(fileName);
                settings = JsonConvert.DeserializeObject<Settings>(settingsJsonString);
            }
            catch (Exception ex)
            {
                CqrException.SetLastException(ex);
            }

            if (settings != null)
            {
                _instance.Value.Contacts = settings.Contacts;
                _instance.Value.MyContact = settings.MyContact;
                _instance.Value.FriendIPs = settings.FriendIPs;
                _instance.Value.MyIPs = settings.MyIPs;
                _instance.Value.Proxies = settings.Proxies;
                _instance.Value.TimeStamp = settings.TimeStamp;
                _instance.Value.SaveStamp = settings.SaveStamp;
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
        public static bool SaveSettings(Settings? settings)
        {
            string saveString = string.Empty;
            if (settings == null)
                settings = Settings.Singleton;
            try
            {
                settings.SaveStamp = DateTime.Now;
                saveString = JsonConvert.SerializeObject(settings);
                File.WriteAllText(LibPaths.SystemDirPath + Constants.JSON_SETTINGS_FILE, saveString);
            }
            catch (Exception ex)
            {                
                CqrException.SetLastException(ex);                
                return false;
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
                    _disposed = Settings.SaveSettings(Singleton);
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
