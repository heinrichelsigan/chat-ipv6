using Area23.At.Framework.Core.Static;
using Area23.At.Framework.Core.Util;
using System.ComponentModel;

namespace EU.CqrXs.WinForm.SecureChat.Util
{
    public sealed class BgWorkerMonitor : BackgroundWorker
    {

        #region fields

        private readonly Lock spinLock = new Lock();
        private bool bgwStarted = false;
        private bool bgwRunning = false;
        private int bgwThreadId = -1;
        internal int bgwProgress = 0;
        private string bgwStatus;
        private readonly DateTime bgwStartTime;
        private DateTime bgwLastWorkTime;
        private DateTime now = DateTime.Now;

        #endregion fields


        #region Properties

        /// <summary>
        /// Eventhanlder Work_Monitor is fired, when needing a delegate back in WinForm Chat
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public EventHandler Work_Monitor { get; set; }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public EventHandler App_Restart { get; set; }

        public bool IsRunning { get => bgwStarted && bgwRunning && bgwProgress > 0 && bgwProgress < 100; }

        public int ThreadId { get => (bgwThreadId > 0) ? bgwThreadId : GetCurrentThreadId(); }

        public static int ProcessId { get => Thread.GetCurrentProcessorId(); }


        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string CurrentStatus { get => bgwStatus; internal set => bgwStatus = value; }
        

        #endregion Properties

                
        /// <summary>
        /// Constructor of PollSleeper
        /// </summary>
        public BgWorkerMonitor()
        {
            bgwStartTime = DateTime.Now;
            bgwLastWorkTime = DateTime.Today;
            bgwStarted = true;
            bgwStatus = string.Empty;
            DoWork += new DoWorkEventHandler(BgWorkerMonitor_DoWork);
            RunWorkerCompleted += new RunWorkerCompletedEventHandler(BgWorkerMonitor_RunWorkerCompleted);
            ProgressChanged += new ProgressChangedEventHandler(BgWorkerMonitor_ProgressChanged);
            WorkerReportsProgress = true;
            WorkerSupportsCancellation = true;
        }



        /// <summary>
        /// BgWorkerMonitor_DoWork is fired on <see cref="BackgroundWorker.DoWork"/>
        /// </summary>
        /// <param name="sender"><see cref="object?"/> sender</param>
        /// <param name="e"><see cref="DoWorkEventArgs"/> e</param>
        public void BgWorkerMonitor_DoWork(object? sender, DoWorkEventArgs e)
        {
            bgwRunning = true;
            bgwThreadId = GetCurrentThreadId();
            while (true)
            {
                Thread.Sleep(Constants.BGWORKWE_BUSYWAITING_SLEEP);

                if (!bgwRunning)
                    break;

                if (++bgwProgress > 96)
                    bgwProgress = 1;

                bool dirWatch = BgWorkerMonitor_DoMonitor(sender);
                if (CancellationPending)
                {
                    e.Cancel = true;
                    ReportProgress(0);
                }
                
            }
        }



        /// <summary>
        /// BgWorkerMonitor_ProgressChanged is fired, when <see cref="BgWorkerMonitor"/> performs another working step 
        /// and reports progress <see cref="<see cref="BackgroundWorker.ReportProgress(int)"/>
        /// </summary>
        /// <param name="sender"><see cref="object?"/> sender</param>
        /// <param name="e"><see cref="ProgressChangedEventArgs"/> e</param>
        public void BgWorkerMonitor_ProgressChanged(object? sender, ProgressChangedEventArgs e)
        { 
            int pWorked = e.ProgressPercentage;
            now = DateTime.Now;

            if (bgwStartTime.Day != now.Day ||
                now.Hour == 0 && now.Minute < 18 ||
                pWorked >= 96)
            {
                EventHandler handler = App_Restart;
                Area23EventArgs<DateTime> eArgs = new Area23EventArgs<DateTime>(now);
                handler?.Invoke(this, eArgs);
            }
        }

        /// <summary>
        /// BgWorkerMonitor_RunWorkerCompleted is launched, when <see cref="BgWorkerMonitor" /> 
        /// finished work by <see cref="BackgroundWorker.RunWorkerCompleted"/>
        /// </summary>
        /// <param name="sender"><see cref="object?"/> sender</param>
        /// <param name="e"><see cref="RunWorkerCompletedEventArgs"/> e</param>
        public void BgWorkerMonitor_RunWorkerCompleted(object? sender, RunWorkerCompletedEventArgs e)
        {
            bgwRunning = false;
            bgwProgress = 100;

            if (e.Cancelled)
            {
                CurrentStatus = $"[Canceled]";
            }
            else if (e.Error != null)
            {
                string msg = string.IsNullOrEmpty(e.Error.Message) ? "" : e.Error.Message;
                CurrentStatus = $"[Error ({msg})]";
            }
            else
            {
                CurrentStatus = $"[Completed]";
            }

        }


        /// <summary>
        /// ObserverBeholderWatchMonitor invokes EventHandler <see cref="BgWorkerMonitor"/> with
        /// <see cref="Area23EventArgs{DateTime}"/>
        /// </summary>
        /// <param name="sender">object sender</param>
        /// <returns>true, invoking EventHandler <see cref="Watch_Monitor"/> succeeds.</returns>
        internal bool BgWorkerMonitor_DoMonitor(object? sender)
        {
            EventHandler handler = Work_Monitor;
            Area23EventArgs<DateTime> eArgs = new Area23EventArgs<DateTime>(DateTime.Now);
            handler?.Invoke(this, eArgs);

            return handler != null;
        }


        internal int GetCurrentThreadId()
        {
            try
            {
                bgwThreadId = System.AppDomain.GetCurrentThreadId(); // 
            }
            catch (Exception exa)
            {
                Area23Log.LogOriginMsgEx("BgWorkerMonitor", "GetCurrentThreadId()", exa);
            }
            if (bgwThreadId <= 0)
            {
                bgwThreadId = Environment.CurrentManagedThreadId;
            }

            return bgwThreadId;
        }
        

    }
}
