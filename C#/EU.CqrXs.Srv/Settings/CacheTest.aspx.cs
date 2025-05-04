using Area23.At.Framework.Library;
using Area23.At.Framework.Library.Cache;
using Area23.At.Framework.Library.Static;
using Area23.At.Framework.Library.Util;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Web;
using System.Web.UI.WebControls;
using System.Windows.Input;

namespace EU.CqrXs.Srv.Settings
{

    public partial class CacheTest : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            BuildTests();
            BuildSettingsTable();
            BuildTableRuntime();

        }

        protected void BuildSettingsTable()
        {
            Dictionary<string, string> settings = new Dictionary<string, string>();
            try
            {
                settings.Add("APP_DIR", Constants.APP_DIR);
                settings.Add("AppPath", LibPaths.AppPath);
                settings.Add("BaseAppPath", LibPaths.BaseAppPath);
                settings.Add("ResAppPath", LibPaths.ResAppPath);
                settings.Add("CssAppPath", LibPaths.CssAppPath);
                settings.Add("OutAppPath", LibPaths.OutAppPath);
                settings.Add("SystemDirPath", LibPaths.SystemDirPath);
                settings.Add("SystemDirJsonPath", LibPaths.SystemDirJsonPath);
                settings.Add("SystemDirLogPath", LibPaths.SystemDirLogPath);
                settings.Add("LogFileSystemPath", LibPaths.LogFileSystemPath);
                settings.Add("AppLogFile", Constants.AppLogFile);
                settings.Add("SLog.LogFile", SLog.LogFile);
                settings.Add("UserHostAddress", Request.UserHostAddress);
                settings.Add("RawUrl", Request.RawUrl);
                settings.Add("UserAgent", Request.UserAgent);
            }
            catch (Exception ex0)
            {
                DivTest0.InnerHtml = $"<p>Exception ${ex0.GetType()} {ex0.Message}</p><pre>\n{ex0.ToString()}</pre>";
            }
            try
            {
                settings.Add("PersistType.AppDomain Parallel", RunTasks(PersistType.AppDomain, 64));               
                settings.Add("PersistType.AppDomain Serial", RunSerial(PersistType.AppDomain, 64));            
                settings.Add("PersistType.ApplicationState Parallel", RunTasks(PersistType.ApplicationState, 64));
                settings.Add("PersistType.ApplicationState Serial", RunSerial(PersistType.ApplicationState, 64));
            }
            catch (Exception ex0)
            {
                DivTest0.InnerHtml = $"<p>Exception ${ex0.GetType()} {ex0.Message}</p><pre>\n{ex0.ToString()}</pre>";
            }
            try
            {
                settings.Add("PersistType.JsonFile Parallel", RunTasks(PersistType.JsonFile, 64));
                settings.Add("PersistType.JsonFile Serial", RunSerial(PersistType.JsonFile, 64));
            }
            catch (Exception ex1)
            {
                DivTest1.InnerHtml = $"<p>Exception ${ex1.GetType()} {ex1.Message}</p><pre>\n{ex1.ToString()}</pre>";
            }
            try
            {

                settings.Add("PersistType.Redis Parallel", RunTasks(PersistType.Redis, 64));
                settings.Add("PersistType.Redis Serial", RunSerial(PersistType.Redis, 64));
            }
            catch (Exception ex2)
            {
                DivTest2.InnerHtml = $"<p>Exception ${ex2.GetType()} {ex2.Message}</p><pre>\n{ex2.ToString()}</pre>";
            }

            try
            {
                foreach (string key in settings.Keys)
                {
                    TableRow row = new TableRow();
                    TableCell cellName = new TableCell();
                    cellName.Text = key.ToString();
                    TableCell cellValue = new TableCell();
                    cellValue.Text = settings[key].ToString();
                    row.Cells.Add(cellName);
                    row.Cells.Add(cellValue);
                    TableSettings.Rows.Add(row);
                }
            }
            catch (Exception ex3)
            {
                DivTest2.InnerHtml += $"<p>Exception ${ex3.GetType()} {ex3.Message}</p><pre>\n{ex3.ToString()}</pre>";
            }

        }


        protected void BuildTableRuntime()
        {

            Dictionary<string, string> rtSettings = new Dictionary<string, string>();

            long lenSess = 0;
            rtSettings.Add("SesionID", HttpContext.Current.Session.SessionID.ToString());
            rtSettings.Add("Session state all items count ", HttpContext.Current.Session.Keys.Count.ToString());
            for (int i = 0; i < HttpContext.Current.Session.Count; i++)
            {
                string skey = HttpContext.Current.Session.Keys[i].ToString();
                string sv = HttpContext.Current.Session[skey].ToString();
                lenSess += sv.Length;
                rtSettings.Add(skey, sv);
            }
            rtSettings.Add("Session state all items size ", lenSess.ToString());

            long lenApp = 0;
            rtSettings.Add("Application state all items count ", HttpContext.Current.Application.AllKeys.Length.ToString());
            foreach (string akey in HttpContext.Current.Application.AllKeys)
            {
                string av = HttpContext.Current.Application[akey].ToString();
                lenApp += av.Length;
                rtSettings.Add(akey, av);
            }
            rtSettings.Add("Application state all items size ", lenApp.ToString());

            rtSettings.Add("Headers count", HttpContext.Current.Request.Headers.AllKeys.Length.ToString());
            foreach (string hkey in HttpContext.Current.Request.Headers.Keys)
            {
                string hval = HttpContext.Current.Request.Headers[hkey].ToString();
                rtSettings.Add(hkey, hval);
            }

            rtSettings.Add("Cookies count ", HttpContext.Current.Request.Cookies.AllKeys.Length.ToString());
            foreach (string ckey in HttpContext.Current.Request.Cookies.AllKeys)
            {
                string cval = HttpContext.Current.Request.Cookies[ckey].ToString();
                rtSettings.Add(ckey, cval);
            }

            rtSettings.Add("AppSettings keys count ", ConfigurationManager.AppSettings.AllKeys.Length.ToString());
            foreach (string appSetKey in ConfigurationManager.AppSettings.AllKeys)
            {
                string appSetVal = ConfigurationManager.AppSettings[appSetKey].ToString();
                rtSettings.Add(appSetKey, appSetVal);
            }

            foreach (string rtKey in rtSettings.Keys)
            {
                TableRow row = new TableRow();
                TableCell cellName = new TableCell();
                cellName.Text = rtKey.ToString();
                TableCell cellValue = new TableCell();
                cellValue.Text = rtSettings[rtKey].ToString();
                row.Cells.Add(cellName);
                row.Cells.Add(cellValue);
                TableRuntime.Rows.Add(row);
            }
        }


        protected void BuildTests()
        {
            try
            {
                Area23Log.LogStatic("Log test from " + Request.UserHostAddress + " " + Request.UserAgent);
                DivTest1.InnerHtml = $"<p>{DateTime.Now.Area23DateTimeWithMillis()} LogStatic to {SLog.LogFile} successfull!</p>";
            }
            catch (Exception ex1)
            {
                DivTest1.InnerHtml = $"<p>Exception ${ex1.GetType()} {ex1.Message}</p><pre>\n{ex1.ToString()}</pre>";
            }
            try
            {
                Area23Log.LogStatic("Log test from " + Request.UserHostAddress + " " + Request.UserAgent);
                DivTest2.InnerHtml = $"<p>{DateTime.Now.Area23DateTimeWithMillis()} Logger.Log to {SLog.LogFile} successfull!</p>";
            }
            catch (Exception ex2)
            {
                DivTest1.InnerHtml = $"<p>Exception ${ex2.GetType()} {ex2.Message}</p><pre>\n{ex2.ToString()}</pre>";
            }
        }
              

        static string RunTasks(PersistType persitVariant, int numberOfTasks, short maxKexs = 16)
        {
            MemoryCache memoryCache = null;
            switch (persitVariant)
            {
                case PersistType.JsonFile:
                    JsonFileCache jsonFileCache = new JsonFileCache(persitVariant);
                    memoryCache = (MemoryCache)jsonFileCache;
                    break;
                case PersistType.ApplicationState:
                    ApplicationStateCache appStateCache = new ApplicationStateCache(persitVariant);
                    memoryCache = (MemoryCache)appStateCache;
                    break;
                case PersistType.Redis:
                    RedisCache redisCache = new RedisCache(persitVariant);
                    memoryCache = (MemoryCache)redisCache;
                    break;
                case PersistType.AppDomain:
                default:
                    AppDomainCache appDomainCache = new AppDomainCache(persitVariant);
                    memoryCache = (MemoryCache)appDomainCache;
                    break;
            }
            string parallelCache = MemoryCache.CacheVariant;
            System.Console.WriteLine($"RunTasks(int numberOfTasks = {numberOfTasks}) cache = {parallelCache}.");
            DateTime now = DateTime.Now;
            if (numberOfTasks <= 0)
                numberOfTasks = 16;
            if ((numberOfTasks % 4) != 0)
                numberOfTasks += (4 - (numberOfTasks % 4));

            int quater = numberOfTasks / 4;
            int half = numberOfTasks / 2;
            int threequater = quater + half;

            Task[] taskArray = new Task[numberOfTasks];
            for (int i = 0; i < numberOfTasks; i++)
            {
                if (i < quater || (i >= half && i < threequater))
                {
                    taskArray[i] = Task.Factory.StartNew((object obj) =>
                    {
                        string ckey = string.Concat("Key_", (i % maxKexs).ToString());
                        CacheData data = obj as CacheData;
                        if (data == null)
                            data = new CacheData(ckey, Thread.CurrentThread.ManagedThreadId);

                        data.CThreadId = Thread.CurrentThread.ManagedThreadId;
                        MemoryCache.CacheDict.SetValue<CacheData>(ckey, data);
                        // Console.WriteLine($"Task set cache key #{data.CKey} created at {data.CTime} on thread #{data.CThreadId}.");
                    },
                    new CacheData("Key_" + (i % maxKexs).ToString()));
                }
                else if ((i >= quater && i < half) || i >= threequater)
                {
                    taskArray[i] = Task.Factory.StartNew((object obj) =>
                    {
                        string ckey = string.Concat("Key_", (i % maxKexs).ToString());
                        string strkey = obj as string;
                        if (string.IsNullOrEmpty(strkey))
                            strkey = ckey;

                        CacheData data = (CacheData)MemoryCache.CacheDict[strkey];
                        // Console.WriteLine($"Task get cache key #{strkey} => {data.CValue} created at {data.CTime} original thread {data.CThreadId} on current thread #{Thread.CurrentThread.ManagedThreadId}.");
                    },
                    new StringBuilder(string.Concat("Key_", (i % maxKexs).ToString())).ToString());
                }
            }

            Task.WaitAll(taskArray);

            TimeSpan ts = DateTime.Now.Subtract(now);
            double doublePerSecond = numberOfTasks / ts.TotalSeconds;
            if (numberOfTasks > ts.TotalSeconds)
                doublePerSecond = (1000 * numberOfTasks) / ts.TotalMilliseconds;
            ulong perSecond = (ulong)doublePerSecond;
            
            return $"Finished {numberOfTasks} parallel tasks in {ts.Minutes:d2}:{ts.Seconds:d2}.{ts.Milliseconds:d3}\n\t{perSecond} tasks per second.";
        }

        static string RunSerial(PersistType persitVariant, int iterationsCount, short maxKexs = 16)
        {
            MemoryCache memoryCache = null;
            switch (persitVariant)
            {                
                case PersistType.JsonFile:
                    JsonFileCache jsonFileCache = new JsonFileCache(persitVariant);
                    memoryCache = (MemoryCache)jsonFileCache;
                    break;
                case PersistType.ApplicationState:
                    ApplicationStateCache appStateCache = new ApplicationStateCache(persitVariant);
                    memoryCache = (MemoryCache)appStateCache;
                    break;
                case PersistType.Redis:
                    RedisCache redisCache = new RedisCache(persitVariant);
                    memoryCache = (MemoryCache)redisCache;
                    break;
                case PersistType.AppDomain:
                default:
                    AppDomainCache appDomainCache = new AppDomainCache(persitVariant);
                    memoryCache = (MemoryCache)appDomainCache;
                    break;
            }
            string serialSache = MemoryCache.CacheVariant;
            System.Console.WriteLine($"RunSerial(int iterationsCount = {iterationsCount}) cache = {serialSache}.");

            if (iterationsCount <= 0)
                iterationsCount = 16;
            if ((iterationsCount % 4) != 0)
                iterationsCount += (4 - (iterationsCount % 4));
            int quater = iterationsCount / 4;
            int half = iterationsCount / 2;
            int threequater = quater + half;

            DateTime now = DateTime.Now;
            for (int i = 0; i < iterationsCount; i++)
            {
                if (i < quater || (i >= half && i < threequater))
                {
                    string ckey = string.Concat("Key_", (i % maxKexs).ToString());
                    CacheData data = new CacheData(ckey, Thread.CurrentThread.ManagedThreadId);
                    MemoryCache.CacheDict.SetValue<CacheData>(ckey, data);
                    // Console.WriteLine($"Task set cache key #{data.CKey} created at {data.CTime} on thread #{data.CThreadId}.");
                }
                else if ((i >= quater && i < half) || i >= threequater)
                {
                    string strkey = "Key_" + (i % maxKexs).ToString();
                    CacheData cacheData = (CacheData)MemoryCache.CacheDict[strkey];
                    // Console.WriteLine($"Task get cache key #{strkey} => {cacheData.CValue} created at {cacheData.CTime} original thread {cacheData.CThreadId} on current thread #{Thread.CurrentThread.ManagedThreadId}.");
                }
            }

            // var tasks = new List<Task>(taskArray);            
            // Parallel.ForEach(tasks, task => { task.Start(); });
            //Task.WhenAll(tasks).ContinueWith(done => { Console.WriteLine("done"); });

            TimeSpan ts = DateTime.Now.Subtract(now);
            double doublePerSecond = iterationsCount / ts.TotalSeconds;
            if (iterationsCount > ts.TotalSeconds)
                doublePerSecond = (1000 * iterationsCount) / ts.TotalMilliseconds;
            ulong perSecond = (ulong)doublePerSecond;
            
            return $"Finished {iterationsCount} iterations in {ts.Minutes:d2}:{ts.Seconds:d2}.{ts.Milliseconds:d3}\n\t{perSecond} iterations per second.";

        }

    }

}