using Hardcodet.Wpf.TaskbarNotification;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace CLW
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {   
        private TaskbarIcon notifyIcon;
        //public Session AppSession { get; private set; 
        private const string AppName = "{GUID}";
        private const string UniqueEventName = "{GUIDo}";
        private EventWaitHandle eventWaitHandle;

        private Mutex _mutex ;
       
        protected override void OnStartup(StartupEventArgs e)
        {

            base.OnStartup(e);

        }




        protected override void OnExit(ExitEventArgs e)
        {
            notifyIcon?.Dispose(); //the icon would clean up automatically, but this is cleaner
            base.OnExit(e);
        }
        //check if another instance is alread running
        
        private void Application_Startup(object sender, StartupEventArgs e)
        {

            bool isOnwned;

            this._mutex = new Mutex(true, AppName, out isOnwned);
             this.eventWaitHandle = new EventWaitHandle(false, EventResetMode.AutoReset, UniqueEventName);
            GC.KeepAlive(this._mutex);

            if (isOnwned)
            {
                var thread = new Thread(() => {
                    while (this.eventWaitHandle.WaitOne())
                    {
                        Current.Dispatcher.BeginInvoke((Action)(() => {
                            Services.Utils.ShowMainWindow();
                        }));
                    }
                });
                thread.IsBackground = true;
                thread.Start();
                //create the notifyicon (it's a resource declared in NotifyIconResources.xaml
                notifyIcon = (TaskbarIcon)FindResource("NotifyIcon");
                var triggerCall = Services.WatchingService.Instance;// triggers the instance to be created
                bool shouldOpenMw = Services.ConfigService.Instance.OpenMainWindowOnStartup;
                if (shouldOpenMw)
                {
                    Services.Utils.ShowMainWindow();
                }
                return;

            }
            
                this.eventWaitHandle.Set();
                this.Shutdown();


            

           
        }
    }
}
