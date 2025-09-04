using Area23.At.Framework.Library;
using Area23.At.Framework.Library.Cache;
using Area23.At.Framework.Library.Cqr;
using Area23.At.Framework.Library.Static;
using Area23.At.Framework.Library.Util;
using Newtonsoft.Json;
using System;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace EU.CqrXs.Console.Net48
{

    internal class Program
    {
        static void Main(string[] args)
        {
            RunTasks(256);
            RunSerial(256);

            System.Console.WriteLine($"\nPress any key to continue...\n");
            System.Console.ReadKey();
        }

        static void RunTasks(int numberOfTasks, short maxKexs = 16)
        {
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
                        CacheTestData data = obj as CacheTestData;
                        if (data == null)
                            data = new CacheTestData(ckey, Thread.CurrentThread.ManagedThreadId);

                        data.CThreadId = Thread.CurrentThread.ManagedThreadId;
                        MemoryCache.CacheDict.SetValue<CacheTestData>(ckey, data);
                        // Console.WriteLine($"Task set cache key #{data.CKey} created at {data.CTime} on thread #{data.CThreadId}.");
                    },
                    new CacheTestData("Key_" + (i % maxKexs).ToString()));
                }
                else if ((i >= quater && i < half) || i >= threequater)
                {
                    taskArray[i] = Task.Factory.StartNew((object obj) =>
                    {
                        string ckey = string.Concat("Key_", (i % maxKexs).ToString());
                        string strkey = obj as string;
                        if (string.IsNullOrEmpty(strkey))
                            strkey = ckey;

                        CacheTestData data = (CacheTestData)MemoryCache.CacheDict.GetValue<CacheTestData>(ckey);
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
            System.Console.WriteLine($"Finished {numberOfTasks} parallel tasks in {ts.Minutes:d2}:{ts.Seconds:d2}.{ts.Milliseconds:d3}\n\t{perSecond} tasks per second.\n");
        }

        static void RunSerial(int iterationsCount, short maxKexs = 16)
        {
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
                    CacheTestData data = new CacheTestData(ckey, Thread.CurrentThread.ManagedThreadId);
                    MemoryCache.CacheDict.SetValue<CacheTestData>(ckey, data);
                    // Console.WriteLine($"Task set cache key #{data.CKey} created at {data.CTime} on thread #{data.CThreadId}.");
                }
                else if ((i >= quater && i < half) || i >= threequater)
                {
                    string strkey = "Key_" + (i % maxKexs).ToString();
                    CacheTestData cacheData = (CacheTestData)MemoryCache.CacheDict.GetValue<CacheTestData>(strkey);
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
            System.Console.WriteLine($"Finished {iterationsCount} iterations in {ts.Minutes:d2}:{ts.Seconds:d2}.{ts.Milliseconds:d3}\n\t{perSecond} iterations per second.\n");

        }

    }

}
