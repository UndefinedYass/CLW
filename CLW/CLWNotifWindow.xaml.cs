using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;


namespace CLW
{
    /// <summary>
    /// Interaction logic for CLWNotifWindow.xaml
    /// </summary>
    public partial class CLWNotifWindow : Window
    {
        public CLWNotifWindow()
        {
            InitializeComponent();
        }


        private void CloseButt_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            animateSlideUp();
        }

        private void animateSlideUp()
        {

            Left = SystemParameters.PrimaryScreenWidth - Width - 2;

            double H = SystemParameters.PrimaryScreenHeight;

            Storyboard sb = new Storyboard();
            sb.Duration = new Duration(TimeSpan.FromSeconds(0.5));
            DoubleAnimation ra = new DoubleAnimation() { From = H - Height, To = H - Height - 39, Duration = sb.Duration };
            ra.EasingFunction = new ElasticEase() { Oscillations = 1, Springiness = 0.05 };

            Storyboard.SetTarget(ra, this);
            Storyboard.SetTargetProperty(ra, new PropertyPath(Window.TopProperty));
            sb.Children.Add(ra);
            sb.Begin();
        }


        public List<INotifableItem> Newslist { get; set; }

        public IWatch ListWatcherObj { get; set; }
        public void Init(IEnumerable<INotifableItem> news, IWatch listWatcher)
        {
            ListWatcherObj = listWatcher;
            Newslist = news.ToList();
            MultipleNews = Newslist.Count > 1;

            FirstTitle = Newslist[0].Title;

            
            title.Text = FirstTitle;
            countInfo.Visibility = MultipleNews ? Visibility.Visible : Visibility.Collapsed;

            fastLink .Visibility = !MultipleNews ? Visibility.Visible : Visibility.Collapsed;
            fastLink.Text = Newslist[0].Link;
            countInfo.Text = $"and {Newslist.Count-1} more..";

        }







        public string FirstTitle
        {
            get { return (string)GetValue(FirstTitleProperty); }
            set { SetValue(FirstTitleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FirstTitle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FirstTitleProperty =
            DependencyProperty.Register("FirstTitle", typeof(string), typeof(CLWNotifWindow), new PropertyMetadata(null));



        public bool MultipleNews
        {
            get { return (bool)GetValue(MultipleNewsProperty); }
            set { SetValue(MultipleNewsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MultipleNews.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MultipleNewsProperty =
            DependencyProperty.Register("MultipleNews", typeof(bool), typeof(CLWNotifWindow), new PropertyMetadata(false));



        MainWindow mw = (MainWindow)Application.Current.MainWindow;

        private void ShowNewsButt_Click(object sender, RoutedEventArgs e)
        {
            this.Closed += (s, a) =>
            {
                // mw.PopupNews(Newslist, ListWatcherObj, "News", false); // only suitable for the old fbhd project
                mw.WindowState = WindowState.Normal;
                mw.Activate();

            };
            Close();
        }

        private void MarkAsReadButt_Click(object sender, RoutedEventArgs e)
        {
            this.Closed += (s, a) =>
            {
                ListWatcherObj.MarkAllAsRead();
            };
            Close();

        }

        private void DownloadBtton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
