using Ookii.Dialogs.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace CLW.ViewModel
{

    /// <summary>
    /// the MVVM equivalent to Session class, created on sept-24-2021 as part of switching to full MVVM pattern
    /// </summary>
    public class MainViewModel : BaseViewModel
    {
        public ObservableCollection<WatcherViewModel> WatcherViewModels { get; set; }



        private NewsSectionViewModel newsSectionViewModel;
        public NewsSectionViewModel NewsSectionViewModel
        {
            set { newsSectionViewModel = value; notif(nameof(NewsSectionViewModel)); }
            get { return newsSectionViewModel; }
        }


        private WatcherViewModel previousSelectedWatcherVM = null;

        private WatcherViewModel selectedWatcher;
        public WatcherViewModel SelectedWatcher
        {
            set { selectedWatcher = value;
                if (value != null)
                {
                    NewsSectionViewModel = new NewsSectionViewModel(SelectedWatcher.model);
                    selectedWatcher.IsSelected = true;
                    if(previousSelectedWatcherVM!=null)
                    previousSelectedWatcherVM.IsSelected = false;
                    previousSelectedWatcherVM = selectedWatcher;
                    notif(nameof(NewsSectionViewModel));
                    //Trace.WriteLine($"watcher selection changed to {value.model.CoreCustomLW.Name} ");
                }
                else
                {
                    if (previousSelectedWatcherVM != null)
                        previousSelectedWatcherVM.IsSelected = false;
                    previousSelectedWatcherVM = null;
                    //Trace.WriteLine($"watcher selection nulled");
                }
                notif(nameof(SelectedWatcher));
            }
            get { return selectedWatcher; }
        }




        public bool IsWatchersSectionsExpanded { get; set; }


        public MainViewModel()
        {
            //Trace.WriteLine($"MainViewModel ctor");

            populate();
           
            SelectedWatcher = WatcherViewModels.LastOrDefault();
        }


        void populate()
        {
            var watchers = new ObservableCollection<WatcherViewModel>();
            for (int i = 0; i < Services.WatchingService.Instance.Watchers.Count; i++)
            {

                var n = new WatcherViewModel(Services.WatchingService.Instance.Watchers[i]);
                //Trace.WriteLine($"adding watcher view model");
                watchers.Add(n);
            }
            Trace.WriteLine($"watchers were loaded from service");
            // Watchers = watchers;
            WatcherViewModels = watchers;
            notif(nameof(WatcherViewModels));
        }


        private void watchers_changed(object sender, NotifyCollectionChangedEventArgs e)
        {
            Trace.WriteLine("populating");
            populate();
            try
            {
                SelectedWatcher = WatcherViewModels.First((wm) => wm.model.guid == ((Model.WatcherModel)e.NewItems.GetEnumerator().Current).guid);
                Trace.WriteLine("populated");
                notif(nameof(SelectedWatcher));

            }
            catch (Exception)
            {

               
            }
        }

        internal void SelectWatcherById(Guid guid)
        {
            SelectedWatcher = WatcherViewModels.FirstOrDefault((wvm) => wvm.model.guid == guid);
        }




        public ICommand OpenSettingsCommand
        {
            get
            {
                return new MICommand
                (() => {
                    Services.Utils.ShowSettingsWindow();
                });
            }
        }
        public ICommand LoadWatcherFromFileCommand
        {
            get
            {
                return new MICommand
                (loadWatcherFromFileTAP);
            }
        }

        
        public ICommand LoadWatcherFromFilePathCommand
        {
            get
            {
                return new MICommand<string>
                (loadWatcherFromFilePath);
            }
        }


        static bool _lockVofd = false;
        private void loadWatcherFromFile()
        {
            if (_lockVofd) return;
            _lockVofd = true;
            BackgroundWorker okiiBgWorker = new BackgroundWorker();
            okiiBgWorker.DoWork += ((s,e) => {
                var worker = s as BackgroundWorker;

                Ookii.Dialogs.Wpf.VistaOpenFileDialog vofd = new VistaOpenFileDialog();
                throw new Exception("nah");
                vofd.DefaultExt = ".xml";
                vofd.InitialDirectory = MI.APP_DATA;
                bool? success = vofd.ShowDialog();
                if ((!success.HasValue) || (!success.Value))
                    e.Result = null;//means canceled
                else
                {
                    e.Result = vofd.FileName;
                }
            });

            okiiBgWorker.RunWorkerCompleted += (s, e) =>
             {
                 _lockVofd = false;
                 if (e.Error != null)
                 {
                     MainWindow.ShowMessage("error happened");
                 }
                 else if (e.Result == null)
                 {
                     return;
                 }
                 else
                 {
                     loadWatcherFromFilePath((string)e.Result);
                 }
                 
             };

            okiiBgWorker.RunWorkerAsync();
           

        }

        private void loadWatcherFromFileTAP()
        {

            Task<string> okiiTask = new Task<string>(() => {

                Ookii.Dialogs.Wpf.VistaOpenFileDialog vofd = new VistaOpenFileDialog();
                vofd.DefaultExt = ".xml";
                
                vofd.InitialDirectory = MI.APP_DATA;
                bool? success = vofd.ShowDialog();
                if ((!success.HasValue) || (!success.Value))
                    return null;
                else
                {
                    return vofd.FileName;
                }
            });



            okiiTask.GetAwaiter().OnCompleted(() => {
                string e = okiiTask.Result;// okiiTask.Result;
                if (e == null)
                {

                }
                else
                {
                    loadWatcherFromFilePath((string)e);
                }
            });
            okiiTask.Start();





        }




        private void loadWatcherFromFilePath(string path)
        {
           




            try
            {
               Guid? succed= Services.WatchingService.Instance.TryLoadWatcher(path);
                if (succed!=null)
                {
                    WatcherViewModels.Add(new WatcherViewModel(Services.WatchingService.Instance.Watchers.First((wm) => wm.guid == succed)));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Parsing failed", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }


            //if (loaded != null) //todo instead show message whn thnsg go wrong
            // MessageBox.Show("preset loaded successfilly"); // annoying
        }
    }
}
