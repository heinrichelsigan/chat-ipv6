using Area23.At.Framework.Core.Cache;
using Area23.At.Framework.Core.Cqr;
using Area23.At.Framework.Core.Static;
using System.Reflection;
using System.Runtime.InteropServices;

namespace EU.CqrXs.WinForm.SecureChat
{

    /// <summary>
    /// Main Program entrance for <see cref="Controls.Forms.SecureChat"/> or <see cref="Controls.Forms.Peer2PeerChat"/>
    /// </summary>
    internal static class Program
    {

        /// <summary>
        /// Helper class containing kernel32 functions
        /// </summary>
        public class Kernel32
        {
            public const int ATTACH_PARENT_PROCESS = -1;

            /// <summary>
            /// AttachConsole to Windows Form App
            /// </summary>
            /// <param name="dwProcessId"></param>
            /// <returns></returns>
            [DllImport("kernel32.dll")]
            public static extern bool AttachConsole(int dwProcessId);
        }


        /// <summary>
        /// Helper class containing User32 API functions
        /// </summary>
        public class User32
        {

            public const int HT_CAPTION = 0x2;

            public const uint GW_HWNDFIRST = 0x000;
            public const uint GW_HWNDLAST = 0x001;
            public const uint GW_HWNDNEXT = 0x002;
            public const uint GW_HWNDPREV = 0x003;
            public const uint GW_OWNER = 0x004;
            public const uint GW_CHILD = 0x005;
            public const uint GW_ENABLEDPOPUP = 0x006;

            public const uint WM_PRINT = 0x317;
            public const int WM_NCLBUTTONDOWN = 0xA1;
            public const int WM_APPCOMMAND = 0x319;

            [StructLayout(LayoutKind.Sequential)]
            public struct RECT
            {
                internal int left;
                internal int top;
                internal int right;
                internal int bottom;
            }

            [Flags]
            public enum PRF_FLAGS : uint
            {
                CHECKVISIBLE = 0x01,
                CHILDREN = 0x02,
                CLIENT = 0x04,
                ERASEBKGND = 0x08,
                NONCLIENT = 0x10,
                OWNED = 0x20
            }


            [DllImport("user32.dll")]
            public static extern IntPtr GetDesktopWindow();


            [DllImport("user32.dll")]
            public static extern IntPtr GetWindowDC(IntPtr hWnd);

            [DllImport("user32.dll")]
            public static extern IntPtr GetWindowRect(IntPtr hWnd, ref RECT rect);

            [DllImport("user32.dll")]
            public static extern IntPtr GetTopWindow(IntPtr hWnd);

            [DllImport("user32.dll")]
            public static extern IntPtr GetWindow(IntPtr hWnd, uint uCmd);
            [DllImport("User32.dll")]
            public static extern int GetWindowDC(int hWnd);

            [System.Runtime.InteropServices.DllImportAttribute("user32.dll")]
            public static extern bool ReleaseCapture();

            [DllImport("user32.dll")]
            public static extern IntPtr ReleaseDC(IntPtr hWnd, IntPtr hDC);
            [DllImport("User32.dll")]
            public static extern int ReleaseDC(int hWnd, int hDC);


            [System.Runtime.InteropServices.DllImportAttribute("user32.dll")]
            public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

            [System.Runtime.InteropServices.DllImportAttribute("user32.dll")]
            public static extern IntPtr SendMessageW(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

            [DllImport("user32.dll")]
            public static extern IntPtr SendMessage(IntPtr hWnd, uint msg, IntPtr hdc, PRF_FLAGS drawingOptions);

        }


        internal static string progName = System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().Location);
        // internal static readonly string? progName = System.Environment.ProcessPath;
        internal static readonly string? progDirectory = Path.GetFullPath(System.Environment.ProcessPath);
        private static Mutex? _mutex = null;
        static internal int mode = 0;
        static internal string startFormSwitch = string.Empty;
        static internal bool firstRegistration = false;


        internal static Mutex? PMutec { get => _mutex; }

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
                Kernel32.AttachConsole(Kernel32.ATTACH_PARENT_PROCESS);
                // Area23.At.Framework.Library.Area23Log.LogOriginMsg(roachName, $"Another instance of {roachName} is already running!");
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
                        MemoryCache.CacheDict.SetValue<bool>(Constants.CQRXS_TEST_FORM, true);
                        // AppDomain.CurrentDomain.SetData(Constants.CQRXS_TEST_FORM, true);
                    if (arg.ToLower().Contains("fishonaes") || arg.ToLower().Contains("fish on aes"))
                        MemoryCache.CacheDict.SetValue<bool>(Constants.FISH_ON_AES_ENGINE, true);
                    if (arg.ToLower().Contains("rich"))
                        startFormSwitch = "rich";
                    if (arg.ToLower().Contains("secure"))
                        startFormSwitch = "secure";                    
                }
            }

            MemoryCache.CacheDict.SetValue<bool>(Constants.APP_FIRST_REG, firstRegistration);

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