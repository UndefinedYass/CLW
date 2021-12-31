using CLW.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;


namespace CLW.ViewModel
{
   public class SysTrayModel : INotifyPropertyChanged
    {

        static CLW.Converters.IntToVisibilityConverter yu;
        public SysTrayModel()
        {
           var uri = new Uri($@"pack://application:,,,/media/SysTrayIcon-default.ico");
            CurrentIcon = new BitmapImage(uri);
            ToolTipText = "Nothing New";
        }


        private void notif(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }




        private ImageSource currentIcon;

        public ImageSource CurrentIcon
        {
            set { currentIcon = value; notif(nameof(CurrentIcon)); }
            get { return currentIcon; }
        }


        private string toolTipText;
        public string ToolTipText
        {
            set { toolTipText = value; notif(nameof(ToolTipText)); }
            get { return toolTipText; }
        }




        /// <summary>
        /// try Starting AllWatchers
        /// </summary>
        public ICommand StartAllWatchersCommand
        {
            get
            {
                return new MICommand

                (() => {

                    foreach(Model.WatcherModel wm in WatchingService.Instance.Watchers)
                    {
                        WatchingService.TryStartWatchingAsync(wm);
                    }
                });
            }
        }

        /// <summary>
        /// Try stoping all watchers
        /// </summary>
        public ICommand StopAllWatchersCommand
        {
            get
            {
                return new MICommand

                (() => {
                    foreach (Model.WatcherModel wm in WatchingService.Instance.Watchers)
                    {
                        WatchingService.TryStopWatchingAsync(wm);
                    }

                });
            }
        }


        /// <summary>
        /// spwan a window / activate or maximze it
        /// </summary>
        public ICommand ActivateWindowCommand
        {
            get
            {
                return new MICommand
                    
                (() => {
                    
                    Services.Utils.ShowMainWindow();
                });
            }
        }



        /// <summary>
        /// Shows a window, if none is already open.
        /// </summary>
        public ICommand ShowHideWindowCommand
        {
            get
            {
                return new MICommand
                (() =>  {
                MainWindow maybeMainWindow = (MainWindow) Utils.GetOpenWindowByType(typeof(MainWindow));
                if (maybeMainWindow != null)
                      {
                        maybeMainWindow.Close();
                        Application.Current.MainWindow = null;
                          GC.Collect(); GC.WaitForPendingFinalizers(); GC.Collect();
                      }
                      else
                      {
                        MainWindow newMainWiindow = new MainWindow();
                        Application.Current.MainWindow = newMainWiindow;
                        newMainWiindow.Show();
                      }
                  }
                );
            }
        }

        

        /// <summary>
        /// Shuts down the application.
        /// </summary>
        public ICommand ShowLastNotificationCommand
        {
            get
            {
                GC.Collect(10);
                GC.Collect(10); GC.WaitForPendingFinalizers(); GC.Collect(10);
                GC.Collect(); GC.WaitForPendingFinalizers(); GC.Collect();

                return new MICommand(() => Utils.ShowLastNotification());
            }
        }

        /// <summary>
        /// Shuts down the application.
        /// </summary>
        public ICommand ExitApplicationCommand
        {
            get
            {
                return new MICommand(() => Application.Current.Shutdown() );
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
