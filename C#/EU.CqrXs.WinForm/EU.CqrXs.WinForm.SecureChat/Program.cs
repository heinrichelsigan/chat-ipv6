using Area23.At.Framework.Core;
using Area23.At.Framework.Core.CqrXs;
using Area23.At.Framework.Core.CqrXs.CqrMsg;
using Area23.At.Framework.Core.CqrXs.CqrSrv;
using Area23.At.Framework.Core.Win32Api;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolTip;

namespace EU.CqrXs.WinForm.SecureChat
{

    /// <summary>
    /// Main Program entrance for <see cref="Controls.Forms.SecureChat"/> or <see cref="Controls.Forms.Peer2PeerChat"/>
    /// </summary>
    internal static class Program
    {
        internal static string progName = System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().Location);
        private static Mutex mutex = new Mutex(true, progName);
        static internal int mode = 0;
        static internal string startFormSwitch = string.Empty;

        internal static Mutex PMutec
        {
            get => mutex;
        }

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            if (PMutec == null)
                mutex = new Mutex(true, progName);

            if (!mutex.WaitOne(1000, false))
            {                
                NativeWrapper.Kernel32.AttachConsole(NativeWrapper.Kernel32.ATTACH_PARENT_PROCESS);
                // Area23.At.Framework.Library.Area23Log.Logger.LogOriginMsg(roachName, $"Another instance of {roachName} is already running!");
                Console.Out.WriteLine($"Another instance of {progName} is already running!");
                MessageBox.Show($"Another instance of {progName} is already running!", $"{progName}: multiple startup!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }

            if (args != null && args.Length > 0)
            {
                foreach (string arg in args)
                {
                    if (arg.ToLower().Contains("test"))
                        AppDomain.CurrentDomain.SetData(Constants.CQRXS_TEST_FORM, true);
                    if (arg.ToLower().Contains("fishonaes") || arg.ToLower().Contains("fish on aes"))
                        AppDomain.CurrentDomain.SetData(Constants.FISH_ON_AES_ENGINE, true);
                    if (arg.ToLower().Contains("peer"))
                        startFormSwitch = "peer";
                    if (arg.ToLower().Contains("rich"))
                        startFormSwitch = "rich";
                    if (arg.ToLower().Contains("secure"))
                        startFormSwitch = "secure";
                }
            }

            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.SetHighDpiMode(HighDpiMode.DpiUnawareGdiScaled);
            ApplicationConfiguration.Initialize();

            switch (startFormSwitch)
            {
                case "peer":
                    Controls.Forms.Peer2PeerChat peer2PeerChat = new Controls.Forms.Peer2PeerChat();
                    Application.Run(peer2PeerChat);
                    break;
                
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


            ReleaseCloseDisposeMutex(mutex);

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
                        Area23Log.LogStatic(exRelease);                        
                        CqrException.SetLastException(exRelease);
                        ex = exRelease;
                    }
                    try
                    {
                        mutex.Close();
                    }
                    catch (Exception exClose)
                    {
                        Area23Log.LogStatic(exClose);
                        CqrException.SetLastException(exClose);
                        ex = exClose;
                    }
                    try
                    {
                        mutex.Dispose();
                    }
                    catch (Exception exDispose)
                    {
                        Area23Log.LogStatic(exDispose);
                        CqrException.SetLastException(exDispose);
                        ex = exDispose;
                    }

                }
            }
            try
            {
                mutex = null;
            }
            catch (Exception exNull)
            {
                Area23Log.LogStatic(exNull);
                CqrException.SetLastException(exNull);
                ex = exNull;
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