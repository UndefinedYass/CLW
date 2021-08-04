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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Globalization;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.ComponentModel;
using System.IO;
using System.Linq;
using Ookii.Dialogs.Wpf; 
using System.Xml.Linq;
using System.Windows.Shell;
using System.Xml.Serialization;


namespace CLW
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Session mainSession { get; private set; }

        public MainWindow()
        {
            CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.GetCultureInfo("en-US");
            InitializeComponent();


            //webServer = new WebServer(); //niy
            // DataContext = mainSession;
            mainSession = new Session();
            DataContext = mainSession;
        }



        public void PopupNews(IEnumerable<INotifableItem> news, IWatch listwatcher, string winTitle = "News", bool IsNotifyMode = true)
        {
            return;
            //GenericPopupWindow gpw = new GenericPopupWindow(); niy
            SoundPlayerAction spa = new SoundPlayerAction();
            if (IsNotifyMode) mainSession.PlayNotification();
           // gpw.setItemsSource(news, listwatcher, winTitle);
           // gpw.ShowDialog();
        }



        private async void devShowNotification_Click(object sender, RoutedEventArgs e)
        {


         
            var nnf = new CLWNotifWindow();
            nnf.Show();
            return;
            //MessageBox.Show("ok");
            //return;
            var a = new FsdmNew()
            {
                Title = "Dummy Notification Title",
                Link = "http://fsdm.ma/dummyhrtdfhdf",
                ID = "44444",
                date = "zzrhzr"

            };

            var dummynews = new List<FsdmNew>()
            {
               a 
            };
            ShowNotificationNews(dummynews.Cast<INotifableItem>(), null);
        }

        private void Cltry_NewItems(object sender, List<ExpandoLWItemObject> e)
        {
            MessageBox.Show($"you have {e.Count} news, one of which have as title: {e[0].Title} ");
        }

        public void ShowNotificationNews(IEnumerable<INotifableItem> news, IWatch listWathcer)
        {
            CLWNotifWindow nm = new CLWNotifWindow();

            nm.Init(news, listWathcer);
            nm.Owner = this;
            nm.ShowInTaskbar = false;

            nm.Show();
        }





        private void primaryButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //var fx = new MahApps.Metro.IconPacks.FileIconsExtension(PackIconFileIconsKind.AdobeAcrobat);
            //var fx = new MahApps.Metro.IconPacks.PackIconFontAwesome();
            
        }

        private void PackIconCodicons_Loaded(object sender, RoutedEventArgs e)
        {
           // loader2.SpinEasingFunction = new loa();
        }

        private void CloseWindowHIButt_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void maximizeHIButt_Copy_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState== WindowState.Maximized? WindowState.Normal: WindowState.Maximized;
        }

        private void minimizeHIBUTT_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void DragArea_DragOver(object sender, DragEventArgs e)
        {
            
        }

        private void DragArea_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            
        }

        private void DragArea_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();// dreag area 2
        }

        private void window_Header_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove(); // dreag area 1
        }

        private void ResizeHandle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.ResizeMode = ResizeMode.CanResizeWithGrip;
            
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
           
        }


        private async void loadListWatcherPresetButt_Click(object sender, RoutedEventArgs e)
        {
            Ookii.Dialogs.Wpf.VistaOpenFileDialog vofd = new VistaOpenFileDialog();
            vofd.DefaultExt = ".xml";
            vofd.InitialDirectory = MI.APP_DATA;
            bool? success = vofd.ShowDialog(this);
            if (!success.HasValue) return;
            if (!success.Value) return;




            CustomLW loaded;

            try
            {
                loaded = await mainSession.LoadXMLLW(vofd.FileName);

            }
            catch (Exception ex)
            {
                MessageBox.Show( ex.Message, "Parsing failed", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

           
            //if (loaded != null) //todo instead show message whn thnsg go wrong
               // MessageBox.Show("preset loaded successfilly"); // annoying
        }


        private async void AddWatcherButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("ok");
            CustomLW cltry;
            try
            {
                 cltry = await mainSession.LoadXMLLW(@"C:\TOOLS\fbhd-gui\xml\fsdmUploads.clw");

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
            

            if (cltry == null)
            {
                MessageBox.Show("wrong");
                return;
            }
            MessageBox.Show($"loaded clw preset with name: {cltry.Name}");
            cltry.StartWatching();


            cltry.NewItems += Cltry_NewItems;


            return;
        }

        private void image5_Loaded(object sender, RoutedEventArgs e)

        {
           // image5.Source = new BitmapImage(new Uri(@"C:\TOOLS\CLW\CLW\CLW\media\websitesExamples\USMBA-256.jpg"));
            //image5.Source = new BitmapImage(new Uri("http://fsdmfes.ac.ma/favicon.ico"));
        }
    }


    class loa : IEasingFunction
    {
        public double Ease(double normalizedTime)
        {
            return (Math.Floor(normalizedTime * 8)) * 1 / 8;
        }

        
    }



    public struct FsdmNew : ListWatcherItem, INotifableItem
    {
        public string ID { get; set; }
        public string PopupMessageString
        {
            get
            {
                return $"{Title} \n       {Link}";
            }
        }

        public string SubTitle { get { return date; } }

        public string Link { get; set; }
        public string Title { get; set; }
        public string date { get; set; }
        public string category { get; set; }


    }

}
