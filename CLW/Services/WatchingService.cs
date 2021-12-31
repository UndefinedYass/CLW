using CLW.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLW.Services
{
    public class WatchingService 
    {
        private static WatchingService _instance;
        public static int CCInstances;

        //singletone, gets created once tha app starts and says around intile the app shuts down
        //does, or contains, the object that do the watching
        public static WatchingService Instance 
        {
            get
            {
                if(_instance!=null)
                return _instance;
                else
                {
                    _instance = new WatchingService();
                    return _instance;
                }
            }
            
        }


        public WatchingService()
        {
            //Trace.WriteLine("WatchingService ctor");
            CCInstances++;
            Watchers = new ObservableCollection<WatcherModel>( );
            for (int i = 0; i < Services.ConfigService.Instance.CLWPresetsDeclarations.Count; i++)
            {
                string pth = ConfigService.Instance.CLWPresetsDeclarations[i].Path;
                bool shouldLoad = ConfigService.Instance.CLWPresetsDeclarations[i].AutoLoad;
                bool shouldSatrt = ConfigService.Instance.CLWPresetsDeclarations[i].AutoStart;
                //Trace.WriteLine("loading watcher declaration informaton: PATH "+ pth);
                if(File.Exists(pth)== false)
                {
                    Trace.WriteLine($"preset fileis missing: {pth}");
                    shouldLoad = false;
                }
                if (shouldLoad == false) continue;
                Exception loadingErr;
                var lpr = WatcherModel.FromPresetFile(pth).GetAwaiter().GetResult();
                if (lpr.Success==false)
                {
                    MainWindow.ShowMessage(lpr.Error?.Message );
                    if (lpr.Error.Message.Contains("Value"))
                    {
                        throw lpr.Error;
                    }
                    Trace.WriteLine(lpr.Error?.Message, "Error");
                    Trace.WriteLine(lpr.Error?.InnerException?.Message, "Inner");
                    Trace.WriteLine(lpr.Error?.InnerException?.InnerException?.Message, "Inner2");


                    continue;
                }
                var wm = lpr.Result;
                wm.CoreCustomLW.NewItems += (s, news) => {

                    
                    Utils.SetTrayIconOverlay(Model.Enums.TrayIconOverlayMode.Dot);
                    Utils.SpawnNotification(news.OrderByDescending((o) => o.SubTitle), wm);
                    
                    //updating TrayTooltip
                    string newTrayToolTipText = $"{wm.CoreCustomLW.UnreadCount} news from {wm.CoreCustomLW.Name}";
                    int _totalNewsCout = GetGlobalUnreadCout();
                    if(_totalNewsCout!= wm.CoreCustomLW.UnreadCount)
                    {
                        newTrayToolTipText += $", {_totalNewsCout} news in total.";
                    }
                    Utils.UpdateToolTipText(newTrayToolTipText);
                };
                if (shouldSatrt)
                {
                    WatchingService.TryStartWatchingAsync(wm);
                    
                }
                wm.guid = Guid.NewGuid();
                //Trace.WriteLine("addiing watcher model in watchingservice ");

                Watchers.Add(wm);
            }
        }



        public Guid? TryLoadWatcher(string pth)
        {
            //Trace.WriteLine("loading watcher declaration informaton: PATH " + pth);
            if (File.Exists(pth) == false)
            {
                Trace.WriteLine($"preset fileis missing: {pth}");
                return null;
            }
            Exception loadingErr;
            var lpr = WatcherModel.FromPresetFile(pth).GetAwaiter().GetResult();
            if (lpr.Success == false)
            {
                MainWindow.ShowMessage(lpr.Error?.Message);
                if (lpr.Error.Message.Contains("Value"))
                {
                    throw lpr.Error;
                }
                Trace.WriteLine(lpr.Error?.Message, "Error");
                Trace.WriteLine(lpr.Error?.InnerException?.Message, "Inner");
                Trace.WriteLine(lpr.Error?.InnerException?.InnerException?.Message, "Inner2");


                return null;
            }
            var wm = lpr.Result;
            wm.CoreCustomLW.NewItems += (s, news) => {
                Utils.SpawnNotification (news.OrderByDescending((o) => o.SubTitle), wm);
               
            };
            wm.guid = Guid.NewGuid();
           // Trace.WriteLine("addiing watcher model in watchingservice ");

            Watchers.Add(wm);
            return wm.guid;
        }




        public bool GetHasUnreadNews()
        {
            return Watchers.Any((w) => w.CoreCustomLW.HasUnreadNews);
        }

        public int GetGlobalUnreadCout()
        {
            return Watchers.Aggregate(0,(acc,curr) => acc+ curr.CoreCustomLW.UnreadCount);
        }



        /// <summary>
        /// app-wide startWatching method, can , and should be called by the UI thread directely, 
        /// exception free
        /// </summary>
         public static void TryStartWatchingAsync(WatcherModel wm)
         {
            wm.CoreCustomLW.StartWatchingAsync();

            Task.Run(() =>
                {
                    //wm.CoreCustomLW.StartWatchingWorker(); //switch to this when you ned the app working, but this is depreceted
                   
                });
           



         }

        /// <summary>
        /// app-wide startWatching method, can , and should be called by the UI thread directely, 
        /// exception free
        /// </summary>
        public static void TryStopWatchingAsync(WatcherModel wm)
        {

            Task.Run(() =>
            {
                // wm.CoreCustomLW.StopWatching(); deprecatd but workng, untill changing the ui side logic
                
            });

            wm.CoreCustomLW.StopWatchingAsync();

        }



        /// <summary>
        /// app-wide startWatching method, can be called by the UI thread directely, 
        /// </summary>
        /* public static void TryStartWatchingAsyncold(WatcherModel wm)
         {
             lock (wm)
             {
                 if (!wm.CoreCustomLW.IsInitialized)
                 {
                     try
                     {
                          Task.Run(() => {
                             wm.CoreCustomLW.TryInitialize();

                         }).Wait();
                     }
                     catch(Exception err)
                     {
                         Trace.WriteLine("failed to initialize: "+Logger.BuildStackMessage(err) );
                         return;
                     }
                     finally
                     {
                         if (wm.CoreCustomLW.IsInitialized ==true)
                         {
                             try
                             {
                                 Task.Run(() => {
                                     wm.CoreCustomLW.StartWatching();
                                 });
                             }
                             catch (Exception err)
                             {
                                 Trace.WriteLine("watching failed :" + Logger.BuildStackMessage(err));
                             }
                             finally
                             {
                                 //whether it completed or failed , we need to make sure isWatching is false and OnStoped is fired
                                 wm.CoreCustomLW.StopWatching();
                             }
                             // throw new Model.WatcherException("Initializing watcher failed", false, null);
                         }
                         else
                         {
                             Trace.WriteLine("still not initialized");
                         }

                     }

                 }

                 else
                 {
                     try
                     {
                         Task.Run(() => {
                             wm.CoreCustomLW.StartWatching();
                         });
                     }
                     catch (Exception err)
                     {
                         Trace.WriteLine("watching failed :" + Logger.BuildStackMessage(err));
                     }
                     finally
                     {
                         //whether it completed or failed , we need to make sure isWatching is false and OnStoped is fired
                         wm.CoreCustomLW.StopWatching();
                     }
                 }


             }


         }*/

        public ObservableCollection<WatcherModel> Watchers { get; set; }



    }
}
