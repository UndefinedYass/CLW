using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Text.RegularExpressions;
using System.ComponentModel;
using System.Collections.ObjectModel;
using CLW.Services;

namespace CLW
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool is_toy_page_visible;
       // public Session mainSession { get; private set; }

        public MainWindow()
        {
            
            CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.GetCultureInfo("en-US");
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.GetCultureInfo("en-US");
            InitializeComponent();
            watcher_win.Drop += (ss, se) =>
            {
                ShowMessage("File(s) droped");
                string[]files = (string[]) se.Data.GetData(DataFormats.FileDrop);
                foreach(string pth in files)
                {
                    if (System.IO.Path.GetExtension(pth).ToLower() == ".clw")
                        ((ViewModel.MainViewModel)DataContext).LoadWatcherFromFilePathCommand.Execute(pth);
                }
            };
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            Application.Current.MainWindow = null;
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

        private void listBox_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                //todo: unloead selected watcher                
            }
        }

        

        /// <summary>
        /// callable from anywhere anytime, any thread, exceptions-safe
        /// </summary>
        /// <param name="str">message to pop up at snackbar,</param>
        public static void ShowMessage(string str)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                ((MainWindow)Application.Current.MainWindow)?.Snackbar?.MessageQueue?.Enqueue(str);
            });
        }



        

        private void brushesToy_Loaded(object sender, RoutedEventArgs e)
        {
            string brushNamesRaw = @"PrimaryHueLightBrush PrimaryHueLightForegroundBrush PrimaryHueMidBrush PrimaryHueMidForegroundBrush PrimaryHueDarkBrush PrimaryHueDarkForegroundBrush SecondaryHueLightBrush SecondaryHueLightForegroundBrush SecondaryHueMidBrush SecondaryHueMidForegroundBrush SecondaryHueDarkBrush SecondaryHueDarkForegroundBrush MaterialDesignBackground MaterialDesignPaper MaterialDesignCardBackground MaterialDesignToolBarBackground MaterialDesignBody MaterialDesignBodyLight MaterialDesignColumnHeader MaterialDesignCheckBoxOff MaterialDesignCheckBoxDisabled MaterialDesignTextBoxBorder MaterialDesignDivider MaterialDesignSelection MaterialDesignFlatButtonClick MaterialDesignFlatButtonRipple MaterialDesignToolTipBackground MaterialDesignChipBackground MaterialDesignSnackbarBackground MaterialDesignSnackbarMouseOver MaterialDesignSnackbarRipple MaterialDesignTextFieldBoxBackground MaterialDesignTextFieldBoxHoverBackground MaterialDesignTextFieldBoxDisabledBackground MaterialDesignTextAreaBorder MaterialDesignTextAreaInactiveBorder";
            var separat = brushNamesRaw.Split(' ');
            separat = separat.Select((s) => s.Trim()).ToArray();
            BrushesToyPageCB.ItemsSource = separat;
            BrushesToyPanelCB.ItemsSource = separat;
            BrushesToyTextCB.ItemsSource = separat;
            BrushesToyPageCB.SelectionChanged += (s, ee) =>
             {
                 brushesToy_Page.SetResourceReference(Grid.BackgroundProperty, BrushesToyPageCB.SelectedItem.ToString());
             };
            BrushesToyPanelCB.SelectionChanged += (s, ee) =>
            {
                brushesToy_Panel.SetResourceReference(Grid.BackgroundProperty, BrushesToyPanelCB.SelectedItem.ToString());
            };
            BrushesToyTextCB.SelectionChanged += (s, ee) =>
            {
                BrushesToy_Body.SetResourceReference(Grid.BackgroundProperty, BrushesToyTextCB.SelectedItem.ToString());
                BrushesToy_Text.SetResourceReference(TextBlock.ForegroundProperty, BrushesToyTextCB.SelectedItem.ToString());
            };
        }

       
        

        private void d_key_pressed()
        {
            //toggling dev view
            is_toy_page_visible = !is_toy_page_visible;
            brushesToy_Page.Visibility = is_toy_page_visible ? Visibility.Visible : Visibility.Collapsed;
            var oc = new ObservableCollection<string>();
            oc.Add($"ConfigService Instances: {ConfigService.CCInstances}");
            oc.Add($"WatchingService Instances: {WatchingService.CCInstances}");

            devStatsListBox.ItemsSource = oc;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.D)
            {//todo: only execute if isDevMode
                d_key_pressed();
            }
        }

        

       
        private void WatchersList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            return;
            ((ViewModel.MainViewModel)DataContext).SelectWatcherById(((ViewModel.WatcherViewModel)  WatchersList.SelectedItem).model.guid);
        }

        private void WatchersList_DragEnter(object sender, DragEventArgs e)
        {
            
        }
      




        private void profilerButt_Click(object sender, RoutedEventArgs e)
        {
            var mem = GC.GetTotalMemory(true);
            ShowMessage($"Memo: {mem / 1000000} mb");
            //expressions
            //this is fucking awesome
            //bool resulr = compiled("ok_yass_youre_a_genuis");
            // ShowMessage($"result: {resulr}");
        }

        private void TestButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in WatchingService.Instance.Watchers)
            {
                item?.CoreCustomLW?.CloseAsync();
            }
            var mem = GC.GetTotalMemory(true);
            ShowMessage($"Memo: {mem / 1000000} mb");
            return;
        }
    }


    class loa : IEasingFunction
    {
        public double Ease(double normalizedTime)
        {
            return (Math.Floor(normalizedTime * 8)) * 1 / 8;
        }

        
    }



   
}
