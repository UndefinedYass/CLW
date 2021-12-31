using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Diagnostics;
using System.ComponentModel;
using System.Threading;
using System.Net;
using System.Threading.Tasks;

namespace CLW.Model
{
    /// <summary>
    /// base class for watching a static web page that has a list-like patern, by Checking it's content regularely with the pre configured Interval
    /// notifies changes through firing the events NewItems, RevokedItems ;
    /// the OnError event is fired in when the client fails to connect or when the list parser method fails 
    /// NOTE/ it's important for the ID property of an item to be unique and stay unchanged over time in order for this to work
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class ListWatcherBase<T> : BackgroundWorker  where T : INewItem
    {

        public ListWatcherBase()
        {
            WorkerReportsProgress = true;
            WorkerSupportsCancellation = true;
            //don't assign handelers becse their overrden
        }


        #region events
        public delegate void NewItemsEventHandle(object s, IList<T> news);
        public event NewItemsEventHandle NewItems;
        public event EventHandler<IList<T>> RevokedItems;
        public event EventHandler<CheckingException> OnError;
        public event EventHandler<EventArgs> OnCheck;
        public event EventHandler<EventArgs> OnStarted;
        public event EventHandler<EventArgs> OnStoped;
        public event EventHandler<Exception> OnFatalError;

        //secondary
        public event EventHandler<EventArgs> OnConnectionStarted;
        public event EventHandler<EventArgs> OnConnectionEnded;
        public event EventHandler<EventArgs> OnAnyChange;



        #endregion



        private enum ProgressReportType
        {
           StartedUpdate, InternetOperationUpdate, NewsUpdate, NonFatalErrorUpdate
        }
        private class WorkerProgress
        {
            public ProgressReportType ReportType { get; set; }
            public CheckingException Error { get; set; }
            public IList<T> NewItems { get; set; }
            public IList<T> RevokedItems { get; set; }
            public bool HasStartedConnaction { get; set; }
            public bool HasEndedConnection { get; set; }


        }


        #region Properties
        public Exception FailingException { set; get; }
        private int interval = 60000;
        public int Interval
        {
            set
            {
                if (value < 1000) value = 1000;
                interval = value;
            }
            get { return interval; }
        }
        public int ChecksCount { get; set; } = 0; // countes both successfull and unsuccessful checkings
        public TimeSpan WatchingFor { get; set; }
        public bool IsWatching { get { return IsBusy; } }
        public bool IsFailing { get; set; }
        public bool IsRunningSuccessfully { get; set; }
        public int SuccessfulCheckCount { set; get; }
        public ICollection<DictBasedNewItemObject> UnreadNews { set; get; }
        public bool HasUnreadNews { get { return UnreadNews.Count > 0; } }
        public int UnreadCount { get { return UnreadNews.Count; } }
        public string StatusMessage {get; set; }
        public int FailingCount { set; get; }
        /// <summary>
        /// short description: only assign something that Fucs.TimeSpanFromString can 
        /// parse, and expect the getter to return "03:30"-like strings
        /// this property works as a parser/converter for the actual int Interval property 
        /// simplifing the two way binding for the UI IncreaseTextBox control
        /// NOTE: when passing a string tha cannot be resolved the value 00:30 is supplied 
        /// </summary>
        public string IntervalAsString
        {
            set
            {
                var ts = Fucs.TimeSpanFromString(value);
                if (!ts.HasValue)
                {
                    Interval = (int)TimeSpan.FromSeconds(30).TotalMilliseconds;
                }
                else
                {
                    Interval = (int)ts.Value.TotalMilliseconds;
                }

            }
            get { return TimeSpan.FromMilliseconds(Interval).ToString().Substring(3); }
        }

        public abstract string Href { get; }
        public string InitialReferenceContent { get; set; }
        public ICollection<T> CurrentReferenceList { get; set; }
        public string ReferenceFilePath { get; set; }
        public bool IsInitialized { get; set; }
        public Exception FatalException { get; private set; }



        #endregion


        #region Methodes

        protected override void OnDoWork(DoWorkEventArgs e)
        {//todo: plug the worker routine here or don't, just use this body
            base.OnDoWork(e); //not really needed 

            //infinit loop
            //check CancellationPending value, if true set canceled and get out of here
            //time & cpu consuming work chunks separated by checking, better be synchronous
            //knwon exceptions that dont require stoping the watcher, eg connectivity,
            //should be handeled here, and sent as progress information
            //whereas unknow exceptions and
            //exception that reauirs terminating, should be thrwon, so that the worker ends emidiately and the completein
            //handeler will do the nessessary clean up and event raising
            //generate and report progress, using an internally implemented state structure
            //progress reports include even information on when a webClient connection starts and ends
            //this enables the consumer to implement better UI feedback (show/hide connection spinner)
            //don't fire any events here, the internal progress handler will, based on the progres rerportes
            //check on CancellationPending again
            //non blocking CancellationPending-aware waiting mechanism 
            //endof loop
            wcts = new CancellationTokenSource();
            if (IsInitialized == false) { TryInitialize(); }//todo, handel exceptions, keep cheking cancelation

            if (!IsInitialized) {
                throw new Exception("wacther failed to initialize");
            }
            ReportProgress(0, new WorkerProgress() { ReportType = ProgressReportType.StartedUpdate });
            while (true)
            {
                if (CancellationPending) { e.Cancel = true; return; }
                //#check
                CheckingResult res;
                try
                {


                    //check
                    res = WorkerCheck();
                    //report connectionstartd
                    if (CancellationPending) { e.Cancel = true; return; }
                    //report connection ended
                      
                }
                catch(Exception err)
                {
                    //if(err)//is fatal
                    if(true)
                    throw err;
                    else
                    {

                    }
                }

                //report lsts progress
                ChecksCount++;
                if (res.Success)
                {
                    IsFailing = false;
                    StatusMessage = $"Running{(SuccessfulCheckCount == 0 ? "" : $" ({SuccessfulCheckCount})")}";
                    if (res.ExistChanges)
                    {
                        ReportProgress(0, new WorkerProgress()
                        {
                            ReportType = ProgressReportType.NewsUpdate,
                            NewItems = res.NewItems, RevokedItems= res.RevokedItems,
                        });
                    }
                     SuccessfulCheckCount++;


                }
                else if (res.Success == false)
                {
                    if (res.Error.ShouldStopWatching)
                    {
                        throw res.Error;
                    }
                    else
                    {
                        this.FailingException = res.Error;
                        IsRunningSuccessfully = false;
                        IsFailing = true;
                        ReportProgress(0, new WorkerProgress() {
                            ReportType = ProgressReportType.NonFatalErrorUpdate,
                            Error = res.Error
                        });
                    }
                }

                if (CancellationPending) { e.Cancel = true; return; }
                var wt = Task.Delay(Interval, wcts.Token);
                try
                {
                    wt.Wait();
                    
                }
                catch (System.AggregateException err)
                {
                    Trace.WriteLine("this block");

                }
                if (wt.IsCanceled)
                {
                    Trace.WriteLine("wait task was canceled");
                }
                else
                {
                    Trace.WriteLine("that block");
                    //Trace.WriteLine("wait task completed");
                }

                if (CancellationPending) {
                    Trace.WriteLine("also this block");
                    e.Cancel = true; return;

                }



            }

        }

        protected override void OnRunWorkerCompleted(RunWorkerCompletedEventArgs e)
        {
            //getting here means two scenarios: 
            //normal completion via a cancelation: aka user-side calling stopWatchingAsync
            //in this case, u wanna rise stoped,

            //an error ruined the party: the workRoutine throwed exception
            //this may need implementing another stoped event or using soped event with approprate parametrage
            //to inform the user-sde (ui) to mabe block the watcher permanently
            //as it's beleived to be problematic and wont suceed again ;
            //(outdated, or causes unknown problems,)

            if (e.Cancelled)
            {
                StatusMessage = "Disabled";
                OnStoped.Invoke(this, new EventArgs());
            }
            else if (e.Error!=null)
            {
                IsRunningSuccessfully = false;

                StatusMessage = "Error";
                this.FatalException = e.Error;
                OnStoped?.Invoke(this, new EventArgs());
                OnFatalError?.Invoke(this, e.Error);
            }

            OnAnyChange?.Invoke(this, new EventArgs());

            base.OnRunWorkerCompleted(e);
        }
        protected override void OnProgressChanged(ProgressChangedEventArgs e)
        {
            WorkerProgress P = (WorkerProgress)e.UserState;
            switch (P.ReportType)
            {
                case ProgressReportType.InternetOperationUpdate:
                    if (P.HasStartedConnaction) OnConnectionStarted?.Invoke(this, new EventArgs());
                    if (P.HasEndedConnection) OnConnectionEnded?.Invoke(this, new EventArgs());
                    break;
                case ProgressReportType.NewsUpdate:
                    StatusMessage = $"Running{(SuccessfulCheckCount==0?"":$" ({SuccessfulCheckCount})")}";
                    if (P.NewItems.Count > 0) NewItems?.Invoke(this, P.NewItems);
                    if (P.RevokedItems.Count > 0) RevokedItems?.Invoke(this, P.RevokedItems);
                    SuccessfulCheckCount++;
                    IsRunningSuccessfully = true;
                    IsFailing = false;
                    break;
                case ProgressReportType.NonFatalErrorUpdate:
                    StatusMessage = $"Failed ({FailingCount})";
                    IsFailing = true;
                    FailingException = P.Error;
                    FailingCount++;
                    IsRunningSuccessfully = false;
                    OnError?.Invoke(this, P.Error);
                    break;
                case ProgressReportType.StartedUpdate:
                    StatusMessage = "Running";
                    OnStarted?.Invoke(this, new EventArgs());
                    break;
                default:
                    break;
            }
            //make this interprets Progress state that the work routine emits 
            //fires the 5 public events accorfignly, this should make the events WPF friendly? or at least avoid complicatting the user-side code with ugly progress handeling
            OnAnyChange?.Invoke(this, new EventArgs());
            base.OnProgressChanged(e);
        }

        public void StartWatchingAsync()
        {
            if (this.IsBusy) return;
            this.RunWorkerAsync();
        }
        public void StopWatchingAsync()
        {
            if (this.IsBusy==false) return;
            this.CancelAsync();
            wcts.Cancel();// the cancelation-aware wating call in the work body uses this
        }

        


        #endregion

        public void MarkAllAsRead()
        {
            UnreadNews.Clear();
            UpdateReferenceFile();
        }

        public void MarkAsRead(IEnumerable<DictBasedNewItemObject> what)
        {
            foreach (var item in what)
            {
                UnreadNews.Remove(item);
            }
        }


      



   


        /// <summary>
        /// returns false on any parsing failure, 
        /// on successful parsing it assigns a new list oject to the out parsedList argument
        /// </summary>
        /// <returns></returns>
        internal abstract bool TryParseList(string pageContent, out ICollection<T> parsedList);
        public string currentContent;
        struct CheckingResult
        {
            public bool ExistChanges;
            public IList<T> NewItems; public IList<T> RevokedItems;
            public bool Success;
            public CheckingException Error;
        }


        static SemaphoreSlim _sem = new SemaphoreSlim(Services.ConfigService.Instance.MaxCheckingConcurrency);

        
        //todo: allow user to set concurency in settngs, by defaults it is locked because the cpu usage is too much when 5 watchers parse simultanously
        //use this as it canxels, handels well exceptions, belongs in the OnDoWork body onlt created for code readabilit
        private CheckingResult WorkerCheck()
        {
            //lock (typeof(ListWatcherBase<T>))
            //{
            try
            {
                _sem.Wait(wcts.Token);
            }
            catch (OperationCanceledException)
            {
                return new CheckingResult();
            }
            try // this aims to make use of the finally block to realease the _sem 
            {
                ReportProgress(0, new WorkerProgress() { ReportType = ProgressReportType.InternetOperationUpdate, HasStartedConnaction = true });
                var r = Services.WebClientMi.Native.getNewInstance().GetTextAdvancedSync(Href);
                currentContent = r.Text;
                if (CancellationPending)
                {
                    ReportProgress(0, new WorkerProgress() { ReportType = ProgressReportType.InternetOperationUpdate, HasEndedConnection = true });
                    return new CheckingResult(); /*no need to set because the containing body will before exiting, this return only akes the canceling quick*/
                }
                if (r.Success == false || currentContent == null)
                {
                    ReportProgress(0, new WorkerProgress() { ReportType = ProgressReportType.InternetOperationUpdate, HasEndedConnection = true });
                    return new CheckingResult() { Success = false, Error = new CheckingException("Checking failed", false, new ConnectivityException("Couldnt connect to " + Href, r.Error)), ExistChanges = false };
                }
                ICollection<T> currentList;
                if (!TryParseList(currentContent, out currentList))
                {
                    ReportProgress(0, new WorkerProgress() { ReportType = ProgressReportType.InternetOperationUpdate, HasEndedConnection = true });
                    return new CheckingResult() { Success = false, Error = new CheckingException("parsing list failed", true, new ListParserException("todo, get this exceptio nfo from the provider")) };
                }
                if (CancellationPending)
                {
                    ReportProgress(0, new WorkerProgress() { ReportType = ProgressReportType.InternetOperationUpdate, HasEndedConnection = true });
                    return new CheckingResult(); /*no need to set because the containing body will before exiting, this return only akes the canceling quick*/
                }
                List<T> maybeNewItems, maybeRevokedItems;
                maybeNewItems = currentList.Where((itm) => !CurrentReferenceList.Any((itmOld) => itmOld.ID == itm.ID)).ToList();
                maybeRevokedItems = CurrentReferenceList.Where((itm) => !currentList.Any((itmNew) => itmNew.ID == itm.ID)).ToList();
                bool existChanges = false;
                if ((maybeNewItems.Count > 0))
                {
                    CurrentReferenceList = currentList;
                    existChanges = true;
                }
                if (maybeRevokedItems.Count > 0)
                {
                    CurrentReferenceList = currentList;
                    existChanges = true;
                }
                ReportProgress(0, new WorkerProgress() { ReportType = ProgressReportType.InternetOperationUpdate, HasEndedConnection = true });
                return new CheckingResult() { Success = true, NewItems = maybeNewItems, RevokedItems = maybeRevokedItems, ExistChanges = existChanges };
            }
            finally
            {
                _sem.Release();
            }
            //parallel programming is such a pain in the ass 
        }




        private void TryInitialize()
        {
            Stopwatch sw = Stopwatch.StartNew();
            if (IsInitialized) return;
            StatusMessage = "Initilaizing";
            if (CurrentReferenceList == null)
            {
                
                ICollection<T> initialList;
                Stopwatch swp = Stopwatch.StartNew();
                bool succesfullInitialization = TryParseList(InitialReferenceContent, out initialList);
                if (!succesfullInitialization)
                {
                    FailingException = new WatcherException("mi: canot init the watcher because parsing the initialContentReference failed", true, null);
                    throw FailingException;
                }
                Logger.Info($"watcherName:{(this as CustomLW).Name}{Environment.NewLine}TryParse time:{swp.Elapsed.ToString()}");
                CurrentReferenceList = initialList;
            }
            Logger.Info($"watcherName:{(this as CustomLW).Name}{Environment.NewLine}IryInitialize time:{sw.Elapsed.ToString()}");
            StatusMessage = "Ready";
            IsInitialized = true;
        }



     
        private BackgroundWorker bg = null;
        private CancellationTokenSource wcts = new CancellationTokenSource() ;





        /// <summary>
        /// free the memory allocated by the CPE model, the watcher can still be initialyzed and started again
        /// </summary>
        public virtual void CloseAsync()
        {
            Delegate[] lst;
            lst= this.OnCheck.GetInvocationList();
            if(lst!=null)
            foreach(Delegate del in lst)
            {
                OnCheck -= ( del as EventHandler<EventArgs>);
            }
            var  lst2 = this.OnConnectionEnded.GetInvocationList();
            if (lst2 != null)
                foreach (Delegate del in lst2)
            {
                OnCheck -= (del as EventHandler<EventArgs>);
            }
            var lst3 = this.OnConnectionStarted.GetInvocationList();
            if (lst3 != null)
                foreach (Delegate del in lst3)
            {
                OnCheck -= (del as EventHandler<EventArgs>);
            }
            var lst4 = this.OnStarted.GetInvocationList();
            if (lst4 != null)
                foreach (Delegate del in lst4)
            {
                OnCheck -= (del as EventHandler<EventArgs>);
            }
            var lst5 = this.OnStoped.GetInvocationList();
            if (lst5 != null)
                foreach (Delegate del in lst5)
            {
                OnCheck -= (del as EventHandler<EventArgs>);
            }
            var lst6 = this.OnFatalError.GetInvocationList();
            if (lst6 != null)
                foreach (Delegate del in lst6)
            {
                OnCheck -= (del as EventHandler<EventArgs>);
            }

        }

        internal void UpdateReferenceFile()
        {
            File.WriteAllText(this.ReferenceFilePath, currentContent);
        }

        private void InitializeComponent()
        {

        }
    }





}
