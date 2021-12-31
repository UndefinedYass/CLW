using CLW.Model;
using CLW.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace CLW.ViewModel
{
    public class WatcherViewModel :BaseViewModel
    {

        public WatcherViewModel()
        {
            //design-time ctor
            Title = "Dummy";
            UnreadCount = 333;
            StatusMessage = "StatusMessage";
        }
      

        public WatcherViewModel(Model.WatcherModel _watchermodel)
        {
            cts = new CancellationTokenSource();
            model = _watchermodel;
            Title = model.CoreCustomLW.Name;
            StatusMessage = model.CoreCustomLW.StatusMessage;
            UnreadCount = model.CoreCustomLW.UnreadCount;
            notif(nameof(AvatarLetter));
            IsActive = model.CoreCustomLW.IsWatching;
            
            AvatarBackground = model.Color.HasValue? new SolidColorBrush(model.Color.Value): Fucs.RandomAvatarBackgroundBrush();
            model.CoreCustomLW.NewItems += (s, news) =>
            {
                //Trace.WriteLine("NewItems watcherViewModel handeler here");
                UnreadCount = model.CoreCustomLW.UnreadCount;
                StatusMessage = model.CoreCustomLW.StatusMessage;
                //UpdateVisibilitiesTemp();

            };
            model.CoreCustomLW.OnConnectionStarted += (s, news) =>
            {
                IsSpinnerVisible = true;
                //UpdateVisibilitiesTemp();
                //Trace.WriteLine("OnConnectionStarted here");
            };
            model.CoreCustomLW.OnConnectionEnded += (s, news) =>
            {
                IsSpinnerVisible = false;
                //UpdateVisibilitiesTemp();
                //Trace.WriteLine("OnConnectionEnded here");
            };
            model.CoreCustomLW.OnFatalError += (s, e) =>
            {
                Trace.WriteLine("OnFatalError here");
                IsActive = false;
                StatusMessage = model.CoreCustomLW.StatusMessage;
                FatalException = e;
                FatalExceptionStackMessage = Logger.BuildStackMessage(e);
                //throw e;
                Trace.WriteLine(FatalExceptionStackMessage);
                //UpdateVisibilitiesTemp();
            };
            model.CoreCustomLW.OnStarted += (s, news) =>
            {
                StatusMessage = model.CoreCustomLW.StatusMessage;
                isActive = true;
                notif(nameof(IsActive));
                //Trace.WriteLine("OnStarted here");
                //UpdateVisibilitiesTemp();
            };
            model.CoreCustomLW.OnStoped += (s, news) =>
            {
                StatusMessage = model.CoreCustomLW.StatusMessage;

                Trace.WriteLine("OnStoped here");
                IsActive = false;
                //UpdateVisibilitiesTemp();

            };
            model.CoreCustomLW.OnError += (e, r) => {
                StatusMessage = model.CoreCustomLW.StatusMessage;
                FailingException = model.CoreCustomLW.FailingException;
                FailingExceptionStackMessage = Logger.BuildStackMessage(FailingException);
                Trace.WriteLine("OneRROR here");

                Trace.WriteLine(FailingExceptionStackMessage);
                //UpdateVisibilitiesTemp();

            };
            model.CoreCustomLW.OnCheck += (e, r) => {
                StatusMessage = model.CoreCustomLW.StatusMessage;
                //UpdateVisibilitiesTemp();
            };
          
        }



        public WatcherViewModel(string title, int unread, string status, bool active)
        {
            //this ctor is used for design-time views only, that's the automated ViewModelLocator's job call thsi
            Title = title; UnreadCount = unread; StatusMessage = status; IsActive = active;
        }




        #region Properties

       

        public WatcherModel model { get; set; }


        




        private string title;
        public string Title
        {
            set { title = value; notif(nameof(Title)); }
            get { return title; }
        }

        private CancellationTokenSource cts;

        private bool isActive;
        public bool IsActive
        {
            set { isActive = value;
                if (model != null)
                {
                    if (isActive==true)
                    {
                        WatchingService.TryStartWatchingAsync(model);

                    }
                    else if(isActive==false)
                    {
                        WatchingService.TryStopWatchingAsync(model);
                    }
                }


                notif(nameof(IsActive)); }
            get { return isActive; }
        }


        private string statusMessage;
        public string StatusMessage
        {
            set { statusMessage = value; notif(nameof(StatusMessage)); }
            get { return statusMessage; }
        }


        private bool isSpinnerVisible;
        public bool IsSpinnerVisible
        {
            set { isSpinnerVisible = value; notif(nameof(IsSpinnerVisible)); }
            get { return isSpinnerVisible; }
        }




        private ImageSource avatarImage;
        public ImageSource AvatarImage
        {
            set { avatarImage = value; notif(nameof(AvatarImage)); }
            get { return avatarImage; }
        }


        private string avatarLetter;
        public string AvatarLetter
        {
            get { return Title.First().ToString(); }
        }


        private Brush avatarBackground;
        public Brush AvatarBackground
        {
            set { avatarBackground = value; notif(nameof(AvatarBackground)); }
            get { return avatarBackground; }
        }


        private Exception fatalException;
        public Exception FatalException
        {
            set { fatalException = value; notif(nameof(FatalException)); }
            get { return fatalException; }
        }
        
              private string fatalExceptionStackMessage;
        public string FatalExceptionStackMessage
        {
            set { fatalExceptionStackMessage = value; notif(nameof(FatalExceptionStackMessage)); }
            get { return fatalExceptionStackMessage; }
        }

        private Exception failingException;
        public Exception FailingException
        {
            set { failingException = value; notif(nameof(FailingException)); }
            get { return failingException; }
        }

        private string failingExceptionStackMessage;
        public string FailingExceptionStackMessage
        {
            set { failingExceptionStackMessage = value; notif(nameof(FailingExceptionStackMessage)); }
            get { return failingExceptionStackMessage; }
        }


        private bool hasAvatarImage;
        public bool HasAvatarImage
        {
            set { hasAvatarImage = value; notif(nameof(HasAvatarImage)); }
            get { return hasAvatarImage; }
        }




        private int unreadCount;
        public int UnreadCount
        {
            set { unreadCount = value; notif(nameof(UnreadCount)); }
            get { return unreadCount; }
        }



        static string SelectedTitleBrush = "PrimaryHueLightBrush";
        static string SelectedStatusMessageBrush = "PrimaryHueMidBrush";

        private Brush titleForeground = (Brush) Application.Current.FindResource("MaterialDesignBody");
        public Brush TitleForeground
        {
            set { titleForeground = value; notif(nameof(TitleForeground)); }
            get { return IsSelected? (Brush)Application.Current.FindResource(SelectedTitleBrush): (Brush)Application.Current.FindResource("MaterialDesignBody") ; }
        }

        private Brush statusMessageForeground = (Brush)Application.Current.FindResource("MaterialDesignBody");
        public Brush StatusMessageForeground
        {
            set { statusMessageForeground = value; notif(nameof(StatusMessageForeground)); }
            get { return IsSelected ? (Brush)Application.Current.FindResource(SelectedStatusMessageBrush) : (Brush)Application.Current.FindResource("MaterialDesignBodyLight"); }
        }



        private bool isSelected;
        public bool IsSelected
        {
            set { isSelected = value; notif(nameof(IsSelected)); notif(nameof(StatusMessageForeground)); notif(nameof(TitleForeground)); }
            get { return isSelected; }
        }




        #endregion

        #region Commands
        public MICommand OpenPropertiesWindow
        {
            get { return new MICommand(() => {
                Views.WatcherPropertiesWindow wpw = new Views.WatcherPropertiesWindow();
                wpw.DataContext = new ViewModel.WatcherPropertiesWindowViewModel(model);
                wpw.Show();
            }); }
        }
        #endregion


    }
}
