using Area23.At.Framework.Core.Cache;
using Area23.At.Framework.Core.Cqr;
using Area23.At.Framework.Core.CqrXs;
using Area23.At.Framework.Core.Static;
using Area23.At.Framework.Core.Win32Api;
using System.Reflection;

namespace EU.CqrXs.WinForm.SecureChat
{

    /// <summary>
    /// Main Program entrance for <see cref="Controls.Forms.SecureChat"/> or <see cref="Controls.Forms.Peer2PeerChat"/>
    /// </summary>
    internal static class Program
    {
        internal static string progName = System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().Location);
        // internal static readonly string? progName = System.Environment.ProcessPath;
        internal static readonly string? progDirectory = Path.GetFullPath(System.Environment.ProcessPath);
        private static Mutex? _mutex = null;
        static internal int mode = 0;
        static internal string startFormSwitch = string.Empty;
        static internal bool firstRegistration = false;


        internal static Mutex PMutec { get => _mutex; }

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            if (_mutex == null)
                _mutex = new Mutex(true, progName);

            if (!_mutex.WaitOne(1000, false))
            {                
                NativeWrapper.Kernel32.AttachConsole(NativeWrapper.Kernel32.ATTACH_PARENT_PROCESS);
                // Area23.At.Framework.Library.Area23Log.Logger.LogOriginMsg(roachName, $"Another instance of {roachName} is already running!");
                Console.Error.WriteLine($"Another instance of {progName} is already running!");
                MessageBox.Show($"Another instance of {progName} is already running!", $"{progName}: multiple startup!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }

            if (args != null && args.Length > 0)
            {
                foreach (string arg in args)
                {
                    if (arg.ToLower().Contains("nolog"))
                        Constants.NOLog = true;
                    if (arg.ToLower().Contains("firstreg") || arg.ToLower().Contains("1streg"))
                        firstRegistration = true;                        
                    if (arg.ToLower().Contains("dir") &&
                        (arg.ToLower().Contains("no") || arg.ToLower().Contains("false")))
                        Constants.DirCreate = false;
                    if (arg.ToLower().Contains("test"))
                        AppHashTable.SetValue<bool>(Constants.CQRXS_TEST_FORM, true);
                        // AppDomain.CurrentDomain.SetData(Constants.CQRXS_TEST_FORM, true);
                    if (arg.ToLower().Contains("fishonaes") || arg.ToLower().Contains("fish on aes"))
                        AppHashTable.SetValue<bool>(Constants.FISH_ON_AES_ENGINE, true);
                    if (arg.ToLower().Contains("rich"))
                        startFormSwitch = "rich";
                    if (arg.ToLower().Contains("secure"))
                        startFormSwitch = "secure";                    
                }
            }

            AppHashTable.SetValue<bool>(Constants.APP_FIRST_REG, firstRegistration);

            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.SetHighDpiMode(HighDpiMode.DpiUnawareGdiScaled);
            ApplicationConfiguration.Initialize();

            switch (startFormSwitch)
            {              
                case "rich":                       
                    Controls.Forms.RichTextChat richTextChat = new Controls.Forms.RichTextChat();
                    Application.Run(richTextChat);
                    break;

                case "secure":
                default:
                    Controls.Forms.SecureChat secureChat = new Controls.Forms.SecureChat();
                    Application.Run(secureChat);
                    break;
            }


            ReleaseCloseDisposeMutex(_mutex);

        }

        /// <summary>
        /// Release Mutax exclusion, that not 2 chat programs could be started at same machine
        /// </summary>
        /// <param name="mutex"></param>
        internal static void ReleaseCloseDisposeMutex(Mutex? mutex)
        {
            Exception? ex = null;            
            
            if (mutex != null)
            {
                var safeWaitHandle = mutex.GetSafeWaitHandle();
                if (safeWaitHandle != null && !safeWaitHandle.IsInvalid && !safeWaitHandle.IsClosed)
                {
                    try
                    {
                        mutex.ReleaseMutex();
                    }
                    catch (Exception exRelease)
                    {
                        ex = new CqrException("Releasing Mutex failed", exRelease);
                        CqrException.SetLastException(ex);
                    }
                    try
                    {
                        mutex.Close();
                    }
                    catch (Exception exClose)
                    {
                        ex = new CqrException("Closing Mutex failed", exClose);
                        CqrException.SetLastException(ex);
                    }
                    try
                    {
                        mutex.Dispose();
                    }
                    catch (Exception exDispose)
                    {
                        ex = new CqrException("Disposing Mutex failed", exDispose);
                        CqrException.SetLastException(ex);
                    }

                }
            }
            try
            {
                mutex = null;
            }
            catch (Exception exNull)
            {
                ex = new CqrException("Setting Mutex to null failed", exNull);
                CqrException.SetLastException(ex);
            }
            finally
            {                
                if (ex != null)
                {                    
                    throw ex;
                }                
            }

            return ;
        }

    }

}