using CLW.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace CLW.ViewModel
{

    public enum NotificationViewType { SingleItem, MultipleItems }

    public class NotificationViewModel :BaseViewModel
    {

       
        public NotificationViewModel()
        {
            //used in design time
            Mode = NotificationViewType.SingleItem;
            Title = "Dummy Notification Title here";
            Link = "https://dummy.com/more_dummy_stuff";
            /*CountInfo.Add(new Run("33 news from "));
            CountInfo.Add(new Bold(new Run("Uploads"))); CountInfo.Add(new Run(", 33 in total."));
            */

            
        }

        public NotificationViewModel(IEnumerable<INewItem> news, WatcherModel _initiator)
        {
            //used irt
            //copied code from the old noticifactionwndow's code behind
            InitiatorWatcher = _initiator ;

            var Newslist = news.ToList();
            bool MultipleNews = Newslist.Count > 1;
            Mode = MultipleNews ? NotificationViewType.MultipleItems : NotificationViewType.SingleItem;

            Title = Newslist[0].Title;
            Link = Newslist[0].Link;

            CountInfo = $"{Newslist.Count} news from {InitiatorWatcher.CoreCustomLW.Name}, {Newslist.Count} in total.";

            AvatarImageSource = new BitmapImage(new Uri(@"pack://Application:,,,/media/node-eye-red-white-circles-play.png"));
            /* CountInfo.Add(new Run($"{Newslist.Count} news from "));
             CountInfo.Add(new Bold(new Run(InitiatorWatcher.CoreCustomLW.Name)));
             CountInfo.Add(new Run($", {Newslist.Count} in total."));
             */
            notif(nameof(CountInfo));

           

        }


        public event EventHandler OnRequestClose;



        private NotificationViewType mode;
        public NotificationViewType Mode
        {
            set { mode = value; notif(nameof(Mode)); }
            get { return mode; }
        }


        private ImageSource avatarImageSource;
        public ImageSource AvatarImageSource
        {
            set { avatarImageSource = value; notif(nameof(AvatarImageSource)); }
            get { return avatarImageSource; }
        }




        WatcherModel InitiatorWatcher { get; set; }

        public MICommand ShowNewsCommand { get { return new MICommand(onShowNews); } }
        public MICommand MarkAllSeenCommand { get { return new MICommand(onMarkAsSeen); } }

        //irrelevent because it's ok to hanfle ths in code behind
        public MICommand CloseCommand { get { return new MICommand(onClose); } }

       

        private string title;
        public string Title
        {
            set { title = value; notif(nameof(Title)); }
            get { return title; }
        }


        private string link;
        public string Link
        {
            set { link = value; notif(nameof(Link)); }
            get { return link; }
        }

        //33 news from uploads, 35 total news
        private string newsCountInfo;
        public string CountInfo
        {
            set { newsCountInfo = value; notif(nameof(CountInfo)); }
            get { return newsCountInfo; }
        }




        private void onShowNews()
        {
            //if mainwindow is open, select the required watccher
            //else , open it then select it
            Services.Utils.ShowMainWindow();
            

            ((MainViewModel)System.Windows.Application.Current.MainWindow.DataContext).SelectWatcherById(InitiatorWatcher.guid);
            onClose();
        }

      
        private void onMarkAsSeen()
        {
            onClose();
        }

        private void onClose()
        {
            if (OnRequestClose != null) OnRequestClose(this,new EventArgs());
        }
    }
}
