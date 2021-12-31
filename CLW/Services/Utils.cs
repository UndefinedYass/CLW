using CLW.Model.Enums;
using CLW.ViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using CLW.Model;
using System.Threading;
using System.IO;
using System.ComponentModel;

namespace CLW.Services
{

    /// <summary>
    /// some things here must have their own wraper class e.g SysTray controllers
    /// </summary>
    public static class Utils
    {

        /// <summary>
        /// bring the main window to front, creating new one if needed
        /// 
        /// </summary>
        public static void ShowMainWindow()
        {
            MainWindow maybeMainWindowRef =(MainWindow) GetOpenWindowByType(typeof(MainWindow));
            if (maybeMainWindowRef != null)
            {
                if (maybeMainWindowRef.WindowState == System.Windows.WindowState.Minimized)
                    maybeMainWindowRef.WindowState = System.Windows.WindowState.Normal;
                maybeMainWindowRef.Activate();
                maybeMainWindowRef.Topmost = true;
                maybeMainWindowRef.Topmost = false;
                maybeMainWindowRef.Focus();
                 }
            else
            {
                MainWindow newMainWindow = new MainWindow();
                Application.Current.MainWindow = newMainWindow;
                newMainWindow.Show();
            }
           
        }

        /// <summary>
        /// Exeption-safe, synchronous, outputs the filepath that exists checked or null
        /// </summary>
        /// <returns></returns>
        internal static bool GetDownloadedFileExists(NewModel nw, out string filepath)
        {

            string pth = null;
            try
            {
                pth= Path.Combine(ConfigService.Instance.RememberedDownloadOutputDirectory, Path.GetFileName(nw.DownloadLink));

            }
            catch (Exception)
            {

               
            }
            filepath = pth;
            if (pth == null || !File.Exists(pth)) return false;
            else return true;
        }

        internal static void ShowSettingsWindow()
        {
            var mm = new SettingsWindowViewModel(ConfigService.Instance);

            CLWSettingsWindow csw = new CLWSettingsWindow() { DataContext = mm };
            mm.OnRequestClose += (ss, ee) =>
            {
                csw?.Close();
            };

            csw.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            csw.ShowDialog();
           
        }


        static SysTrayModel _notifyIconModel = null;
        private static NotificationViewModel LastNotificationViewModel;

        static SysTrayModel NotifyIconModelInstance
        { get
            {
                if (_notifyIconModel == null)
                    _notifyIconModel = (SysTrayModel)Application.Current.FindResource("SysTrayModelInstance");
                return _notifyIconModel;
            }
        }


        /// <summary>
        /// thread-safe
        /// </summary>
        public static void SetTrayIconOverlay(TrayIconOverlayMode mode)
        {
           
            var targetIconUri = new Uri( $@"pack://application:,,,/media/SysTrayIcon-{mode.ToString()}.ico");// I relly dont wanna repeat myself
            Application.Current.Dispatcher.Invoke(() =>
            {
                NotifyIconModelInstance.CurrentIcon = new BitmapImage(targetIconUri);
            });
        }


        /// <summary>
        /// thread safe
        /// </summary>
        /// <param name="text"></param>
        public static void UpdateToolTipText(string text)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                NotifyIconModelInstance.ToolTipText = text;
            });
        }


        /// <summary>
        /// official notifiction creating methode
        /// thread-safe
        /// </summary>
        /// <param name="news"></param>
        /// <param name="wm"></param>
        internal static void SpawnNotification(IOrderedEnumerable<DictBasedNewItemObject> news, WatcherModel wm)
        {
            Application.Current.Dispatcher.Invoke(() => {
                Views.NotificationWindowView cnw = new Views.NotificationWindowView();
                var cnw_dc = new ViewModel.NotificationViewModel(news.OrderByDescending((o) => o.SubTitle), wm);
                cnw.DataContext = cnw_dc;
                cnw_dc.OnRequestClose += ((se, ee) => { if (cnw != null) cnw.Close(); });
                cnw.ShowInTaskbar = false;
                cnw.Show();
                
                LastNotificationViewModel = cnw_dc;
            });
           
        }


        /// <summary>
        /// creates a new notification window with a viewModel saved from the last SpawnNotification call
        /// if nothing is saved this does nothing
        /// </summary>
        public static void ShowLastNotification()
        {
            if (LastNotificationViewModel == null) return;
            Views.NotificationWindowView cnw = new Views.NotificationWindowView();
            cnw.DataContext = LastNotificationViewModel;
            //todo code smell, leaking event handlers
            LastNotificationViewModel.OnRequestClose 
               += ((se, ee) => { if (cnw != null) cnw.Close(); });
            cnw.ShowInTaskbar = false;
            cnw.Show();
        }


        public struct PickFolderResult
        {
            public string selectedPath;
            public bool Success;
        }
        public struct PickFileResult
        {
            public string selectedPath;
            public bool Success;
        }
        public delegate void PickFolderCallback(PickFolderResult OpertionResult);
        public delegate void PickFileCallback(PickFileResult OpertionResult);

        public static void PickFolderAsync(PickFolderCallback cllbck )
        {
            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += (s, e) =>
            {
                Ookii.Dialogs.Wpf.VistaFolderBrowserDialog vfbd = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog();
                vfbd.ShowDialog();
                if (string.IsNullOrEmpty(vfbd.SelectedPath))
                {
                    e.Result = new PickFolderResult() { Success = false };
                    return;
                }
                else
                {
                    e.Result = new PickFolderResult() { Success = true, selectedPath = vfbd.SelectedPath };
                    return;
                }
            };
            bw.RunWorkerCompleted += (s, r) =>
             {
                 cllbck.Invoke((PickFolderResult)r.Result);
             };
            bw.RunWorkerAsync();
        }


        public static void PickFileAsync(PickFileCallback cllbck)
        {
            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += (s, e) =>
            {
                Ookii.Dialogs.Wpf.VistaOpenFileDialog vofd = new Ookii.Dialogs.Wpf.VistaOpenFileDialog();
                vofd.ShowDialog();
                if (string.IsNullOrEmpty(vofd.FileName))
                {
                    e.Result = new PickFileResult() { Success = false };
                    return;
                }
                else
                {
                    e.Result = new PickFileResult() { Success = true, selectedPath = vofd.FileName };
                    return;
                }
            };
            bw.RunWorkerCompleted += (s, r) =>
            {
                cllbck.Invoke((PickFileResult)r.Result);
            };
            bw.RunWorkerAsync();
        }




        public static Window GetOpenWindowByType(Type targetType)
        {
            foreach(var item in Application.Current.Windows)
            {
                if (item.GetType() == targetType)
                {
                    return item as Window;
                }
            }

            return null;
        }

        /// <summary>
        /// asynchronously starts a backgroundworker that creates and starts a process with the filename specified, 
        /// handels errors log
        /// </summary>
        /// <param name="fileToOpen"></param>
        internal static void TryOpenFileAsync(string fileToOpen)
        {
            BackgroundWorker bg = new BackgroundWorker();
            bg.DoWork += (s,e) =>{
                
                    Process.Start(fileToOpen);
                    e.Result = true;
                
            };
            bg.RunWorkerAsync();
            
        }
    }
}
