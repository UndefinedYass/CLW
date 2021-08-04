using System;
using System.Collections.Generic;
using System.Globalization;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CLW
{



    public class IntToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int threshold=0;
            if (parameter!=null)
            threshold = (int) parameter;
            return ((int)value > ((int)(threshold)) ? Visibility.Visible : Visibility.Collapsed);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }



    /// <summary>
    /// Interaction logic for WatcherUC.xaml
    /// </summary>
    public partial class WatcherUC : UserControl
    {
        public WatcherUC()
        {
            InitializeComponent();
            //if(DataContext!=null)
            //CoreCustomListWatcher = (CustomLW)DataContext;
        }







        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            CoreCustomListWatcher = (CustomLW)DataContext;

        }







        public CustomLW CoreCustomListWatcher
        {
            get { return (CustomLW)GetValue(CoreCustomListWatcherProperty); }
            set { SetValue(CoreCustomListWatcherProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CoreCustomListWatcher.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CoreCustomListWatcherProperty =
            DependencyProperty.Register("CoreCustomListWatcher", typeof(CustomLW), typeof(WatcherUC), new PropertyMetadata((CustomLW) null));



        private void mainButton_Click(object sender, RoutedEventArgs e)
        {
           // mainPopup.IsOpen = true; niy

        }

        private void intervalIncTb_OnDecrease(object sender, RoutedEventArgs e)
        {
            CoreCustomListWatcher.Interval -= 30000;

        }

        private void intervalIncTb_OnIncrease(object sender, RoutedEventArgs e)
        {
            CoreCustomListWatcher.Interval += 30000;

        }

        private void intervalIncTb_Loaded(object sender, RoutedEventArgs e)
        {
            //intervalIncTb.IncreaseFunction = null; //niy //important to override the default int increaser
            return;
        }


        private async void StartStopButt_Click(object sender, RoutedEventArgs e)
        {

            if (CoreCustomListWatcher.IsWatching)
            {
                CoreCustomListWatcher.StopWatching();
            }
            else
            {
                CoreCustomListWatcher.StartWatching();
            }
            await Task.Run(() => { });

        }

        //static MainWindow mw = (MainWindow)Application.Current.MainWindow;
        private void UnreadNewsButt_Click(object sender, RoutedEventArgs e)
        {
            //mw.PopupNews(CoreCustomListWatcher.UnreadNews, CoreCustomListWatcher, CoreCustomListWatcher.Name, false);
        }




        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Title.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(WatcherUC), new PropertyMetadata("TITLE"));






      




        public int NewsCount
        {
            get { return (int)GetValue(NewsCountProperty); }
            set { SetValue(NewsCountProperty, value); }
        }

        // Using a DependencyProperty as the backing store for NewsCount.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NewsCountProperty =
            DependencyProperty.Register("NewsCount", typeof(int), typeof(WatcherUC), new PropertyMetadata(0));





        public bool ShowConnectingSpinner
        {
            get { return (bool)GetValue(ShowConnectingSpinnerProperty); }
            set { SetValue(ShowConnectingSpinnerProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowConnectingSpinner.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowConnectingSpinnerProperty =
            DependencyProperty.Register("ShowConnectingSpinner", typeof(bool), typeof(WatcherUC), new PropertyMetadata(false));



        public string StateText
        {
            get { return (string)GetValue(StateTextProperty); }
            set { SetValue(StateTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for StateText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StateTextProperty =
            DependencyProperty.Register("StateText", typeof(string), typeof(WatcherUC), new PropertyMetadata("state"));






        public ImageSource Favico
        {
            get { return (ImageSource)GetValue(FavicoProperty); }
            set { SetValue(FavicoProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Favico.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FavicoProperty =
            DependencyProperty.Register("Favico", typeof(ImageSource), typeof(WatcherUC), new PropertyMetadata(null));

        private async void toggle_Checked(object sender, RoutedEventArgs e)
        {
            CoreCustomListWatcher.StartWatching();
        }

        private async void toggle_Unchecked(object sender, RoutedEventArgs e)
        {
            CoreCustomListWatcher.StopWatching();
        }
    }
}
