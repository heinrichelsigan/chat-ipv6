namespace EU.CqrXs.Srv.SvcSwashbuckle.Logging
{
    public class EnablerLog
    {
        private static readonly Lazy<EnablerLog> instance = new Lazy<EnablerLog>(() => new EnablerLog());
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        /// <summary>
        /// LogFile
        /// </summary>
        public static string LogFile { get => Paths.LogPathFile; }

        /// <summary>
        /// LogLevel default Log Level 
        /// </summary>
        public static int LogLevel { get => Constants.LOG_LEVEL; }

        public static int DebugLevel { get => LogLevel; }

        /// <summary>
        /// Last LogDate
        /// </summary>
        public static DateTime LogDate { get; protected internal set; }

        /// <summary>
        /// <see cref="LoggingConfiguration">LogConfig</see> gets or sets <seealso cref="NLog.LogManager.Configuration"/> 
        /// </summary>
        public static LoggingConfiguration LogConfig
        {
            get => NLog.LogManager.Configuration;
            protected internal set => NLog.LogManager.Configuration = value;
        }

        /// <summary>
        /// Get the Logger
        /// </summary>
        public static ILogger Logger { get => logger; }

        /// <summary>
        /// static singelton accessor
        /// </summary>
        public static EnablerLog EnablerLogger { get => instance.Value; }

        #region staticLoggingsMethods

        /// <summary>
        /// LogStatic - static logger without EnablerLog.Logger singelton
        /// </summary>
        /// <param name="msg">message to log</param>
        public static void LogStatic(string msg)
        {
            string logMsg = string.Empty;
            if (!File.Exists(LogFile))
            {
                try
                {
                    File.Create(LogFile);
                }
                catch (Exception ex)
                {
                    EnablerLog.LogStatic(ex);
                }
            }
            try
            {
                logMsg = String.Format("{0} \t{1}\r\n",
                        Constants.DateArea23Seconds,
                        msg);
                File.AppendAllText(LogFile, logMsg);
            }
            catch (Exception exLog)
            {
                Console.WriteLine("Enabler.Lib.Logging.EnablerLog Exception:  Exception: " + exLog.ToString());
            }
        }

        /// <summary>
        /// LogStatic - static logger for Exception
        /// </summary>
        /// <param name="exLog"><see cref="Exception"/> to log</param>
        public static void LogStatic(Exception ex)
        {
            string exMsg = String.Format("Exception {0} ⇒ {1}\t{2}\t{3}",
                ex.GetType(),
                ex.Message,
                ex.ToString().Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " "),
                ex.StackTrace.Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " "));

            LogStatic(exMsg);
        }

        /// <summary>
        /// LogExStatic - log Exception with a static method
        /// </summary>
        /// <param name="module"><see cref="string"/> module for enabler4BIZ module</param>
        /// <param name="ex"><see cref="Exception"/> ex for thrown Exception</param>
        public static void LogExStatic(string module, Exception ex)
        {
            string exMsg = String.Format("{0} \t{1} ⇒ {2}\t{3}",
                module,
                ex.GetType(),
                ex.Message,
                ex.ToString().Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " "));

            LogStatic(exMsg);
        }

        /// <summary>
        /// WriteStatic - Write in a static method
        /// </summary>
        /// <param name="module"><see cref="string"/> module for enabler4BIZ module</param>
        /// <param name="msg"><see cref="string"/> msg for logging message</param>
        public static void WriteStatic(string module, string msg)
        {
            string logMsg = logMsg = String.Format("{0} \t{1}", module, msg);
            LogStatic(logMsg);
        }

        /// <summary>
        /// WriteExStatic - WriteEx as static method
        /// </summary>
        /// <param name="module"><see cref="string"/> module for enabler4BIZ module</param>
        /// <param name="msg"><see cref="string"/> msg for logging message</param>
        /// <param name="ex"><see cref="Exception"/> ex for thrown Exception</param>
        public static void WriteExStatic(string module, string msg, Exception ex)
        {
            string exMsg = String.Format("{0} \t{1} \t{2} ⇒ {3}\t{4}",
                module,
                msg,
                ex.GetType(),
                ex.Message,
                ex.ToString().Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " "));

            LogStatic(exMsg);
        }

        public static void Write(string module, string msg)
        {
            WriteStatic(module, msg);
        }
        public static void LogException(Exception ex)
        {
            LogStatic(ex);
        }
        public static void LogException(string module, Exception ex)
        {
            LogExStatic(module, ex);
        }

        #endregion staticLoggingsMethods

        /// <summary>
        /// private Singelton constructor
        /// </summary>
        internal EnablerLog()
        {
            LogDate = DateTime.MinValue;
            RefreshLogger();
        }

        /// <summary>
        /// RefreshLogger refreshed <see cref="LoggingConfiguration"/> of <see cref="NLog.Logger"/> 
        /// and sets <see cref="LoggingConfiguration"/> via static property <see cref="LogConfig"/>
        /// </summary>
        /// <param name="forceRefresh"><see cref="bool">bool forceRefresh</see> when true, forces to refresh logger
        /// when false, logger will be only refrehed at the beginning of a new day at midnight</param>
        internal void RefreshLogger(bool forceRefresh = false)
        {
            if ((LogDate.Day != DateTime.Now.Day) || forceRefresh)
            {
                LogDate = DateTime.Now;
                NLog.Config.LoggingConfiguration config = new NLog.Config.LoggingConfiguration();
                // Targets where to log to: File and Console
                NLog.Targets.Target logfile = new NLog.Targets.FileTarget("logfile") { FileName = LogFile };
                NLog.Targets.Target logconsole = new NLog.Targets.ConsoleTarget("logconsole");
                // Rules for mapping loggers to targets            
                config.AddRule(NLog.LogLevel.Trace, NLog.LogLevel.Trace, logconsole);
                config.AddRule(NLog.LogLevel.Debug, NLog.LogLevel.Debug, logfile);
                config.AddRule(NLog.LogLevel.Info, NLog.LogLevel.Info, logfile);
                config.AddRule(NLog.LogLevel.Warn, NLog.LogLevel.Warn, logfile);
                config.AddRule(NLog.LogLevel.Error, NLog.LogLevel.Error, logfile);
                config.AddRule(NLog.LogLevel.Fatal, NLog.LogLevel.Fatal, logfile);
                LogConfig = config; // Apply config
            }
        }

        /// <summary>
        /// Log logs message to NLog with <see cref="NLog.LogLevel"/> by <seealso cref="NLog.LogLevel.FromOrdinal(int)">NLog.LogLevel.FromOrdinal(LogLevel)</see>
        /// </summary>
        /// <param name="message">logging <see cref="string">string message</see></param>
        public void Log(string message)
        {
            NLog.LogLevel nlogLvl = NLog.LogLevel.FromOrdinal(LogLevel);
            logger.Log(nlogLvl, message);
        }

        /// <summary>
        /// Log logs message to NLog
        /// </summary>
        /// <param name="message">logging <see cref="string">string message</see></param>
        /// <param name="level"><see cref="int">int log level</see>: 0 for Trace, 1 for Debug, ..., 4 for Error, 5 for Fatal</param>
        public void Log(string message, int logLevel = 3)
        {
            RefreshLogger();
            NLog.LogLevel nlogLvl = NLog.LogLevel.FromOrdinal(logLevel);
            logger.Log(nlogLvl, message);
        }

        #region LogLevelLogger members

        public void LogDebug(string msg)
        {
            Log(msg, NLog.LogLevel.Debug.Ordinal);
        }

        public void LogInfo(string msg)
        {
            Log(msg, NLog.LogLevel.Info.Ordinal);
        }

        public void LogWarn(string msg)
        {
            Log(msg, NLog.LogLevel.Warn.Ordinal);
        }

        public void LogError(string msg)
        {
            Log(msg, NLog.LogLevel.Error.Ordinal);
        }

        #endregion LogLevelLogger members

        /// <summary>
        /// LogEx - logs Exception to NLog
        /// </summary>
        /// <param name="ex">logging <see cref="Exception">Exception ex</see></param>
        /// <param name="level"><see cref="int">int log level</see>: 0 for Trace, 1 for Debug, ..., 4 for Error, 5 for Fatal</param>
        public void LogEx(Exception ex, int level = 2)
        {
            Log($"{ex.GetType()}: \t{ex.Message}", level);
            if (level < 4)
                Log($"Exception {ex.GetType()}: \t{ex.ToString()}", level);
            if (level < 2)
                Log($"{ex.GetType()} StackTrace: \t{ex.StackTrace}", level);
        }

        /// <summary>
        /// LogEx logs message and ex to NLog
        /// </summary>
        /// <param name="message">logging <see cref="string">string message</see></param>
        /// <param name="ex">logging <see cref="Exception">Exception ex</see></param>
        /// <param name="level"><see cref="int">int log level</see>: 0 for Trace, 1 for Debug, ..., 4 for Error, 5 for Fatal</param>
        public void LogEx(string message, Exception ex, int level = 2)
        {
            string logPrefix = string.IsNullOrEmpty(message) ? " \t " : message;
            Log($"{logPrefix} \t{ex.GetType()}: \t{ex.Message}", level);
            if (level < 4)
                Log($"{logPrefix} \tException {ex.GetType()}: \t{ex.ToString()}", level);
            if (level < 2)
                Log($"{logPrefix} \t{ex.GetType()} StackTrace: \t{ex.StackTrace}", level);
        }

        /// <summary>
        /// Write writes enabler module with message to NLog
        /// </summary>
        /// <param name="module">enabler module</param>
        /// <param name="message">enabler message to log</param>
        /// <param name="logLevel">log level: 0 for Trace, 1 for Debug, ..., 4 for Error, 5 for Fatal</param>
        public void Write(string module, string message, int logLevel = 3)
        {
            string logMsg = (string.IsNullOrEmpty(module) ? "  \t" : module + " \t") + message;
            Log(logMsg, logLevel);
        }

        /// <summary>
        /// WriteEx writes enabler module with message and ex to NLog
        /// </summary>
        /// <param name="module">enabler module</param>
        /// <param name="message">logging <see cref="string">string message</see></param>
        /// <param name="ex">logging <see cref="Exception">Exception ex</see></param>
        /// <param name="level"><see cref="int">int log level</see>: 0 for Trace, 1 for Debug, ..., 4 for Error, 5 for Fatal</param>
        public void WriteEx(string module, string message, Exception ex, int level = 2)
        {
            string logPrefix = string.IsNullOrEmpty(module) ? "   " : module;
            Log($"{logPrefix} \t{message} {ex.GetType()}: \t{ex.Message}", level);
            if (level < 4)
                Log($"{logPrefix} \tException {ex.GetType()}: \t{ex.ToString()}", level);
            if (level < 2)
                Log($"{logPrefix} \t{ex.GetType()} StackTrace: \t{ex.StackTrace}", level);
        }

    }
}
