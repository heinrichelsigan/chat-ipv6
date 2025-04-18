namespace EU.CqrXs.Srv.SvcSwashbuckle.Logging
{
    public class Paths
    {
        private static string appPath = null;
        private static string baseAppPath = null;
        private static string appDirPath = null;
        private static string outDirPath = null;
        private static string resDirPath = null;

        public static string SepChar { get => Path.DirectorySeparatorChar.ToString(); }

        public static string AppPath
        {
            get
            {
                if (String.IsNullOrEmpty(appPath))
                {
                    try
                    {
                        if (System.Configuration.ConfigurationManager.AppSettings["AppDir"] != null)
                            appPath = System.Configuration.ConfigurationManager.AppSettings["AppDir"];
                    }
                    catch (Exception appFolderEx)
                    {
                        EnablerLog.LogStatic(appFolderEx);
                    }
                    if (String.IsNullOrEmpty(appPath))
                        appPath = Constants.APP_DIR;
                }
                return appPath;
            }
        }


        public static string AppDirPath
        {
            get
            {
                if (String.IsNullOrEmpty(appDirPath))
                {
                    appDirPath = "." + SepChar;

                    if (AppContext.BaseDirectory != null)
                        appDirPath = AppContext.BaseDirectory + SepChar;
                    else if (AppDomain.CurrentDomain != null)
                        appDirPath = AppDomain.CurrentDomain.BaseDirectory + SepChar;
                }

                return appDirPath;
            }
        }

        public static string BaseAppPath
        {
            get
            {
                if (String.IsNullOrEmpty(baseAppPath))
                {
                    string basApPath = HttpContext.Current.Request.Url.ToString().
                        Replace("/Util/", "/").Replace("/UpdateService/", "/").Replace("/Logging/", "/").
                        Replace("/css/", "/").Replace("/js/", "/").Replace("/img/", "/").
                        Replace("/out/", "/");
                    baseAppPath = basApPath.Substring(0, basApPath.LastIndexOf("/"));
                    if (!baseAppPath.EndsWith("/"))
                        baseAppPath += "/";
                }
                return baseAppPath;
            }
        }


        public static string LogPathDir
        {
            get
            {
                string logPath = AppDirPath;

                if (!logPath.Contains(Constants.LOG_DIR))
                    logPath += Constants.LOG_DIR + SepChar;

                if (!Directory.Exists(logPath))
                {
                    string dirNotFoundMsg = String.Format("{0} directory {1} doesn't exist, creating it!", Constants.LOG_DIR, logPath);
                    // EnablerLog.LogStatic(dirNotFoundMsg);
                    Directory.CreateDirectory(logPath);
                }
                return logPath;
            }
        }

        public static string LogPathFile { get => LogPathDir + Constants.AppLogFile; }


        public static string OutDirPath
        {
            get
            {
                if (String.IsNullOrEmpty(outDirPath))
                {
                    outDirPath = AppDirPath;
                    if (!outDirPath.Contains(Constants.OUT_DIR))
                        outDirPath += Constants.OUT_DIR + SepChar;

                    if (!Directory.Exists(outDirPath))
                    {
                        try
                        {
                            string dirNotFoundMsg = String.Format("out directory {0} doesn't exist, creating it!", outDirPath);
                            EnablerLog.LogStatic(dirNotFoundMsg);
                            Directory.CreateDirectory(outDirPath);
                        }
                        catch (Exception ex)
                        {
                            EnablerLog.LogStatic(ex);
                        }
                    }
                }
                return outDirPath;
            }
        }

    }
}
