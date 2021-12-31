using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CLW.Model;
using System.Diagnostics;

namespace CLW.ViewModel
{
    public class NewsSectionViewModel: BaseViewModel
    {


        public NewsSectionViewModel()
        {
            NewsViewModels = new ObservableCollection<NewItemViewModel>();

        }

        public NewsSectionViewModel(WatcherModel model)
        {
            Watcher = model;
            model.CoreCustomLW.OnAnyChange += (s, e) =>
            {
                UpdateVisibilitiesTemp();
            };
            model.CoreCustomLW.NewItems += (s, e) =>
            {
                updateCollection();
            };
        }

        private bool isVisible;
        public bool IsVisible
        {
            get { return Watcher!=null; }
        }

       



        private Model.WatcherModel watcher = null;
        public Model.WatcherModel Watcher
        {
            set { watcher = value;
                notif(nameof(IsVisible));
                if (watcher != null)
                {
                    notif(nameof(Watcher));
                    notif(nameof(HeaderText));
                    updateCollection();
                }

                


            }
            get { return watcher; }
        }

        private void updateCollection()
        {
            if (Watcher == null) return;
            var enume = watcher.CoreCustomLW.UnreadNews.Select((nw) => new NewItemViewModel(Converters.TempGen.expandoLWOtoNewModel( nw)));
            enume = enume.Take(Services.ConfigService.Instance.ItemsPerPage);
            var newsViewModels = new ObservableCollection<NewItemViewModel>(enume);
            NewsViewModels = newsViewModels;
        }

        public ObservableCollection<NewItemViewModel> NewsViewModels { get; set; }


        private string headerText;

        public string HeaderText
        {
           
            get { return Watcher==null? "-": Watcher.CoreCustomLW.Name  ; }
        }





        public Exception FatalException
        {
            get { return Watcher?.CoreCustomLW.FatalException; }
        }

        public string FatalExceptionStackMessage
        {
            get { return Logger.BuildStackMessage(Watcher?.CoreCustomLW.FatalException); }
        }

        public Exception FailingException
        {
            get { return Watcher?.CoreCustomLW.FailingException; }
        }

        public string FailingExceptionStackMessage
        {
            get { return Logger.BuildStackMessage(Watcher?.CoreCustomLW?.FailingException); }
        }



        #region UIElementsVisibilities

        private bool isErrorCardVisible;
        public bool IsErrorCardVisible
        {
            //error card visible when:
            //initialized
            // the watcher is not runnningSuccessfully aka isFailing
            // and the watcher is watching
            // and there is a non null failingexception
            // there s no fatal error visbility

            set { isErrorCardVisible = value; notif(nameof(IsErrorCardVisible)); }
            get
            {
                return Watcher.CoreCustomLW.IsInitialized
                  && Watcher.CoreCustomLW.IsFailing
                  && Watcher.CoreCustomLW.IsWatching
                  && Watcher.CoreCustomLW.FailingException != null
                  && !IsFatalErrorCardVisible;
                ;
            }
        }


        private bool isFatalErrorCardVisible;
        public bool IsFatalErrorCardVisible
        {
            //fatal error card visible when:
            // the watcher is not watching
            // and the watcher has a fatal error

            set { isFatalErrorCardVisible = value; notif(nameof(IsFatalErrorCardVisible)); }
            get
            {
                return !Watcher.CoreCustomLW.IsWatching
                  && Watcher.CoreCustomLW.FatalException != null;
            }
        }


        private bool isNoNewsCardVisible;
        public bool IsNoNewsCardVisible
        {
            //no news card visible when:
            // initiaalized
            // the watcher is watching
            // and the watcher has done a succesfull check 
            // the news count is zero
            set { isNoNewsCardVisible = value; notif(nameof(IsNoNewsCardVisible)); }
            get
            {
                return Watcher.CoreCustomLW.IsInitialized
                  && Watcher.CoreCustomLW.IsWatching
                  && Watcher.CoreCustomLW.SuccessfulCheckCount > 0
                  && Watcher.CoreCustomLW.UnreadCount == 0;
            }
        }


        private bool newsListVisibility;
        public bool IsNewsListVisible
        {
            //aka actual news,
            //initialized
            // when the watcher has done at least one check
            // the news count is greater than zero
            set { newsListVisibility = value; notif(nameof(IsNewsListVisible)); }
            get
            {
                return Watcher.CoreCustomLW.IsInitialized
                  && Watcher.CoreCustomLW.SuccessfulCheckCount > 0
                  && Watcher.CoreCustomLW.UnreadCount > 0;
            }
        }





        #endregion



        private void UpdateVisibilitiesTemp()
        {
            //thses assignments are just propertychanged invokers
            IsErrorCardVisible = false;
            IsFatalErrorCardVisible = false;
            IsNoNewsCardVisible = false;
            IsNewsListVisible = false;
            notif(nameof(FatalExceptionStackMessage));
            notif(nameof(FatalException));
            notif(nameof(FailingExceptionStackMessage));
            notif(nameof(FailingException));




            string st = $"is initialized {watcher.CoreCustomLW.IsInitialized}\n is watching {Watcher.CoreCustomLW.IsWatching}\n has fatal exc {Watcher.CoreCustomLW.FatalException != null}\n is runnin sux {Watcher.CoreCustomLW.IsRunningSuccessfully}\n\n";
            Trace.WriteLine(Watcher.CoreCustomLW.Name + " : \n" + st);
        }



    }
}
